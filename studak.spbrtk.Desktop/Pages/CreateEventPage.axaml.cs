using System;
using System.Collections.Generic;
using System.Linq;
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

public partial class CreateEventPage : UserControl
{
    private readonly string apiUrl = "http://localhost:5209/api";
    
    private ProgressBar _loader;

    private readonly HttpClient _httpClient;
    public CreateEventPage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();

        EventsNavBtn.FontWeight = FontWeight.ExtraBold;
        
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

        TitleTextBox = this.Find<TextBox>("TitleTextBox");
        PlaceTextBox = this.Find<TextBox>("PlaceTextBox");
        DatePicker = this.Find<DatePicker>("DatePicker");
        TimePicker = this.Find<TimePicker>("TimePicker");
        RateTextBox = this.Find<TextBox>("RateTextBox");
        DescriptionTextBox = this.Find<TextBox>("DescriptionTextBox");
        OrgComboBox = this.Find<ComboBox>("OrgComboBox");
        
        _loader = this.Find<ProgressBar>("Loader");
    }

    private async void LoadComboBox()
    {
        _loader.IsVisible = true;
        try
        {
            var responce = await _httpClient.GetAsync(apiUrl + "/User/GetManagers");
        
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                //преобразование json в строку
                var responseValue = responce.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<Activists>>(responseValue);
                
                OrgComboBox.Items.Add("Выберите");
                foreach (var VARIABLE in list)
                {
                    string managerName = $"{VARIABLE.Surname} {VARIABLE.Name[0]}. {VARIABLE.Patronymic[0]}.";
                    OrgComboBox.Items.Add(managerName);
                }

                OrgComboBox.SelectedIndex = 0;
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
                var eventData = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "responsible", OrgComboBox.SelectedIndex.ToString()},
                    { "direction", "1"},
                    { "name", TitleTextBox.Text},
                    { "description", DescriptionTextBox.Text },
                    { "place", PlaceTextBox.Text },
                    { "startDate", DatePicker.SelectedDate.ToString() },
                    { "endDate", DatePicker.SelectedDate.ToString()},
                    { "startTime", TimePicker.SelectedTime.ToString() },
                    { "endTime", TimePicker.SelectedTime.ToString() },
                    { "rate", RateTextBox.Text },
                    { "isActive", "true" },
                });
            
                var response = await _httpClient.PostAsync($"{this.apiUrl}/Event/AddEvent", eventData);
            
                Navigation.NavigateTo(new EventsPage());
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
            if (!(PlaceTextBox.Text.Length > 0 && TitleTextBox.Text.Length > 0
                  && DatePicker.SelectedDate.ToString().Length > 0
                  && TimePicker.SelectedTime.ToString().Length > 0
                  && RateTextBox.Text.Length > 0 && DescriptionTextBox.Text.Length > 0
                  && OrgComboBox.SelectedIndex > 0))
            {
                MessageBox messageBox = new MessageBox("Заполните все поля!");
                messageBox.Show();
                return false;
            }
            
            bool containsOnlyDigits = RateTextBox.Text.All(char.IsDigit);
            if (!containsOnlyDigits)
            {
                MessageBox messageBox = new MessageBox("Строка БАЛЛЫ может содержать только целые числа!");
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
    
    private void ActivistsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new ActivistsPage());
    }

    private void BackArrowBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventsPage());
    }
}