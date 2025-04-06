using CommunityToolkit.Mvvm.Input;
using Microcharts;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class GraphViewModel : ObservableObject
{
    [ObservableProperty]
    public string title = "Analyse de la bibliothèque";

    [ObservableProperty]
    public Chart myObservableChart;

    private List<ChartEntry> GenerateBooksByCategoryEntries()
    {
        var entries = new List<ChartEntry>();

        // Regrouper les livres par catégorie et compter
        var groupedBooks = Globals.MyBooks
            .GroupBy(b => b.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(10) // Prendre les 10 premières catégories
            .ToList();

        // Palette de couleurs pour le graphique
        var colors = new[]
        {
            SKColor.Parse("#266489"),
            SKColor.Parse("#68B9C0"),
            SKColor.Parse("#90D585"),
            SKColor.Parse("#F3C151"),
            SKColor.Parse("#F37F64"),
            SKColor.Parse("#D8383A"),
            SKColor.Parse("#8465A7"),
            SKColor.Parse("#649FF2"),
            SKColor.Parse("#DFBFBF"),
            SKColor.Parse("#CCCCCC")
        };

        // Créer les entrées pour le graphique
        for (int i = 0; i < groupedBooks.Count; i++)
        {
            var group = groupedBooks[i];
            entries.Add(new ChartEntry(group.Count)
            {
                Label = group.Category,
                ValueLabel = group.Count.ToString(),
                Color = colors[i % colors.Length], // Utiliser les couleurs de manière cyclique
                TextColor = colors[i % colors.Length]
            });
        }

        return entries;
    }

    private List<ChartEntry> GenerateBooksByLanguageEntries()
    {
        var entries = new List<ChartEntry>();

        // Regrouper les livres par langue et compter
        var groupedBooks = Globals.MyBooks
            .GroupBy(b => b.Language)
            .Select(g => new { Language = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(10) // Prendre les 10 premières langues
            .ToList();

        // Palette de couleurs pour le graphique
        var colors = new[]
        {
            SKColor.Parse("#266489"),
            SKColor.Parse("#68B9C0"),
            SKColor.Parse("#90D585"),
            SKColor.Parse("#F3C151"),
            SKColor.Parse("#F37F64"),
            SKColor.Parse("#D8383A"),
            SKColor.Parse("#8465A7"),
            SKColor.Parse("#649FF2"),
            SKColor.Parse("#DFBFBF"),
            SKColor.Parse("#CCCCCC")
        };

        // Créer les entrées pour le graphique
        for (int i = 0; i < groupedBooks.Count; i++)
        {
            var group = groupedBooks[i];
            entries.Add(new ChartEntry(group.Count)
            {
                Label = string.IsNullOrEmpty(group.Language) ? "Non spécifié" : group.Language,
                ValueLabel = group.Count.ToString(),
                Color = colors[i % colors.Length],
                TextColor = colors[i % colors.Length]
            });
        }

        return entries;
    }

    private List<ChartEntry> GenerateBooksByPriceEntries()
    {
        var entries = new List<ChartEntry>();

        // Définir des tranches de prix
        var priceBrackets = new[]
        {
            new { Label = "0-5€", Min = 0m, Max = 5m, Color = SKColor.Parse("#266489") },
            new { Label = "5-10€", Min = 5m, Max = 10m, Color = SKColor.Parse("#68B9C0") },
            new { Label = "10-15€", Min = 10m, Max = 15m, Color = SKColor.Parse("#90D585") },
            new { Label = "15-20€", Min = 15m, Max = 20m, Color = SKColor.Parse("#F3C151") },
            new { Label = "20-30€", Min = 20m, Max = 30m, Color = SKColor.Parse("#F37F64") },
            new { Label = ">30€", Min = 30m, Max = decimal.MaxValue, Color = SKColor.Parse("#D8383A") }
        };

        // Compter les livres dans chaque tranche de prix
        foreach (var bracket in priceBrackets)
        {
            var count = Globals.MyBooks.Count(b => b.Price >= bracket.Min && b.Price < bracket.Max);

            entries.Add(new ChartEntry(count)
            {
                Label = bracket.Label,
                ValueLabel = count.ToString(),
                Color = bracket.Color,
                TextColor = bracket.Color
            });
        }

        return entries;
    }

    private List<ChartEntry> GenerateBooksByPublicationYearEntries()
    {
        var entries = new List<ChartEntry>();

        // Extraire les années et regrouper
        var currentYear = DateTime.Now.Year;
        var startYear = currentYear - 10; // 10 dernières années

        var yearGroups = new Dictionary<int, int>();
        for (int year = startYear; year <= currentYear; year++)
        {
            yearGroups[year] = 0;
        }

        // Compter les livres par année
        foreach (var book in Globals.MyBooks)
        {
            int year = book.PublicationDate.Year;
            if (year >= startYear && year <= currentYear)
            {
                yearGroups[year]++;
            }
        }

        // Définir une palette de couleurs avec dégradé
        var baseColor = SKColor.Parse("#266489");
        float hue = baseColor.Hue;

        // Créer les entrées pour le graphique
        int index = 0;
        foreach (var year in yearGroups.Keys.OrderBy(y => y))
        {
            var count = yearGroups[year];

            // Calculer une couleur dans le dégradé
            float saturation = 0.7f + (0.3f * index / yearGroups.Count);
            var color = SKColor.FromHsl(hue, saturation, 0.5f);

            entries.Add(new ChartEntry(count)
            {
                Label = year.ToString(),
                ValueLabel = count.ToString(),
                Color = color,
                TextColor = color
            });

            index++;
        }

        return entries;
    }

    public GraphViewModel()
    {
        // Initialiser avec un graphique vide
        MyObservableChart = new BarChart { Entries = new ChartEntry[0] };
    }

    internal void RefreshPage()
    {
        if (Globals.MyBooks.Count == 0)
        {
            // Pas de données à afficher
            MyObservableChart = new BarChart
            {
                Entries = new[]
                {
                    new ChartEntry(0)
                    {
                        Label = "Aucune donnée",
                        ValueLabel = "0",
                        Color = SKColor.Parse("#CCCCCC")
                    }
                },
                LabelTextSize = 40
            };
            return;
        }

        // Graphique par défaut: livres par catégorie
        SwitchToCategoryChart();
    }

    [RelayCommand]
    internal void SwitchToCategoryChart()
    {
        Title = "Livres par catégorie";
        MyObservableChart = new BarChart
        {
            Entries = GenerateBooksByCategoryEntries().ToArray(),
            LabelTextSize = 40,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            BackgroundColor = SKColors.Transparent
        };
    }

    [RelayCommand]
    internal void SwitchToLanguageChart()
    {
        Title = "Livres par langue";
        MyObservableChart = new DonutChart
        {
            Entries = GenerateBooksByLanguageEntries().ToArray(),
            LabelTextSize = 40,
            BackgroundColor = SKColors.Transparent
        };
    }

    [RelayCommand]
    internal void SwitchToPriceChart()
    {
        Title = "Distribution des livres par prix";
        MyObservableChart = new BarChart
        {
            Entries = GenerateBooksByPriceEntries().ToArray(),
            LabelTextSize = 40,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            BackgroundColor = SKColors.Transparent
        };
    }

    [RelayCommand]
    internal void SwitchToYearChart()
    {
        Title = "Publication par année";
        MyObservableChart = new LineChart
        {
            Entries = GenerateBooksByPublicationYearEntries().ToArray(),
            LabelTextSize = 40,
            LineSize = 8,
            PointSize = 18,
            BackgroundColor = SKColors.Transparent
        };
    }
}