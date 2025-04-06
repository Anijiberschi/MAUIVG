using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

[QueryProperty(nameof(ISBN), "selectedBook")]
public partial class DetailsViewModel : ObservableObject
{
    private string? _isbn;

    // Propriété ISBN avec accesseurs explicites
    public string? ISBN
    {
        get => _isbn;
        set
        {
            if (_isbn != value)
            {
                _isbn = value;
                OnPropertyChanged();
            }
        }
    }

    [ObservableProperty]
    public string? title;

    [ObservableProperty]
    public string? author;

    [ObservableProperty]
    public string? publisher;

    [ObservableProperty]
    public string? category;

    [ObservableProperty]
    public decimal price;

    [ObservableProperty]
    public int quantity;

    [ObservableProperty]
    public DateTime publicationDate = DateTime.Now;

    [ObservableProperty]
    public string? location;

    [ObservableProperty]
    public bool isAvailable = true;

    [ObservableProperty]
    public string? coverUrl;

    [ObservableProperty]
    public string? language;

    [ObservableProperty]
    public int pageCount;

    [ObservableProperty]
    public DateTime lastScanned = DateTime.Now;

    [ObservableProperty]
    public string? notes;

    [ObservableProperty]
    public string? scanBufferContent;

    [ObservableProperty]
    public bool emulatorON_OFF = false;

    readonly BarcodeScannerService _scannerService;
    IDispatcherTimer _emulator = Application.Current!.Dispatcher.CreateTimer();

    public DetailsViewModel(BarcodeScannerService scannerService)
    {
        _scannerService = scannerService;
        _scannerService.OpenPort();
        _scannerService.ScanBuffer.Changed += OnSerialDataReception;
        _scannerService.BarcodeScanned += OnBarcodeScanned;

        _emulator.Interval = TimeSpan.FromSeconds(5);
        _emulator.Tick += EmulatorTimerTick;
    }

    private void EmulatorTimerTick(object? sender, EventArgs e)
    {
        // Simuler un scan d'ISBN toutes les 5 secondes
        SimulateScan();
    }

    private void SimulateScan()
    {
        // Simuler la réception d'un ISBN
        var sampleIsbns = new[]
        {
            "9780061120084", // To Kill a Mockingbird
            "9780141187761", // 1984
            "9782070368228", // L'Étranger
            "9780307474278", // Le Petit Prince
            "9780618640157"  // Le Seigneur des Anneaux
        };

        var random = new Random();
        var isbn = sampleIsbns[random.Next(sampleIsbns.Length)];

        // Ajouter au buffer comme si c'était scanné
        _scannerService.ScanBuffer.Enqueue(isbn + "\r\n");
    }

    partial void OnEmulatorON_OFFChanged(bool value)
    {
        if (value)
        {
            _emulator.Start();
        }
        else
        {
            _emulator.Stop();
        }
    }

