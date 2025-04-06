using Microsoft.Maui.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.Service;

public class JSONServices
{
    private readonly string _serverUrl = "https://185.157.245.38:5000"; // URL du serveur
    private readonly string _fileName = "MyBooks.json"; // Nom du fichier JSON

    internal async Task<List<Book>> GetBooks()
    {
        var url = $"{_serverUrl}/json?FileName={_fileName}";
        List<Book> bookList = new();

        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using HttpClient httpClient = new(handler);
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStreamAsync();
                bookList = JsonSerializer.Deserialize<List<Book>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Book>();

                await Shell.Current.DisplayAlert("Succès", $"{bookList.Count} livres ont été chargés depuis le serveur", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", $"Erreur lors du chargement des données: {response.StatusCode}", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", $"Exception: {ex.Message}", "OK");
        }

        return bookList;
    }

    internal async Task SetBooks(List<Book> bookList)
    {
        var url = $"{_serverUrl}/json";

        try
        {
            MemoryStream stream = new();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            JsonSerializer.Serialize(stream, bookList, options);
            stream.Position = 0;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using HttpClient httpClient = new(handler);
            var fileContent = new ByteArrayContent(stream.ToArray());

            var content = new MultipartFormDataContent
            {
                { fileContent, "file", _fileName }
            };

            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Succès", "Les livres ont été sauvegardés avec succès sur le serveur", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", $"Erreur lors de l'envoi des données: {response.StatusCode}", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", $"Exception: {ex.Message}", "OK");
        }
    }

    // Méthode pour initialiser des données d'exemple si nécessaire
    internal List<Book> GetSampleBooks()
    {
        return new List<Book>
        {
            new Book
            {
                ISBN = "9780061120084",
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                Publisher = "HarperCollins",
                Category = "Fiction",
                Price = 12.99m,
                Quantity = 15,
                PublicationDate = new DateTime(1960, 7, 11),
                Location = "Section A, Étagère 3",
                Language = "Anglais",
                PageCount = 336,
                CoverUrl = "https://covers.openlibrary.org/b/id/8314315-L.jpg",
                IsAvailable = true
            },
            new Book
            {
                ISBN = "9780141187761",
                Title = "1984",
                Author = "George Orwell",
                Publisher = "Penguin Books",
                Category = "Science Fiction",
                Price = 9.99m,
                Quantity = 8,
                PublicationDate = new DateTime(1949, 6, 8),
                Location = "Section B, Étagère 2",
                Language = "Anglais",
                PageCount = 328,
                CoverUrl = "https://covers.openlibrary.org/b/id/8575741-L.jpg",
                IsAvailable = true
            },
            new Book
            {
                ISBN = "9782070368228",
                Title = "L'Étranger",
                Author = "Albert Camus",
                Publisher = "Gallimard",
                Category = "Fiction",
                Price = 8.50m,
                Quantity = 5,
                PublicationDate = new DateTime(1942, 5, 19),
                Location = "Section C, Étagère 4",
                Language = "Français",
                PageCount = 184,
                CoverUrl = "https://covers.openlibrary.org/b/id/8231990-L.jpg",
                IsAvailable = true
            },
            new Book
            {
                ISBN = "9780307474278",
                Title = "Le Petit Prince",
                Author = "Antoine de Saint-Exupéry",
                Publisher = "Reynal & Hitchcock",
                Category = "Jeunesse",
                Price = 7.99m,
                Quantity = 12,
                PublicationDate = new DateTime(1943, 4, 6),
                Location = "Section D, Étagère 1",
                Language = "Français",
                PageCount = 96,
                CoverUrl = "https://covers.openlibrary.org/b/id/8579151-L.jpg",
                IsAvailable = true
            },
            new Book
            {
                ISBN = "9780618640157",
                Title = "Le Seigneur des Anneaux",
                Author = "J.R.R. Tolkien",
                Publisher = "Houghton Mifflin",
                Category = "Fantasy",
                Price = 22.99m,
                Quantity = 3,
                PublicationDate = new DateTime(1954, 7, 29),
                Location = "Section B, Étagère 5",
                Language = "Anglais",
                PageCount = 1178,
                CoverUrl = "https://covers.openlibrary.org/b/id/8477639-L.jpg",
                IsAvailable = true
            }
        };
    }
}