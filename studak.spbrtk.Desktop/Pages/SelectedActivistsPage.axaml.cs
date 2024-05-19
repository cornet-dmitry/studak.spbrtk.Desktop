using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Newtonsoft.Json;
using studak.spbrtk.Desktop.Models;



namespace studak.spbrtk.Desktop.Pages;

public partial class SelectedActivistsPage : UserControl
{
    private readonly string apiUrl = "http://localhost:5209/api";
    
    private readonly HttpClient _httpClient;
    private ProgressBar _loader;

    private static List<Activists> _selectedActivistsList;

    private Event _event;
    public SelectedActivistsPage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();
    }

    public SelectedActivistsPage(Event events)
    {
        _event = events;
        _httpClient = new HttpClient();
        InitializeComponent();

        /*SearchTextBox.Background = Brushes.Transparent;
        SearchTextBox.BorderBrush = Brushes.Transparent;
        NotificationButton.Background = Brushes.Transparent;*/
        ActivistsNavBtn.FontWeight = FontWeight.ExtraBold;

        _selectedActivistsList = new List<Activists>();
        
        LoadData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        SearchTextBox = this.Find<TextBox>("SearchTextBox");
        //NotificationButton = this.Find<Button>("NotificationButton");

        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        DocsNavBtn = this.Find<Button>("DocsNavBtn");
        
        AddListUsers = this.Find<Button>("AddListUsers");
        ActivistsListBox = this.Find<ListBox>("ActivistsListBox");
        SelectedTextBlock = this.Find<TextBlock>("SelectedTextBlock");

        _loader = this.Find<ProgressBar>("Loader");
    }

    private async void LoadData()
    {
        _loader.IsVisible = true;
        try
        {
            List<Activists> activistsList = new List<Activists>();

            //запрос к API
            
            var responseManagers = await _httpClient.GetAsync($"{this.apiUrl}/User/GetManagers");

            if (responseManagers.StatusCode == HttpStatusCode.OK)
            {
                //преобразование json в строку
                var responseValue = responseManagers.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<Activists>>(responseValue);
                
                foreach (var VARIABLE in list)
                {
                    activistsList.Add(VARIABLE);
                }
            }
            
            //запрос к API
            var responseActivists = await _httpClient.GetAsync($"{this.apiUrl}/User/GetUsers");
            try
            {
                if (responseActivists.StatusCode == HttpStatusCode.OK)
                {
                    //преобразование json в строку
                    var responseValue = responseActivists.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<Activists>>(responseValue);

                    foreach (var VARIABLE in list)
                    {
                        activistsList.Add(VARIABLE);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //получение списка всех статусов
            var statusResponse = await _httpClient.GetAsync(this.apiUrl + "/UserStatus/GetUserStatus");
            var statusContent = await statusResponse.Content.ReadAsStringAsync();
            var statusList = JsonConvert.DeserializeObject<List<UserStatus>>(statusContent);
            
            //замена ID статуса на название
            foreach (var VARIABLE in activistsList)
            {
                var userStatus = statusList.FirstOrDefault(s => s.Id == Convert.ToInt32(VARIABLE.Status));
                if (userStatus != null)
                {
                    VARIABLE.Status = userStatus.StatusName;
                }
            }
            
            ActivistsListBox.ItemsSource = activistsList;
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
        }

        _loader.IsVisible = false;
    }
    
    private void ActivistsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItems != null)
            {
                var selectedActivists = (e.AddedItems[0] as Activists);

                bool tf = false;
                
                foreach (var VARIABLE in _selectedActivistsList)
                {
                    if (VARIABLE == selectedActivists)
                    {
                        tf = true;
                        _selectedActivistsList.Remove(selectedActivists);
                    }
                }

                if (!tf)
                {
                    _selectedActivistsList.Add(selectedActivists);
                }
            }
        
            SelectedTextBlock.Text = "";
            string actName = "";
            foreach (var VARIABLE in _selectedActivistsList)
            {
                actName += $"{_selectedActivistsList.Count + 1} - {VARIABLE.Surname} {VARIABLE.Name[0]}. {VARIABLE.Patronymic[0]}." + "\n";
            }
            SelectedTextBlock.Text = actName;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
        
    }
    
    private async void AddListUsers_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_selectedActivistsList.Count == 0)
        {
            MessageBox messageBox = new MessageBox("Ни один участник не выбран");
            messageBox.Show();
        }
        else
        {
            foreach (var VARIABLE in _selectedActivistsList)
            {
                //Console.WriteLine(VARIABLE.Id + " - " + VARIABLE.Surname);
                try
                {
                    var userData = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "eventID", _event.Id.ToString()},
                        { "userID", VARIABLE.Id.ToString()}
                    });
            
                    var response = await _httpClient.PostAsync($"{this.apiUrl}/Involvement/AddInvolvement", userData);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            
            Navigation.NavigateTo(new EventsPage());
        }
    }

    private void EventsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventsPage());
    }

    private void CreateEventBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new AddUserPage());
    }

    private void SearchTextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        throw new NotImplementedException();
    }
}