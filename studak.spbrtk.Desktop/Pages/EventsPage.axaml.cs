using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using studak.spbrtk.API.Models;
using studak.spbrtk.Desktop.Models;

namespace studak.spbrtk.Desktop.Pages;

public partial class EventsPage : UserControl
{
    private readonly string apiUrl = "http://localhost:5209/api";
    
    private readonly HttpClient _httpClient;
    public EventsPage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();

        SearchTextBox.Background = Brushes.Transparent;
        SearchTextBox.BorderBrush = Brushes.Transparent;
        NotificationButton.Background = Brushes.Transparent;
        EventsNavBtn.FontWeight = FontWeight.ExtraBold;

        LoadData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        SearchTextBox = this.Find<TextBox>("SearchTextBox");
        NotificationButton = this.Find<Button>("NotificationButton");

        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        DocsNavBtn = this.Find<Button>("DocsNavBtn");
        CreateEventBtn = this.Find<Button>("CreateEventBtn");

        EventSortComboBox = this.Find<ComboBox>("EventSortComboBox");
        EventsListBox = this.Find<ListBox>("EventsListBox");

    }

    private async void LoadData()
    {
        try
        {
            //получение списка всех мероприятий
            var response = await _httpClient.GetAsync(this.apiUrl + "/Event/GetEvents");
            var content = await response.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<List<Event>>(content);
        
        
            //получение списка участников мероприятий
            var involvementResponse = await _httpClient.GetAsync(this.apiUrl + "/Involvement/GetInvolvement");
            var involvementContent = await involvementResponse.Content.ReadAsStringAsync();
            var involvementList = JsonConvert.DeserializeObject<List<Involvement>>(involvementContent);
        

            foreach (var VARIABLE in events)
            {
                VARIABLE.FullDateTime = VARIABLE.StartDate.ToString().Split(new []{' '})[0]
                                        + " " + VARIABLE.StartTime.ToString().Split(new []{' '})[1];

                VARIABLE.ActivistsNumber = involvementList.Where(x => x.Eventid == VARIABLE.Id).Select(x => x.Userid).Count();
            }

            EventsListBox.ItemsSource = events;
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
        }
    }
    
    private void EventsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox)sender;
        if (listBox.SelectedItems != null)
        {
            var selectedEvent = (e.AddedItems[0] as Event);
            Navigation.NavigateTo(new EventCardPage(selectedEvent));
        }
    }
    
    private void ActivistsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new ActivistsPage());
    }

    private void CreateEventBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new CreateEventPage());
    }
}