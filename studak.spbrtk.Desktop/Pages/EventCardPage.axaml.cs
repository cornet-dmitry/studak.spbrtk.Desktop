using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;
using studak.spbrtk.API.Models;
using studak.spbrtk.Desktop.Models;

namespace studak.spbrtk.Desktop.Pages;

public partial class EventCardPage : UserControl
{
    private readonly string apiUrl = ApplicationState.GetValue<string>("apiUrl");

    private readonly HttpClient _httpClient;
    
    private ProgressBar _loader;
    private Event _event;
    
    public EventCardPage()
    {
        InitializeComponent();
    }
    
    public EventCardPage(Event events)
    {
        _httpClient = new HttpClient();
        
        this._event = events;
        InitializeComponent();
        
        EventsNavBtn.FontWeight = FontWeight.ExtraBold;
        
        LoadData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        DocsNavBtn = this.Find<Button>("DocsNavBtn");
        
        OrderBtn = this.Find<Button>("OrderBtn");
        AddUserBtn = this.Find<Button>("AddUserBtn");
        _loader = this.Find<ProgressBar>("Loader");

        TitleTextBlock = this.Find<TextBlock>("TitleTextBlock");
        PlaceTextBlock = this.Find<TextBlock>("PlaceTextBlock");
        DateTextBlock = this.Find<TextBlock>("DateTextBlock");
        RateTextBlock = this.Find<TextBlock>("RateTextBlock");
        DescriptionTextBlock = this.Find<TextBlock>("DescriptionTextBlock");
        ResponsibleTextBlock = this.Find<TextBlock>("ResponsibleTextBlock");
        EventStatusTextBlock = this.Find<TextBlock>("EventStatusTextBlock");
        ActivistsCountTextBlock = this.Find<TextBlock>("ActivistsCountTextBlock");

        InvolvementListBox = this.Find<ListBox>("InvolvementListBox");
        EventOpenCloseImage = this.Find<Image>("EventOpenCloseImage");
        OrderCreateImage = this.Find<Image>("OrderCreateImage");
    }

    private async void LoadData()
    {
        _loader.IsVisible = true;
        try
        {
            TitleTextBlock.Text = _event.Name;
            PlaceTextBlock.Text = _event.Place;
            DateTextBlock.Text = _event.FullDateTime;
            RateTextBlock.Text = _event.Rate.ToString();
            DescriptionTextBlock.Text = _event.Description;
            ResponsibleTextBlock.Text = _event.Responsible;

            if (_event.Isactice is true)
            {
                EventStatusTextBlock.Text = "ИДЁТ НАБОР УЧАСТНИКОВ";
                EventStatusTextBlock.Foreground = new SolidColorBrush(Color.Parse("#00AA07"));
                
                OrderBtn.IsVisible = false;
                AddUserBtn.IsVisible = true;
            }
            else
            {
                EventStatusTextBlock.Text = "НАБОР ЗАКРЫТ";
                EventStatusTextBlock.Foreground = new SolidColorBrush(Color.Parse("#AA0000"));
                
                OrderBtn.IsVisible = true;
                AddUserBtn.IsVisible = false;
            }
            
            //Блок загрузки таблицы участников
            var involvementData = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"id", _event.Id.ToString()}
            });

            var responceInvolvement =
                await _httpClient.PostAsync(apiUrl + "/Involvement/GetInvolvementByEventId", involvementData);
            
            var involvementContent = await responceInvolvement.Content.ReadAsStringAsync();
            var involvementList = JsonConvert.DeserializeObject<List<Involvement>>(involvementContent);

            if (involvementList.Count > 0)
            {
                List<Activists> activistsList = new List<Activists>();
                
                foreach (var VARIABLE in involvementList)
                {
                    var userData = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "userId", VARIABLE.Userid.ToString() }
                    });

                    var responceUsers =
                        await _httpClient.PostAsync(apiUrl + "/User/GetUserByID", userData);

                    var userContent = await responceUsers.Content.ReadAsStringAsync();
                    var usertList = JsonConvert.DeserializeObject<List<Activists>>(userContent);
                    
                    activistsList.Add(usertList[0]);
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
                
                ActivistsCountTextBlock.Text = $"Участники ({activistsList.Count})";
                InvolvementListBox.ItemsSource = activistsList;
            }
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
        }
        
        _loader.IsVisible = false;
    }

    private async void EventOpenCloseBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var eventData = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "name", _event.Name },
                { "description", _event.Description },
                { "place", _event.Place },
                { "startDate", _event.StartDate.ToString() },
                { "endDate", _event.EndDate.ToString() },
                { "startTime", _event.StartTime.ToString() },
                { "endTime", _event.EndTime.ToString() },
                { "rate", _event.Rate.ToString() },
                { "isActive", (!_event.Isactice).ToString() },
            });
            
            /*ЕСТЬ БАГ: ПРИ ПОПЫТКИ ОБНОВИТЬ ЗАПИСЬ С ПРИВЯЗАННЫМИ УЧАСТНИКАМИ НИЧЕГО НЕ ПРОИСХОДИТ*/
            
            var response = await _httpClient.PostAsync($"{this.apiUrl}/Event/EditEvent/{_event.Id}", eventData);
            
            Navigation.NavigateTo(new EventsPage());
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
        }
    }
    
    private async void DeleteEventBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Подтверждение", "Вы уверены, что хотите удалить данное событие?",
                    ButtonEnum.YesNo);

            var result = await box.ShowAsync();
            
            if (result == ButtonResult.Yes)
            {
                var response = await _httpClient.DeleteAsync($"{this.apiUrl}/Event/DeleteEvent/{_event.Id}");
                Navigation.NavigateTo(new EventsPage());
            }
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
        }
    }
    
    private void InvolvementListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItems != null)
            {
                var selectedActivists = (e.AddedItems[0] as Activists);
                Navigation.NavigateTo(new ActivistCardPage(selectedActivists));
            }
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
        }
    }
    
    private void AddUserBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new SelectedActivistsPage(_event));
    }
    
    private void ActivistsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new ActivistsPage());
    }

    private void EventsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventsPage());
    }

    private void BackArrowBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventsPage());
    }
}