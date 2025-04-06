using Microcharts;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyApp.ViewModel;

public partial class GraphViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? AnimalName{ get; set; }

    [ObservableProperty]
    public partial Chart MyObservableChart { get; set; } = new RadarChart();

    ChartEntry[] entries = new[]
       {
           new ChartEntry(51)
           {
               Label = "S1",
               ValueLabel="51",
               Color = SKColor.Parse("#b455b6")
           },
           new ChartEntry(28)
           {
               Label = "S2",
               ValueLabel= "28",
               Color = SKColor.Parse("#b455b6")
           },
           new ChartEntry(35)
           {
               Label = "S3",
               ValueLabel="35",
               Color = SKColor.Parse("#b455b6")
           },
           new ChartEntry(47)
           {
               Label = "S4",
               ValueLabel="47",
               Color = SKColor.Parse("#C0c0c0")
           },
           new ChartEntry(64)
           {
               Label = "S5",
               ValueLabel="64",
               Color = SKColor.Parse("#b455b6")
           },
           new ChartEntry(57)
           {
               Label = "S6",
               ValueLabel="64",
               Color = SKColor.Parse("#b455b6")
           },
           new ChartEntry(22)
           {
               Label = "S7",
               ValueLabel="22",
               Color = SKColor.Parse("#b455b6")
           },
           new ChartEntry(24)
           {
               Label = "S8",
               ValueLabel="24",
               Color = SKColor.Parse("#b455b6")
           },
           new ChartEntry(48)
           {
               Label = "S9",
               ValueLabel="48",
               Color = SKColor.Parse("#b455b6")
           },
           new ChartEntry(87)
           {
               Label = "S10",
               ValueLabel="87",
               Color = SKColor.Parse("#b455b6")
           }
       };
    public GraphViewModel()
    {
        Chart myChart = new RadarChart
        {
            Entries = entries
        };

        MyObservableChart = myChart;
    }
    internal void RefreshPage()
    {
  
    }
}
