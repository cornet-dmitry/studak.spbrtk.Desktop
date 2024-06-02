using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using studak.spbrtk.Desktop.Models;

namespace studak.spbrtk.Desktop.Pages;

public partial class RatePage : UserControl
{
    private readonly HttpClient _httpClient;
    private ProgressBar _loader;
    
    public RatePage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();
        
        KpiNavBtn.FontWeight = FontWeight.ExtraBold;
        
        LoadData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        SearchTextBox = this.Find<TextBox>("SearchTextBox");

        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn"); 
        DocsNavBtn = this.Find<Button>("DocsNavBtn");
        
        UserNameTextBlock = this.Find<TextBlock>("UserNameTextBlock");
        UserStatusTextBlock = this.Find<TextBlock>("UserStatusTextBlock");
        NowDayTextBlock = this.Find<TextBlock>("NowDayTextBlock");

        ActivistsListBox = this.Find<ListBox>("ActivistsListBox");
        _loader = this.Find<ProgressBar>("Loader");
    }

    private async void LoadData()
    {
        _loader.IsVisible = true;
        try
        {
            UserNameTextBlock.Text = ApplicationState.GetValue<string>("UserNameTextBlock");
            UserStatusTextBlock.Text = ApplicationState.GetValue<string>("UserStatusTextBlock");

            NowDayTextBlock.Text = $"на {DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}";
            
            List<Activists> activistsList = new List<Activists>();
            
            //запрос к API
            var responseManagers = await _httpClient.GetAsync(
                $"{ApplicationState.GetValue<string>("apiUrl")}/User/GetManagers");

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
            var responseActivists = await _httpClient.GetAsync(
                $"{ApplicationState.GetValue<string>("apiUrl")}/User/GetUsers");
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
            var statusResponse = await _httpClient.GetAsync(ApplicationState.GetValue<string>("apiUrl")
                                                            + "/UserStatus/GetUserStatus");
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
            
            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                string[] searchwords = SearchTextBox.Text.ToLower()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                activistsList = activistsList.Where(a => searchwords.All(word =>
                        (a.Name != null && a.Name.ToLower().Contains(word))
                        || (a.Surname != null && a.Surname.ToLower().Contains(word))
                        || (a.Patronymic != null && a.Patronymic.ToLower().Contains(word))
                        || (a.Email != null && a.Email.ToLower().Contains(word))
                        || (a.Phone != null && a.Phone.ToLower().Contains(word))
                        || (a.TgLink != null && a.TgLink.ToLower().Contains(word))
                        || (a.VkLink != null && a.VkLink.ToLower().Contains(word))
                        || (a.Status != null && a.Status.ToLower().Contains(word))
                        || (a.Group != null && a.Group.ToLower().Contains(word))))
                    .ToList();
            }
            
            UserNameTextBlock.Text = ApplicationState.GetValue<string>("UserNameTextBlock");
            UserStatusTextBlock.Text = ApplicationState.GetValue<string>("UserStatusTextBlock");
            
            ActivistsListBox.ItemsSource = activistsList.OrderByDescending(x => x.Kpi);
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
        }
        _loader.IsVisible = false;
    }

    private void EventsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventsPage());
    }

    private void SearchTextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        LoadData();
    }

    private void ActivistsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new ActivistsPage());
    }

    private void DocsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new DocumentsPage());
    }
}