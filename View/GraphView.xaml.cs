namespace MyApp.View;

public partial class GraphView : ContentPage
{
    readonly GraphViewModel viewModel;
    public GraphView(GraphViewModel viewModel)
	{
        this.viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        BindingContext = null;
        viewModel.RefreshPage();    
        BindingContext = viewModel;
    }
}