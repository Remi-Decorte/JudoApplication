using Mde.Project.Mobile.Models;
using System.Net.Http.Json;


namespace Mde.Project.Mobile.Pages;

    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            LoadEventsAsync(); // Laad automatisch de events bij opstart
        }

        private async Task LoadEventsAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:62160/"); // Vervang met jouw juiste poort

                var events = await client.GetFromJsonAsync<List<EventModel>>("api/events/upcoming");

                if (events != null)
                {
                    // Toon ze bijvoorbeeld in een StackLayout of Label
                    foreach (var e in events)
                    {
                        EventsStack.Children.Add(new Label
                        {
                            Text = $"{e.Title} - {e.Location} ({e.Date:dd/MM/yyyy})",
                            FontSize = 14,
                            TextColor = Colors.Black
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Kan events niet laden: {ex.Message}", "Ok");
            }
        }
    }