using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Service;

public class CSVServices
{
    // Liste pour stocker les propriétés sélectionnées pour l'export
    public List<string> SelectedProperties { get; set; } = new List<string>();

    public CSVServices()
    {
        // Par défaut, toutes les propriétés sont sélectionnées
        var properties = typeof(Book).GetProperties();
        foreach (var prop in properties)
        {
            SelectedProperties.Add(prop.Name);
        }
    }

    public async Task<List<Book>> LoadData()
    {
        List<Book> list = [];

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Sélectionnez un fichier CSV"
        });

        if (result != null)
        {
            var lines = await File.ReadAllLinesAsync(result.FullPath, Encoding.UTF8);

            if (lines.Length == 0)
            {
                await Shell.Current.DisplayAlert("Erreur", "Le fichier CSV est vide", "OK");
                return list;
            }

            var headers = lines[0].Split(';');
            var properties = typeof(Book).GetProperties();

            for (int i = 1; i < lines.Length; i++)
            {
                Book obj = new();

                var values = lines[i].Split(';');

                for (int j = 0; j < headers.Length; j++)
                {
                    var property = properties.FirstOrDefault(p => p.Name.Equals(headers[j], StringComparison.OrdinalIgnoreCase));

                    if (property != null && j < values.Length && !string.IsNullOrEmpty(values[j]))
                    {
                        try
                        {
                            // Traitement spécifique pour les dates
                            if (property.PropertyType == typeof(DateTime))
                            {
                                if (DateTime.TryParse(values[j], out DateTime dateValue))
                                {
                                    property.SetValue(obj, dateValue);
                                }
                            }
                            // Traitement spécifique pour les booléens
                            else if (property.PropertyType == typeof(bool))
                            {
                                if (bool.TryParse(values[j], out bool boolValue))
                                {
                                    property.SetValue(obj, boolValue);
                                }
                            }
                            // Traitement spécifique pour les nombres décimaux
                            else if (property.PropertyType == typeof(decimal))
                            {
                                if (decimal.TryParse(values[j], out decimal decimalValue))
                                {
                                    property.SetValue(obj, decimalValue);
                                }
                            }
                            // Traitement spécifique pour les nombres entiers
                            else if (property.PropertyType == typeof(int))
                            {
                                if (int.TryParse(values[j], out int intValue))
                                {
                                    property.SetValue(obj, intValue);
                                }
                            }
                            // Pour les autres types
                            else
                            {
                                object value = Convert.ChangeType(values[j], property.PropertyType);
                                property.SetValue(obj, value);
                            }
                        }
                        catch (Exception ex)
                        {
                            await Shell.Current.DisplayAlert("Erreur", $"Erreur lors de la conversion: {ex.Message}", "OK");
                        }
                    }
                }

                list.Add(obj);
            }

            await Shell.Current.DisplayAlert("Succès", $"{list.Count} livres ont été importés", "OK");
        }
        return list;
    }

    public async Task PrintData<T>(List<T> data)
    {
        if (data.Count == 0)
        {
            await Shell.Current.DisplayAlert("Erreur", "Aucune donnée à exporter", "OK");
            return;
        }

        var csv = new StringBuilder();
        var properties = typeof(T).GetProperties()
            .Where(p => SelectedProperties.Contains(p.Name))
            .ToList();

        // En-tête
        csv.AppendLine(string.Join(";", properties.Select(p => p.Name)));

        // Données
        foreach (var item in data)
        {
            var values = properties.Select(p => {
                var value = p.GetValue(item);

                // Formatage des dates
                if (value is DateTime dateValue)
                {
                    return dateValue.ToString("yyyy-MM-dd HH:mm:ss");
                }

                return value?.ToString() ?? "";
            });

            csv.AppendLine(string.Join(";", values));
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));
        var fileSaverResult = await FileSaver.Default.SaveAsync("Books.csv", stream);

        if (fileSaverResult.IsSuccessful)
        {
            await Shell.Current.DisplayAlert("Succès", "Fichier CSV exporté avec succès", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Erreur", "Échec de l'exportation du fichier CSV", "OK");
        }
    }

    // Méthode pour configurer les propriétés à exporter
    public async Task ConfigureExportProperties()
    {
        // Cette méthode pourrait afficher une popup permettant à l'utilisateur 
        // de sélectionner les propriétés à inclure dans l'export CSV
        // Pour l'instant, on utilise toutes les propriétés
    }
}