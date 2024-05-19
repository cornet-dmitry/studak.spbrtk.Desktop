using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using studak.spbrtk.Desktop.Models;

namespace studak.spbrtk.Desktop.Pages;

public partial class AddUserPage : UserControl
{
    private readonly string apiUrl = ApplicationState.GetValue<string>("apiUrl");
    
    private ProgressBar _loader;

    private readonly HttpClient _httpClient;
    
    public AddUserPage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();
        
        ActivistsNavBtn.FontWeight = FontWeight.ExtraBold;
        
        LoadComboBox();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        FIOTextBlock = this.Find<TextBlock>("FIOTextBlock");
        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        DocsNavBtn = this.Find<Button>("DocsNavBtn");
        BackArrowBtn = this.Find<Button>("BackArrowBtn");
        CreateBtn = this.Find<Button>("CreateBtn");

        SurnameTextBox = this.Find<TextBox>("SurnameTextBox");
        NameTextBox = this.Find<TextBox>("NameTextBox");
        PatrTextBox = this.Find<TextBox>("PatrTextBox");
        GroupTextBox = this.Find<TextBox>("GroupTextBox");
        DatePicker = this.Find<DatePicker>("DatePicker");
        PhoneTextBox = this.Find<MaskedTextBox>("PhoneTextBox");
        EmailTextBox = this.Find<TextBox>("EmailTextBox");
        VKTextBox = this.Find<TextBox>("VKTextBox");
        TGTextBox = this.Find<TextBox>("TGTextBox");
        StatusComboBox = this.Find<ComboBox>("StatusComboBox");
        
        _loader = this.Find<ProgressBar>("Loader");
    }

    private async void LoadComboBox()
    {
        _loader.IsVisible = true;
        try
        {
            var responce = await _httpClient.GetAsync(apiUrl + "/UserStatus/GetUserStatus");
        
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                //преобразование json в строку
                var responseValue = responce.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<UserStatus>>(responseValue);
                
                foreach (var VARIABLE in list)
                {
                    StatusComboBox.Items.Add(VARIABLE.StatusName);
                }
                
                StatusComboBox.SelectedIndex = 0;
            }
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
        }

        _loader.IsVisible = false;
    }
    
    private async void CreateBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _loader.IsVisible = true;
        if (CheckData())
        {
            try
            {
                var userData = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "surname", SurnameTextBox.Text},
                    { "name", NameTextBox.Text},
                    { "patronymic", PatrTextBox.Text},
                    { "group", GroupTextBox.Text },
                    { "dateBirth", DatePicker.SelectedDate.ToString() },
                    { "phone", PhoneTextBox.Text },
                    { "email", EmailTextBox.Text },
                    { "vkLink", VKTextBox.Text },
                    { "tgLink", TGTextBox.Text },
                    { "kpi", "0" },
                    { "status", (StatusComboBox.SelectedIndex + 1).ToString() },
                    { "orderNumber", "0" },
                    { "startDate", DateTime.Now.ToString() },
                });
            
                var response = await _httpClient.PostAsync($"{this.apiUrl}/User/AddUser", userData);
            
                Navigation.NavigateTo(new ActivistsPage());
            }
            catch (Exception exception)
            {
                MessageBox messageBox = new MessageBox(exception.Message);
                messageBox.Show();
            }
        }

        _loader.IsVisible = false;
    }
    
    private bool CheckData()
    {
        try
        {
            if (!(SurnameTextBox.Text.Length > 0 && NameTextBox.Text.Length > 0 && PatrTextBox.Text.Length > 0
                                               && DatePicker.SelectedDate.ToString().Length > 0 
                                               && GroupTextBox.Text.Length > 0 
                                               && PhoneTextBox.Text.Length > 0 && EmailTextBox.Text.Length > 0 
                                               && VKTextBox.Text.Length > 0 && TGTextBox.Text.Length > 0 ))
            {
                MessageBox messageBox = new MessageBox("Заполните все поля!");
                messageBox.Show();
                return false;
            }
            
            return true;
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox("Вы не зеполнили корректно все поля, либо на сервере произошла ошибка!");
            messageBox.Show();
            return false;
        }
        
    }

    private void BackArrowBtn_OnClick(object? sender, RoutedEventArgs e)
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