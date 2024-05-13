using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace studak.spbrtk.Desktop.Pages;

public partial class WelcomePage : UserControl
{
    public WelcomePage()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}