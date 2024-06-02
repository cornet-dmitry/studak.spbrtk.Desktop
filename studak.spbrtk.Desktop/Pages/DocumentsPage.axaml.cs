using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace studak.spbrtk.Desktop.Pages;

public partial class DocumentsPage : UserControl
{
    public DocumentsPage()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        
        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        DocsNavBtn = this.Find<Button>("DocsNavBtn");
    }

    private void ActivistsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new ActivistsPage());
    }

    private void EventsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventsPage());
    }

    private void KpiNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new RatePage());
    }
}