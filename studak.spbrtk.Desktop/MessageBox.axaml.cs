using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace studak.spbrtk.Desktop;

public partial class MessageBox : Window
{
    private readonly string _message;
    public MessageBox()
    {
        InitializeComponent();
    }
    
    public MessageBox(string message = "Ошибка!")
    {
        _message = message;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        ErrorMessageTextBlock = this.Find<TextBlock>("ErrorMessageTextBlock");
        ErrorMessageTextBlock.Text = _message;
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}