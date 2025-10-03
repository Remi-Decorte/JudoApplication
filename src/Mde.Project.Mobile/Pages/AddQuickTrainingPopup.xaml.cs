using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Graphics;

namespace Mde.Project.Mobile.Pages.Popups;

public partial class AddQuickTrainingPopup : Popup
{
    // ?? extra flag toegevoegd
    public record Result(DateTime Start, DateTime End, string Type, Color Color, bool OpenFullEditor = false);

    private Color _selectedColor = Color.FromArgb("#1976D2");
    private int _durationMinutes = 60; // default 1 uur 

    public AddQuickTrainingPopup(DateTime suggestedStart)
        : this(suggestedStart, TimeSpan.FromHours(1), null, null) { }

    // overload voor bewerken: vooraf ingevulde waarden
    public AddQuickTrainingPopup(DateTime suggestedStart, TimeSpan? initialDuration, string? initialType, Color? initialColor)
    {
        InitializeComponent();

        var start = suggestedStart == default ? DateTime.Now : suggestedStart;
        if (start.TimeOfDay == TimeSpan.Zero) start = start.Date.AddHours(9);

        DatePick.Date = start.Date;
        TimePick.Time = start.TimeOfDay;

        // duur init
        var dur = initialDuration ?? TimeSpan.FromHours(1);
        _durationMinutes = (int)dur.TotalMinutes;
        DurationPicker.SelectedIndex = _durationMinutes switch
        {
            30 => 0,
            60 => 1,
            90 => 2,
            120 => 3,
            180 => 4,
            _ => 1
        };

        // type init
        if (!string.IsNullOrWhiteSpace(initialType))
            TypePicker.SelectedItem = initialType;
        else
            TypePicker.SelectedIndex = 0;

        // kleur init
        if (initialColor is not null) _selectedColor = initialColor;

        // events
        DatePick.DateSelected += OnDateChanged;
        TimePick.PropertyChanged += OnTimeChanged;
        DurationPicker.SelectedIndexChanged += OnDurationChanged;

        foreach (var b in new[] { C1, C2, C3, C4, C5 })
            b.Clicked += (s, _) => _selectedColor = ((Button)s).BackgroundColor;

        UpdateHints();
    }

    private void OnCancel(object? sender, EventArgs e) => Close(null);

    private void OnSave(object? sender, EventArgs e)
    {
        var start = DatePick.Date.Add(TimePick.Time);
        var end = start.AddMinutes(_durationMinutes);
        var type = (TypePicker.SelectedItem as string) ?? "Training";

        Close(new Result(start, end, type, _selectedColor, OpenFullEditor: false));
    }

    //open de volledige editor (geen directe save)
    private void OnOpenFullEditor(object? sender, EventArgs e)
    {
        var start = DatePick.Date.Add(TimePick.Time);
        var end = start.AddMinutes(_durationMinutes);
        var type = (TypePicker.SelectedItem as string) ?? "Training";

        Close(new Result(start, end, type, _selectedColor, OpenFullEditor: true));
    }

    private void OnDateChanged(object? s, DateChangedEventArgs e) => UpdateHints();

    private void OnTimeChanged(object? s, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimePicker.Time))
            UpdateHints();
    }

    private void OnDurationChanged(object? s, EventArgs e)
    {
        _durationMinutes = DurationPicker.SelectedIndex switch
        {
            0 => 30,
            1 => 60,
            2 => 90,
            3 => 120,
            4 => 180,
            _ => 60
        };
        UpdateHints();
    }

    private void UpdateHints()
    {
        var start = DatePick.Date.Add(TimePick.Time);
        var end = start.AddMinutes(_durationMinutes);

        LblDateHint.Text = start.ToString("ddd d MMM yyyy HH:mm");
        LblEndHint.Text = $"Eindigt om {end:HH:mm}";
    }
}
