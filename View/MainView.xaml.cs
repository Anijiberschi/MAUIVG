using CommunityToolkit.Maui.Views;

namespace MyApp.View;

public partial class MainView : ContentPage
{
	MainViewModel viewModel;
	public MainView(MainViewModel viewModel)
	{
		this.viewModel = viewModel;
		InitializeComponent();
		BindingContext=viewModel;
	}
	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
        base.OnNavigatedTo(args);

        BindingContext = null;
		await viewModel.RefreshPage();    
		BindingContext = viewModel;
	}
    public void DisplayPopup(object? sender, EventArgs e) 
    {
        var popup = new SimplePopup();

        this.ShowPopup(popup);
    }
}