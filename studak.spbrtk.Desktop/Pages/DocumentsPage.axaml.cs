using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;


namespace studak.spbrtk.Desktop.Pages;

public partial class DocumentsPage : UserControl
{
    private static string relativePath = "Documents";
    private static string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
    public DocumentsPage()
    {
        InitializeComponent();
        
        DocsNavBtn.FontWeight = FontWeight.ExtraBold;
        
        LoadData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        DocsNavBtn = this.Find<Button>("DocsNavBtn");

        DocumentsListBox = this.Find<ListBox>("DocumentsListBox");
    }

    private void LoadData()
    {
        List<string> filesList = new List<string>();
        List<string> filesNameList = new List<string>();
        
        // Получение всех файлов в директории
        string[] files = Directory.GetFiles(absolutePath);
        
        foreach (var file in files)
        {
            filesList.Add(file);
            filesNameList.Add(Path.GetFileName(file));
        }

        DocumentsListBox.ItemsSource = filesNameList;
    }
    
    private void DocumentsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItems != null)
            {
                var selectedDocs = (e.AddedItems[0] as string);
                
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(absolutePath, selectedDocs),
                    UseShellExecute = true // Использование ShellExecute для открытия файла с помощью связанного приложения
                };

                Process.Start(startInfo);
            }
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
        }
    }
    
    private async void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            if (button != null)
            {
                var doc = button.DataContext as string;
                if (doc != null)
                {
                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Подтверждение", 
                            $"Вы уверены, что хотите удалить файл {doc}?\u2028" +
                            "Отменить это действиет будет нельзя!",
                            ButtonEnum.YesNo);

                    var result = await box.ShowAsync();

                    if (result == ButtonResult.Yes)
                    {
                        File.Delete(Path.Combine(absolutePath, doc));
                        Navigation.NavigateTo(new DocumentsPage());
                    }
                }
            }
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
        }
    }
    
    private void PrintButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            if (button != null)
            {
                var doc = button.DataContext as string;
                if (doc != null)
                {
                    PrintDocument(Path.Combine(absolutePath, doc));
                }
            }
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
        }
    }
    
    static void PrintDocument(string path)
    {
        ProcessStartInfo psi = new ProcessStartInfo(path)
        {
            UseShellExecute = true,
            Verb = "print",
            RedirectStandardOutput = false,
            CreateNoWindow = true
        };

        using (Process p = new Process {StartInfo = psi})
        {
            p.Start();
            p.WaitForExit();
        }
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