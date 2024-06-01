using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
    
    private ProgressBar _loader;

    private readonly HttpClient _httpClient;
    public EventsPage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();
        EventsNavBtn.FontWeight = FontWeight.ExtraBold;

        LoadData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        SearchTextBox = this.Find<TextBox>("SearchTextBox");
        
        _loader = this.Find<ProgressBar>("Loader");

        ActivistsNavBtn = this.Find<Button>("ActivistsNavBtn");
        EventsNavBtn = this.Find<Button>("EventsNavBtn");
        KpiNavBtn = this.Find<Button>("KpiNavBtn");
        DocsNavBtn = this.Find<Button>("DocsNavBtn");
        CreateEventBtn = this.Find<Button>("CreateEventBtn");

        EventSortComboBox = this.Find<ComboBox>("EventSortComboBox");
        EventsListBox = this.Find<ListBox>("EventsListBox");

        UserStatusTextBlock = this.Find<TextBlock>("UserStatusTextBlock");
        UserNameTextBlock = this.Find<TextBlock>("UserNameTextBlock");
    }

    private async void LoadData()
    {
        _loader.IsVisible = true;
        try
        {
            UserNameTextBlock.Text = ApplicationState.GetValue<string>("UserNameTextBlock");
            UserStatusTextBlock.Text = ApplicationState.GetValue<string>("UserStatusTextBlock");
            
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

                if (VARIABLE.Isactice == false)
                {
                    VARIABLE.Name = "CLOSE | " + VARIABLE.Name;
                }
            }
            
            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                string[] searchwords = SearchTextBox.Text.ToLower()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                events = events.Where(a => searchwords.All(word =>
                    (a.Name != null && a.Name.ToLower().Contains(word))
                    || (a.Description != null && a.Description.ToLower().Contains(word))
                    || (a.Place != null && a.Place.ToLower().Contains(word))))
                    .ToList();
            }

            switch (EventSortComboBox.SelectedIndex)
            {
                case 0:
                {
                    events = events.OrderBy(x => x.StartDate)
                        .OrderBy(x => x.StartTime).ToList();
                    break;
                }
                
                case 1: 
                {
                    events = events.OrderByDescending(x => x.Rate).ToList();
                    break;
                }
                case 2:
                {
                    events = events.OrderByDescending(x => x.ActivistsNumber).ToList();
                    break;
                }
            }

            events = events.OrderByDescending(x => x.Isactice).ToList();
            
            EventsListBox.ItemsSource = events;
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
        }

        _loader.IsVisible = false;
    }
    
    private void EventsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItems != null)
            {
                var selectedEvent = (e.AddedItems[0] as Event);
                Navigation.NavigateTo(new EventCardPage(selectedEvent));
            }
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

    private void CreateEventBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new CreateEventPage());
    }

    private void EventSortComboBox_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        LoadData();
    }

    private void SearchTextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        LoadData();
    }

    private void KpiNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new RatePage());
    }
}