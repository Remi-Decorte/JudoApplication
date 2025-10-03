using System;
using System.Linq;
using CommunityToolkit.Maui.Views;
using Syncfusion.Maui.Scheduler;
using Mde.Project.Mobile.Pages.Popups;
using Mde.Project.Mobile.ViewModels;
using Microsoft.Maui.Graphics;

namespace Mde.Project.Mobile.Pages;

public partial class AgendaPage : ContentPage
{
    private readonly AgendaViewModel _vm;
    private readonly AddTrainingPage _addTrainingPage;

    public AgendaPage(AgendaViewModel vm, AddTrainingPage addTrainingPage)
    {
        InitializeComponent();
        _vm = vm;
        _addTrainingPage = addTrainingPage;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void OnAddTrainingClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(_addTrainingPage);
    }

    private async void Scheduler_Tapped(object sender, SchedulerTappedEventArgs e)
    {
        // Tap op lege cel -> nieuwe afspraak
        if (e.Element is SchedulerElement.SchedulerCell && e.Date.HasValue)
        {
            var popup = new AddQuickTrainingPopup(e.Date.Value);
            if (await this.ShowPopupAsync(popup) is AddQuickTrainingPopup.Result r1)
            {
                if (r1.OpenFullEditor)
                {
                    var iso = Uri.EscapeDataString(r1.Start.ToString("o"));
                    var mins = (int)(r1.End - r1.Start).TotalMinutes;
                    var type = Uri.EscapeDataString(r1.Type ?? string.Empty);

                    await Shell.Current.GoToAsync(
                        $"{nameof(Pages.AddTrainingPage)}?date={iso}&duration={mins}&type={type}");
                }
                else
                {
                    await _vm.CreateAndAddTrainingAsync(r1.Start, r1.End, r1.Type, r1.Color);
                }
            }
            return;
        }

        // Tap op bestaande afspraak -> bewerken of verwijderen
        if (e.Element is SchedulerElement.Appointment &&
            e.Appointments?.FirstOrDefault() is TrainingAppointment tapAppt)
        {
            var action = await DisplayActionSheet(tapAppt.Subject, "Annuleren", null, "Bewerken", "Verwijderen");
            if (action == "Verwijderen")
            {
                var ok = await DisplayAlert("Verwijderen", "Afspraak verwijderen?", "Ja", "Nee");
                if (ok) await _vm.DeleteTrainingAsync(tapAppt);
                return;
            }

            if (action == "Bewerken")
            {
                var start = tapAppt.StartTime;
                var duration = tapAppt.EndTime - tapAppt.StartTime;
                var color = (tapAppt.Background as SolidColorBrush)?.Color ?? Colors.Blue;

                var popup = new AddQuickTrainingPopup(
                    suggestedStart: start,
                    initialDuration: duration,
                    initialType: tapAppt.Subject,
                    initialColor: color);

                if (await this.ShowPopupAsync(popup) is AddQuickTrainingPopup.Result r2)
                {
                    if (r2.OpenFullEditor)
                    {
                        var iso = Uri.EscapeDataString(r2.Start.ToString("o"));
                        var mins = (int)(r2.End - r2.Start).TotalMinutes;
                        var type = Uri.EscapeDataString(r2.Type ?? string.Empty);

                        await Shell.Current.GoToAsync(
                            $"{nameof(Pages.AddTrainingPage)}?date={iso}&duration={mins}&type={type}");
                    }
                    else
                    {
                        await _vm.UpdateTrainingAsync(tapAppt, r2.Start, r2.End, r2.Type, r2.Color);
                    }
                }
            }
        }
    }
}
