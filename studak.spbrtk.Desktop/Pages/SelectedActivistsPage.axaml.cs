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
using studak.spbrtk.API.Models;
using studak.spbrtk.Desktop.Models;



namespace studak.spbrtk.Desktop.Pages;

public partial class SelectedActivistsPage : UserControl
{
    private readonly string apiUrl = ApplicationState.GetValue<string>("apiUrl");
    
    private readonly HttpClient _httpClient;
    private ProgressBar _loader;

    private static List<SelectedActivists> _selectedActivistsList;

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

        _selectedActivistsList = new List<SelectedActivists>();
        
        LoadListBox();
        LoadData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        //SearchTextBox = this.Find<TextBox>("SearchTextBox");
        //NotificationButton = this.Find<Button>("NotificationButton");

        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        //DocsNavBtn = this.Find<Button>("DocsNavBtn");
        
        AddListUsers = this.Find<Button>("AddListUsers");
        UsersComboBox = this.Find<ComboBox>("UsersComboBox");
        
        /*UserNameTextBlock = this.Find<TextBlock>("UserNameTextBlock");
        UserStatusTextBlock = this.Find<TextBlock>("UserStatusTextBlock");*/
        
        SelectedUsersListBox = this.Find<ListBox>("SelectedUsersListBox");
        
        /*ActivistsListBox = this.Find<ListBox>("ActivistsListBox");
        SelectedTextBlock = this.Find<TextBlock>("SelectedTextBlock");*/

        _loader = this.Find<ProgressBar>("Loader");
    }

    private async void LoadListBox()
    {
        _selectedActivistsList.Clear();
        
        var userData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "id", _event.Id.ToString() }
        });
            
        var involvementResponse = await _httpClient.PostAsync(
            $"{this.apiUrl}/Involvement/GetInvolvementByEventId", userData);
        var involvementContent = await involvementResponse.Content.ReadAsStringAsync();
        var involvementList = JsonConvert.DeserializeObject<List<Involvement>>(involvementContent);

        foreach (var VARIABLE in involvementList)
        {
            var involvementUserData = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "userId", VARIABLE.Userid.ToString() }
            });
            
            var userResponse = await _httpClient.PostAsync($"{this.apiUrl}/User/GetUserByID",
                involvementUserData);
            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userList = JsonConvert.DeserializeObject<List<Activists>>(userContent);
                
            _selectedActivistsList.Add(new SelectedActivists()
            {
                Id = userList[0].Id,
                FullName = $"{userList[0].Surname} {userList[0].Name} {userList[0].Patronymic}"
            });
        }
            
        SelectedUsersListBox.ItemsSource = _selectedActivistsList;
    }

    private async void LoadData()
    {
        try
        {
            UsersComboBox.Items.Add("ФИО активиста");
            UsersComboBox.SelectedIndex = 0;
            
            //запрос к API
            var responseManagers = await _httpClient.GetAsync($"{this.apiUrl}/User/GetManagers");

            if (responseManagers.StatusCode == HttpStatusCode.OK)
            {
                //преобразование json в строку
                var responseValue = responseManagers.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<Activists>>(responseValue);
                
                foreach (var VARIABLE in list)
                {
                    string FIO = $"{VARIABLE.Id} - {VARIABLE.Surname} {VARIABLE.Name} {VARIABLE.Patronymic}";
                    UsersComboBox.Items.Add(FIO);
                }
            }
            
            //запрос к API
            var responseActivists = await _httpClient.GetAsync($"{this.apiUrl}/User/GetUsers");
            if (responseActivists.StatusCode == HttpStatusCode.OK)
            {
                //преобразование json в строку
                var responseValue = responseActivists.Content.ReadAsStringAsync().Result;
                var list = JsonConvert.DeserializeObject<List<Activists>>(responseValue);

                foreach (var VARIABLE in list)
                {
                    string FIO = $"{VARIABLE.Id} - {VARIABLE.Surname} {VARIABLE.Name} {VARIABLE.Patronymic}";
                    UsersComboBox.Items.Add(FIO);
                }
            }
            
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
        }
    }
    
    private async void AddListUsers_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (UsersComboBox.SelectedIndex != 0)
            {
                try
                {
                    var userData = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "eventID", _event.Id.ToString() },
                        { "userID", UsersComboBox.SelectedValue.ToString().Split(new []{'-'}, 
                            StringSplitOptions.RemoveEmptyEntries)[0] }
                    });
            
                    var response = await _httpClient.PostAsync($"{this.apiUrl}/Involvement/AddInvolvement", userData);
                    
                    Navigation.NavigateTo(new SelectedActivistsPage(_event));
                }
                catch (Exception exception)
                {
                    MessageBox messageBox = new MessageBox(exception.Message);
                    messageBox.Show();
                }
            }
            else
            {
                MessageBox messageBox = new MessageBox("Выберите активиста из списка");
                messageBox.Show();
            }

            SelectedUsersListBox.ItemsSource = _selectedActivistsList;
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
                var user = button.DataContext as SelectedActivists;
                if (user != null)
                {
                    /*var userData = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "eventid", _event.Id.ToString() },
                        { "userid", user.Id.ToString() }
                    });*/

                    var response =
                        await _httpClient.DeleteAsync($"{this.apiUrl}/Involvement/DeleteInvolvement/{_event.Id}&{user.Id}");

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Navigation.NavigateTo(new SelectedActivistsPage(_event));
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

    private void EventsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventsPage());
    }

    private void CreateEventBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new AddUserPage());
    }

    private void KpiNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new RatePage());
    }

    private void BackArrowBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventCardPage(_event));
    }
}