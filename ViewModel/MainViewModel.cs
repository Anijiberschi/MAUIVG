using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Book> MyObservableList { get; } = [];
    private readonly JSONServices _jsonService;
    private readonly CSVServices _csvServices;

    public MainViewModel(JSONServices jsonService, CSVServices csvServices)
    {
        _jsonService = jsonService;
        _csvServices = csvServices;
    }

    [RelayCommand]
    internal async Task GoToDetails(string isbn)
    {
        IsBusy = true;

        await Shell.Current.GoToAsync("DetailsView", true, new Dictionary<string, object>
        {
            {"selectedBook", isbn}
        });

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task GoToGraph()
    {
        IsBusy = true;

        await Shell.Current.GoToAsync("GraphView", true);

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task PrintToCSV()
    {
        IsBusy = true;

        await _csvServices.PrintData(Globals.MyBooks);

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task LoadFromCSV()
    {
        IsBusy = true;

        Globals.MyBooks = await _csvServices.LoadData();
        await RefreshPage();

        IsBusy = false;
    }

    [RelayCommand]
    internal async Task ConfigureCSV()
    {
        var popup = new PropertySelectorPopup(_csvServices);
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
    }

    [RelayCommand]
    internal async Task UploadJSON()
    {
        IsBusy = true;

        await _jsonService.SetBooks(Globals.MyBooks);

        IsBusy = false;
    }

    internal async Task RefreshPage()
    {
        MyObservableList.Clear();

        if (Globals.MyBooks.Count == 0)
        {
            // Essayer de charger depuis le serveur
            try
            {
                Globals.MyBooks = await _jsonService.GetBooks();
            }
            catch (Exception ex)
            {
                // En cas d'erreur, utiliser des données d'exemple
                Globals.MyBooks = _jsonService.GetSampleBooks();
                await Shell.Current.DisplayAlert("Info", "Impossible de se connecter au serveur. Utilisation de données d'exemple.", "OK");
            }
        }

        foreach (var item in Globals.MyBooks)
        {
            MyObservableList.Add(item);
        }
    }

    [RelayCommand]
    internal async Task SearchBooks(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            // Si la recherche est vide, afficher tous les livres
            await RefreshPage();
            return;
        }

        // Convertir en minuscules pour une recherche insensible à la casse
        searchTerm = searchTerm.ToLower();

        // Filtrer les livres selon divers critères
        var filteredBooks = Globals.MyBooks.Where(b =>
            b.ISBN.Contains(searchTerm) ||
            b.Title.ToLower().Contains(searchTerm) ||
            b.Author.ToLower().Contains(searchTerm) ||
            b.Publisher.ToLower().Contains(searchTerm) ||
            b.Category.ToLower().Contains(searchTerm)
        ).ToList();

        // Mettre à jour la liste observable
        MyObservableList.Clear();
        foreach (var book in filteredBooks)
        {
            MyObservableList.Add(book);
        }
    }
}