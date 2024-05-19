using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using studak.spbrtk.Desktop.Models;

namespace studak.spbrtk.Desktop.Pages;

public partial class WelcomePage : UserControl
{
    private readonly HttpClient _httpClient;
    public WelcomePage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        Login = this.Find<TextBox>("Login");
        Password = this.Find<TextBox>("Password");
        LoginBtn = this.Find<Button>("LoginBtn");
    }

    private async void LoginBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (Login.Text.Length != 0 && Password.Text.Length != 0)
            {
                var userData = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "username", Login.Text},
                    { "password", Password.Text},
                });
            
                var response = await _httpClient.PostAsync($"{ApplicationState.GetValue<string>("apiUrl")}/Auth/login", userData);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string token = await response.Content.ReadAsStringAsync();
                    
                    SetDataUser(token);
                    ApplicationState.SetValue("token", token);
                    
                    Navigation.NavigateTo(new ActivistsPage());
                }
                else
                {
                    MessageBox messageBox = new MessageBox("Неверный логин или пароль!");
                    messageBox.Show();
                }
            }
            else
            {
                MessageBox messageBox = new MessageBox("Введите логин и пароль!");
                messageBox.Show();
            }
        }
        catch (Exception exception)
        {
            MessageBox messageBox = new MessageBox(exception.Message);
            messageBox.Show();
        }
    }

    private async void SetDataUser(string _token)
    {
        // Добавляем токен в заголовок Authorization
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt = tokenHandler.ReadJwtToken(_token);
        
        // Чтение значения конкретного клейма (claim)
        string userId = jwt.Claims.First(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            
        var userData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "userId", userId}
                
        });
        var userResponce = await _httpClient.PostAsync(
            $"{ApplicationState.GetValue<string>("apiUrl")}/User/GetUserByID/", userData);
        var responseValue1 = userResponce.Content.ReadAsStringAsync().Result;
        var userResponceList = JsonConvert.DeserializeObject<List<Activists>>(responseValue1);
            
        ApplicationState.SetValue("activeUser", userResponceList[0]);

        string username = ApplicationState.GetValue<Activists>("activeUser").Surname + " " +
                          ApplicationState.GetValue<Activists>("activeUser").Name;
        
        ApplicationState.SetValue("UserNameTextBlock", username);
        
        //получение списка всех статусов
        var statusResponse = await _httpClient.GetAsync(ApplicationState.GetValue<string>("apiUrl")
                                                        + "/UserStatus/GetUserStatus");
        var statusContent = await statusResponse.Content.ReadAsStringAsync();
        var statusList = JsonConvert.DeserializeObject<List<UserStatus>>(statusContent);

        foreach (var VARIABLE in statusList)
        {
            if (ApplicationState.GetValue<Activists>("activeUser").Status == VARIABLE.Id.ToString())
            {
                ApplicationState.SetValue("UserStatusTextBlock", VARIABLE.StatusName);
            }
        }
    }
}