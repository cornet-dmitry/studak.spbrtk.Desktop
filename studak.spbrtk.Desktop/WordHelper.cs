using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using studak.spbrtk.API.Models;
using studak.spbrtk.Desktop;
using studak.spbrtk.Desktop.Models;
using Xceed.Document.NET;
using Xceed.Words.NET;

public class WordHelper
{
    private readonly HttpClient _httpClient;
    private readonly string apiUrl = ApplicationState.GetValue<string>("apiUrl");
    
    private static string templateFileName = "Documents/Templates/shablon-prikaza-rtk.docx";
    private static string relativeSaveDirectory = @"Documents";

    public bool GenerateWordFile(Event _event)
    {
        try
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string saveDirectory = Path.Combine(baseDirectory, relativeSaveDirectory);
        
            if (!File.Exists(templateFileName))
            {
                throw new ArgumentException("Template file not found");
            }
        
            // Ensure the directory exists
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string newFileName = Path.Combine(saveDirectory, $"Приказ_{DateTime.Now:yyyyMMdd_HHmmss}.docx");

            // Открытие шаблона
            using (var document = DocX.Load(templateFileName))
            {
                // Словарь с заменами
                var replacements = new Dictionary<string, string>
                {
                    { "{EVENT_NAME}", _event.Name },
                    { "{EVENT_PLACE}", _event.Place },
                    { "{FULL_DATE}", _event.FullDateTime },
                    { "{NUM}", "67" },
                    { "{DATE}", DateTime.Now.ToString("dd.MM.yyyy") }
                };

                // Замена значений в документе
                foreach (var item in replacements)
                {
                    document.ReplaceText(item.Key, item.Value);
                }
                
                var listItems = ApplicationState.GetValue<List<Activists>>("tempActivistsList").Select((activist, index) =>
                    $"{index + 1}. {activist.Surname} {activist.Name} {activist.Patronymic} – группа {activist.Group}"
                ).ToArray();
                
            
                string studentsList = string.Join(Environment.NewLine, listItems);
                document.ReplaceText("{STUDENTS_LIST}", studentsList);
            
                // Сохранение документа с новыми данными
                document.SaveAs(newFileName);
            }
            return true;
        }
        catch (Exception e)
        {
            MessageBox messageBox = new MessageBox(e.Message);
            messageBox.Show();
            return false;
        }
    }
}