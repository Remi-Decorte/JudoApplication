using Mde.Project.Mobile.Models;
using System.Net.Http;
using System.Text.Json;

namespace Mde.Project.Mobile.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadEventsAsync();
    }

    private async Task LoadEventsAsync()
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:62160/"); // jouw backend

        try
        {
            var response = await client.GetAsync("api/events/upcoming");

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var events = await JsonSerializer.DeserializeAsync<List<EventModel>>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (events != null)
                {
                    EventsStack.Children.Clear();
                    foreach (var e in events)
                    {
                        EventsStack.Children.Add(new Label
                        {
                            Text = $"{e.Title} in {e.Location} ({e.Date:dd/MM/yyyy})",
                            FontSize = 18,
                            TextColor = Colors.Black
                        });
                    }
                }
            }
            else
            {
                await DisplayAlert("Fout", $"Server gaf geen succes terug: {response.StatusCode}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Kon events niet laden: {ex.Message}", "OK");
        }
    }
}
