using CommunityToolkit.Maui.Views;

namespace Mde.Project.Mobile.Pages.Popups;

public partial class AddQuickTrainingPopup : Popup
{
    public record Result(DateTime Start, string Type, Color Color);

    private Color _selectedColor = Color.FromArgb("#1976D2");

    public AddQuickTrainingPopup(DateTime suggestedStart)
    {
        InitializeComponent();

        var start = suggestedStart == default ? DateTime.Now : suggestedStart;
        // Month view taps are midnight; default to 09:00
        if (start.TimeOfDay == TimeSpan.Zero) start = start.Date.AddHours(9);

        DatePick.Date = start.Date;
        TimePick.Time = start.TimeOfDay;
        TypePicker.SelectedIndex = 0;
        LblDateHint.Text = start.ToString("ddd d MMM yyyy HH:mm");

        foreach (var btn in new[] { C1, C2, C3, C4, C5 })
            btn.Clicked += (s, e) => _selectedColor = ((Button)s!).BackgroundColor;
    }

    private void OnCancel(object? sender, EventArgs e) => Close(null);

    private void OnSave(object? sender, EventArgs e)
    {
        var start = DatePick.Date.Add(TimePick.Time);
        var type = (TypePicker.SelectedItem as string) ?? "Training";
        Close(new Result(start, type, _selectedColor));
    }
}