    private void OnBarcodeScanned(object? sender, BarcodeScannedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Barcode))
        {
            // Mettre à jour l'ISBN avec le code-barres scanné
            ISBN = e.Barcode;

            // Rechercher si le livre existe déjà
            var existingBook = Globals.MyBooks.FirstOrDefault(b => b.ISBN == e.Barcode);
            if (existingBook != null)
            {
                // Charger les données du livre existant
                LoadBookData(existingBook);
                Shell.Current.DisplayAlert("Livre trouvé", $"Livre trouvé: {existingBook.Title}", "OK");
            }
            else
            {
                // Initialiser un nouveau livre
                Title = "Nouveau livre";
                Author = "";
                Publisher = "";
                Category = "Non catégorisé";
                Price = 0;
                Quantity = 1;
                PublicationDate = DateTime.Now;
                Location = "Non spécifié";
                IsAvailable = true;
                Language = "";
                PageCount = 0;
                CoverUrl = "";
                Notes = "";
                LastScanned = DateTime.Now;

                Shell.Current.DisplayAlert("Nouveau livre", "Scannez un livre qui n'existe pas encore dans la base de données. Veuillez remplir les informations.", "OK");
            }
        }
    }

    private void LoadBookData(Book book)
    {
        Title = book.Title;
        Author = book.Author;
        Publisher = book.Publisher;
        Category = book.Category;
        Price = book.Price;
        Quantity = book.Quantity;
        PublicationDate = book.PublicationDate;
        Location = book.Location;
        IsAvailable = book.IsAvailable;
        Language = book.Language;
        PageCount = book.PageCount;
        CoverUrl = book.CoverUrl;
        Notes = book.Notes;
        LastScanned = DateTime.Now; // Mettre à jour la date de scan
    }

    private void OnSerialDataReception(object? sender, EventArgs arg)
    {
        if (sender is BarcodeScannerService.QueueBuffer buffer && buffer.Count > 0)
        {
            ScanBufferContent += buffer.Dequeue()?.ToString();
            OnPropertyChanged(nameof(ScanBufferContent));
        }
    }

    internal void RefreshPage()
    {
        // Si un ISBN est défini, chercher le livre correspondant
        if (!string.IsNullOrEmpty(ISBN))
        {
            var book = Globals.MyBooks.FirstOrDefault(b => b.ISBN == ISBN);
            if (book != null)
            {
                LoadBookData(book);
            }
        }
    }

    internal void ClosePage()
    {
        _scannerService.ScanBuffer.Changed -= OnSerialDataReception;
        _scannerService.BarcodeScanned -= OnBarcodeScanned;
        _scannerService.ClosePort();

        if (EmulatorON_OFF)
        {
            EmulatorON_OFF = false;
        }
    }

    [RelayCommand]
    internal void SaveBook()
    {
        var book = Globals.MyBooks.FirstOrDefault(b => b.ISBN == ISBN);

        if (book != null)
        {
            // Mettre à jour un livre existant
            book.Title = Title ?? string.Empty;
            book.Author = Author ?? string.Empty;
            book.Publisher = Publisher ?? string.Empty;
            book.Category = Category ?? string.Empty;
            book.Price = Price;
            book.Quantity = Quantity;
            book.PublicationDate = PublicationDate;
            book.Location = Location ?? string.Empty;
            book.IsAvailable = IsAvailable;
            book.Language = Language ?? string.Empty;
            book.PageCount = PageCount;
            book.CoverUrl = CoverUrl ?? string.Empty;
            book.Notes = Notes ?? string.Empty;
            book.LastScanned = DateTime.Now;

            Shell.Current.DisplayAlert("Succès", "Livre mis à jour avec succès", "OK");
        }
        else if (!string.IsNullOrEmpty(ISBN))
        {
            // Créer un nouveau livre
            var newBook = new Book
            {
                ISBN = ISBN,
                Title = Title ?? "Nouveau livre",
                Author = Author ?? string.Empty,
                Publisher = Publisher ?? string.Empty,
                Category = Category ?? "Non catégorisé",
                Price = Price,
                Quantity = Quantity,
                PublicationDate = PublicationDate,
                Location = Location ?? "Non spécifié",
                IsAvailable = IsAvailable,
                Language = Language ?? string.Empty,
                PageCount = PageCount,
                CoverUrl = CoverUrl ?? string.Empty,
                Notes = Notes ?? string.Empty,
                LastScanned = DateTime.Now
            };

            Globals.MyBooks.Add(newBook);
            Shell.Current.DisplayAlert("Succès", "Nouveau livre ajouté avec succès", "OK");
        }
        else
        {
            Shell.Current.DisplayAlert("Erreur", "Veuillez scanner ou entrer un ISBN valide", "OK");
        }
    }

    [RelayCommand]
    internal async Task SearchISBN()
    {
        if (string.IsNullOrEmpty(ISBN))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez d'abord entrer un ISBN", "OK");
            return;
        }

        // Simulation d'une recherche en ligne
        await Shell.Current.DisplayAlert("Recherche", $"Recherche en ligne pour l'ISBN {ISBN}...", "OK");

        // Dans une vraie application, nous ferions une requête à une API comme Google Books
        // Pour cet exemple, nous simulons juste un résultat
        var random = new Random();
        if (random.Next(2) == 0) // 50% de chance de "trouver" le livre
        {
            Title = $"Livre trouvé pour {ISBN}";
            Author = "Auteur trouvé en ligne";
            Publisher = "Éditeur trouvé en ligne";
            Category = "Fiction";
            PublicationDate = new DateTime(2020, 1, 1);
            Language = "Français";
            PageCount = 300;

            await Shell.Current.DisplayAlert("Succès", "Informations trouvées en ligne. Vous pouvez compléter les autres détails.", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Information", "Aucune information trouvée en ligne pour cet ISBN. Veuillez entrer les détails manuellement.", "OK");
        }
    }
}