using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyApp.Model;

public class Book : INotifyPropertyChanged
{
    private string _isbn = string.Empty;
    private string _title = string.Empty;
    private string _author = string.Empty;
    private string _publisher = string.Empty;
    private string _category = string.Empty;
    private decimal _price;
    private int _quantity;
    private DateTime _publicationDate = DateTime.Now;
    private string _location = string.Empty;
    private bool _isAvailable = true;
    private string _coverUrl = string.Empty;
    private string _language = string.Empty;
    private int _pageCount;
    private DateTime _lastScanned = DateTime.Now;
    private string _notes = string.Empty;

    // Code ISBN (peut être scanné)
    public string ISBN
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

    // Titre du livre
    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    // Auteur du livre
    public string Author
    {
        get => _author;
        set
        {
            if (_author != value)
            {
                _author = value;
                OnPropertyChanged();
            }
        }
    }

    // Éditeur
    public string Publisher
    {
        get => _publisher;
        set
        {
            if (_publisher != value)
            {
                _publisher = value;
                OnPropertyChanged();
            }
        }
    }

    // Catégorie (Roman, Technique, Science-fiction, etc.)
    public string Category
    {
        get => _category;
        set
        {
            if (_category != value)
            {
                _category = value;
                OnPropertyChanged();
            }
        }
    }

    // Prix du livre
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                OnPropertyChanged();
            }
        }
    }

    // Nombre d'exemplaires disponibles
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }
    }

    // Date de publication
    public DateTime PublicationDate
    {
        get => _publicationDate;
        set
        {
            if (_publicationDate != value)
            {
                _publicationDate = value;
                OnPropertyChanged();
            }
        }
    }

    // Emplacement physique (rayon, étagère)
    public string Location
    {
        get => _location;
        set
        {
            if (_location != value)
            {
                _location = value;
                OnPropertyChanged();
            }
        }
    }

    // Disponibilité du livre
    public bool IsAvailable
    {
        get => _isAvailable;
        set
        {
            if (_isAvailable != value)
            {
                _isAvailable = value;
                OnPropertyChanged();
            }
        }
    }

    // URL de la couverture du livre
    public string CoverUrl
    {
        get => _coverUrl;
        set
        {
            if (_coverUrl != value)
            {
                _coverUrl = value;
                OnPropertyChanged();
            }
        }
    }

    // Langue du livre
    public string Language
    {
        get => _language;
        set
        {
            if (_language != value)
            {
                _language = value;
                OnPropertyChanged();
            }
        }
    }

    // Nombre de pages
    public int PageCount
    {
        get => _pageCount;
        set
        {
            if (_pageCount != value)
            {
                _pageCount = value;
                OnPropertyChanged();
            }
        }
    }

    // Date du dernier scan
    public DateTime LastScanned
    {
        get => _lastScanned;
        set
        {
            if (_lastScanned != value)
            {
                _lastScanned = value;
                OnPropertyChanged();
            }
        }
    }

    // Notes supplémentaires
    public string Notes
    {
        get => _notes;
        set
        {
            if (_notes != value)
            {
                _notes = value;
                OnPropertyChanged();
            }
        }
    }

    // Implémentation de INotifyPropertyChanged pour le data binding
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}