using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<StrangeAnimal> MyObservableList { get; } = [];
    JSONServices MyJSONService;
    CSVServices MyCSVServices;

    public MainViewModel(JSONServices MyJSONService, CSVServices MyCSVServices)
    {
        this.MyJSONService = MyJSONService;
        this.MyCSVServices = MyCSVServices;
    }
 
    [RelayCommand]
    internal async Task GoToDetails(string id)
    {
        IsBusy = true;

        await Shell.Current.GoToAsync("DetailsView", true, new Dictionary<string,object>
        {
            {"selectedAnimal",id}
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

        await MyCSVServices.PrintData(Globals.MyStrangeAnimals);

        IsBusy = false;
    }
    [RelayCommand]
    internal async Task LoadFromCSV()
    {
        IsBusy = true;

        Globals.MyStrangeAnimals = await MyCSVServices.LoadData();

        IsBusy = false;
    }
    [RelayCommand]
    internal async Task UploadJSON()
    {
        IsBusy = true;

        await MyJSONService.SetStrangeAnimals(Globals.MyStrangeAnimals);

        IsBusy = false;
    }
    
    internal async Task RefreshPage()
    {
        MyObservableList.Clear ();

        if(Globals.MyStrangeAnimals.Count == 0) Globals.MyStrangeAnimals = await MyJSONService.GetStrangeAnimals();

        foreach (var item in Globals.MyStrangeAnimals)
        {
            MyObservableList.Add(item);
        }
    }
}
