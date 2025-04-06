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
    public async Task<List<StrangeAnimal>> LoadData()
    {
        List<StrangeAnimal> list = [];

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Sélectionnez un fichier CSV"
        });

        if (result != null)
        {
            var lines = await File.ReadAllLinesAsync(result.FullPath, Encoding.UTF8);

            var headers = lines[0].Split(';');
            var properties = typeof(StrangeAnimal).GetProperties();

            for (int i = 1; i < lines.Length; i++)
            {
                StrangeAnimal obj = new();

                var values = lines[i].Split(';');

                for (int j = 0; j < headers.Length; j++)
                {
                    var property = properties.FirstOrDefault(p => p.Name.Equals(headers[j], StringComparison.OrdinalIgnoreCase));
                    
                    if (property != null && j < values.Length)
                    {
                        object value = Convert.ChangeType(values[j], property.PropertyType);
                        property.SetValue(obj, value);
                    }
                }

                list.Add(obj);
            }
        }
        return list;
    }
    public async Task PrintData<T>(List<T> data)
    {
        var csv = new StringBuilder();
        var properties = typeof(T).GetProperties();
        csv.AppendLine(string.Join(";", properties.Select(p => p.Name)));

        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
            csv.AppendLine(string.Join(";", values));
        }
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));
        var fileSaverResult = await FileSaver.Default.SaveAsync("Collection.csv", stream);

        if (fileSaverResult.IsSuccessful)
        {
        }
        else
        {
        }
    }
}
