using Avalonia.Controls;

namespace studak.spbrtk.Desktop;

public class Navigation
{
    private static ContentControl MainContent;

    public static void Initialize(ContentControl control)
    {
        MainContent = control;
    }

    public static void NavigateTo(UserControl page)
    {
        MainContent.Content = page;
    }
}