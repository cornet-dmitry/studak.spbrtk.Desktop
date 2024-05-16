using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
}