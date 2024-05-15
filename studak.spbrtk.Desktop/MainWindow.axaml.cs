using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using studak.spbrtk.Desktop.Pages;

namespace studak.spbrtk.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent = this.FindControl<ContentControl>("MainContent");
        Navigation.Initialize(MainContent);
        MainContent.Content = new EventsPage();
        
        ApplicationState.SetValue("apiUrl", "http://localhost:5209/api");
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}