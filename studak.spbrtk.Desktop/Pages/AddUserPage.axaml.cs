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

public partial class AddUserPage : UserControl
{
    private readonly string apiUrl = ApplicationState.GetValue<string>("apiUrl");
    
    private ProgressBar _loader;

    private readonly HttpClient _httpClient;

    private static Activists _activists;
    
    public AddUserPage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();
        
        FIOTextBlock.Text = "ДОБАВИТЬ АКТИВИСТА";
        ActivistsNavBtn.FontWeight = FontWeight.ExtraBold;
        SaveBtn.IsVisible = false;
        CreateBtn.IsVisible = true;
        
        LoadComboBox();
    }
    
    public AddUserPage(Activists activist)
    {
        _activists = activist;
        _httpClient = new HttpClient();
        InitializeComponent();
        
        FIOTextBlock.Text = "РЕДАКТИРОВАТЬ ДАННЫЕ";
        ActivistsNavBtn.FontWeight = FontWeight.ExtraBold;
        SaveBtn.IsVisible = true;
        CreateBtn.IsVisible = false;
        
        LoadComboBox();
        LoadData();
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
        SaveBtn = this.Find<Button>("SaveBtn");

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
                list = list.OrderBy(x => x.Id).ToList();
                
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

    private async void LoadData()
    {
        SurnameTextBox.Text = _activists.Surname;
        NameTextBox.Text = _activists.Name;
        PatrTextBox.Text = _activists.Patronymic;
        GroupTextBox.Text = _activists.Group;
        DatePicker.SelectedDate = _activists.DateBirth;
        PhoneTextBox.Text = _activists.Phone;
        EmailTextBox.Text = _activists.Email;
        VKTextBox.Text = _activists.VkLink;
        TGTextBox.Text = _activists.TgLink;
        
        var userData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "userId", _activists.Id.ToString() }
        });

        var responceUsers =
            await _httpClient.PostAsync(apiUrl + "/User/GetUserByID", userData);

        var userContent = await responceUsers.Content.ReadAsStringAsync();
        var usertList = JsonConvert.DeserializeObject<List<Activists>>(userContent).FirstOrDefault();

        StatusComboBox.SelectedIndex = Convert.ToInt32(usertList.Status) - 1;
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
                    { "phone", _activists.Phone },
                    { "email", EmailTextBox.Text },
                    { "vkLink", VKTextBox.Text },
                    { "tgLink", TGTextBox.Text },
                    { "kpi", "0" },
                    { "status", (StatusComboBox.SelectedIndex + 1).ToString() },
                    { "orderNumber", "0" },
                    { "startDate", DateTime.Now.ToString() },
                });
            
                var response = await _httpClient.PostAsync($"{this.apiUrl}/User/AddUser", userData);
            
                Navigation.NavigateTo(new ActivistCardPage(_activists));
            }
            catch (Exception exception)
            {
                MessageBox messageBox = new MessageBox(exception.Message);
                messageBox.Show();
            }
        }

        _loader.IsVisible = false;
    }
    
    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _loader.IsVisible = true;
        
        if (CheckData())
        {
            try
            {
                var userData = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    /*{ "id", _activists.Id.ToString()},*/
                    { "surname", SurnameTextBox.Text},
                    { "name", NameTextBox.Text},
                    { "patronymic", PatrTextBox.Text},
                    { "group", GroupTextBox.Text },
                    { "dateBirth", DatePicker.SelectedDate.ToString() },
                    { "phone", _activists.Phone },
                    { "email", EmailTextBox.Text },
                    { "vkLink", VKTextBox.Text },
                    { "tgLink", TGTextBox.Text },
                    { "kpi", _activists.Kpi.ToString() },
                    { "status", (StatusComboBox.SelectedIndex + 1).ToString() },
                    { "orderNumber", "0" },
                    { "startDate", DateTime.Now.ToString() }
                });
            
                var response = await _httpClient.PostAsync($"{this.apiUrl}/User/EditUser/{_activists.Id}", userData);
            
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
        if (_activists is null)
        {
            Navigation.NavigateTo(new ActivistsPage());
        }
        else
        {
            Navigation.NavigateTo(new ActivistCardPage(_activists));
        }
    }

    private void EventsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventsPage());
    }

    private void KpiNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new RatePage());
    }

    private void DocsNavBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new DocumentsPage());
    }
}