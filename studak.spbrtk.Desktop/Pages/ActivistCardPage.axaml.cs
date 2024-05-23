using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using studak.spbrtk.Desktop.Models;

namespace studak.spbrtk.Desktop.Pages;

public partial class ActivistCardPage : UserControl
{
    private readonly string apiUrl = "http://localhost:5209/api";

    private readonly HttpClient _httpClient;
    private ProgressBar _loader;
    
    private Activists _activistsList;
    
    public ActivistCardPage()
    {
        InitializeComponent();
    }
    
    public ActivistCardPage(Activists activistsList)
    {
        _httpClient = new HttpClient();
        this._activistsList = activistsList;
        InitializeComponent();
        
        ActivistsNavBtn.FontWeight = FontWeight.ExtraBold;
        
        LoadData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        FIOTextBlock = this.Find<TextBlock>("FIOTextBlock");
        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        //DocsNavBtn = this.Find<Button>("DocsNavBtn");
        BackArrowBtn = this.Find<Button>("BackArrowBtn");

        KPITextBlock = this.Find<TextBlock>("KPITextBlock");
        GroupTextBlock = this.Find<TextBlock>("GroupTextBlock");
        StatusTextBlock = this.Find<TextBlock>("StatusTextBlock");
        BirthdayTextBlock = this.Find<TextBlock>("BirthdayTextBlock");
        PhoneTextBlock = this.Find<TextBlock>("PhoneTextBlock");
        EmailTextBlock = this.Find<TextBlock>("EmailTextBlock"); 
        VkTextBlock = this.Find<TextBlock>("VkTextBlock"); 
        TgTextBLock = this.Find<TextBlock>("TgTextBLock");
        
        VkButton = this.Find<Button>("VkButton");
        TgButton = this.Find<Button>("TgButton");
        _loader = this.Find<ProgressBar>("Loader");
    }

    private async void LoadData()
    {
        _loader.IsVisible = true;
        try
        {
            FIOTextBlock.Text = _activistsList.Surname + " " + _activistsList.Name + " " + _activistsList.Patronymic;
            KPITextBlock.Text = _activistsList.Kpi + " б";
            GroupTextBlock.Text = _activistsList.Group;
            StatusTextBlock.Text = _activistsList.Status;
            BirthdayTextBlock.Text = _activistsList.DateBirth.ToString().Split(new char[] {' '})[0];
            PhoneTextBlock.Text = _activistsList.Phone;
            EmailTextBlock.Text = _activistsList.Email;
            VkTextBlock.Text = _activistsList.VkLink;
            TgTextBLock.Text = _activistsList.TgLink;
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
        }

        _loader.IsVisible = false;
    }

    private void BackArrowBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new ActivistsPage());
    }

    private void VkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(VkTextBlock.Text)
                { UseShellExecute = true });
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
        }
    }
    
    private void TgButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo("https://t.me/" + TgTextBLock.Text.Split(new char[] {'@'})[1])
                { UseShellExecute = true });
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
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