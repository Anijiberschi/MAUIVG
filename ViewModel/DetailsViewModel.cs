using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

[QueryProperty(nameof(Id), "selectedAnimal")]
public partial class DetailsViewModel: ObservableObject
{
    [ObservableProperty]
    public partial string? Id { get; set; }
    [ObservableProperty]
    public partial string? Name { get; set; }
    [ObservableProperty]
    public partial string? Description { get; set; }
    [ObservableProperty]
    public partial string? Picture { get; set; }
    [ObservableProperty]
    public partial string? SerialBufferContent { get; set; }
    [ObservableProperty]
    public partial bool EmulatorON_OFF { get; set; } = false;
    
    readonly DeviceOrientationService MyScanner;

    IDispatcherTimer emulator= Application.Current.Dispatcher.CreateTimer();

    public DetailsViewModel(DeviceOrientationService myScanner)
    {
        this.MyScanner = myScanner;
        MyScanner.OpenPort();
        myScanner.SerialBuffer.Changed += OnSerialDataReception;

        emulator.Interval = TimeSpan.FromSeconds(1);
        emulator.Tick += (s, e) => AddCode();
    }
    partial void OnEmulatorON_OFFChanged(bool value)
    {
        if (value) emulator.Start();
        else emulator.Stop();
    }
    private void AddCode()
    {
        MyScanner.SerialBuffer.Enqueue("B");
    }
    private void OnSerialDataReception(object sender, EventArgs arg)
    {
        DeviceOrientationService.QueueBuffer MyLocalBuffer = (DeviceOrientationService.QueueBuffer)sender;

        if (MyLocalBuffer.Count > 0)
        {
            SerialBufferContent += MyLocalBuffer.Dequeue().ToString();
            OnPropertyChanged(nameof(SerialBufferContent));
        }
    }
    internal void RefreshPage()
    {
        foreach (var item in Globals.MyStrangeAnimals)
        {
            if (Id == item.Id)
            {
                Name = item.Name;
                Description = item.Description;
                Picture = item.Picture;

                break;
            }
        }
    }
    internal void ClosePage()
    {
        MyScanner.SerialBuffer.Changed -= OnSerialDataReception;
        MyScanner.ClosePort();       
    }

    [RelayCommand]
    internal void ChangeObjectParameters()
    {
        foreach (var item in Globals.MyStrangeAnimals)
        {
            if (item.Id == Id)
            {
                item.Name = Name ?? string.Empty;
                item.Description = Description ?? string.Empty;
                item.Picture = Picture ?? string.Empty;
            }
        }        
    }    
}
