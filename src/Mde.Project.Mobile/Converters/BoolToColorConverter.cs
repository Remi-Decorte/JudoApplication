using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Converters
{
    public sealed class BoolToColorConverter : IValueConverter
    {
        // Week-knop actief als IsWeekView = true, Maand-knop actief als IsWeekView = false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isWeekView = value is bool b && b;
            string mode = parameter?.ToString() ?? ""; // "On" of "Off"
            bool active = (mode.Equals("On", StringComparison.OrdinalIgnoreCase) && isWeekView)
                          || (mode.Equals("Off", StringComparison.OrdinalIgnoreCase) && !isWeekView);

            return active ? Color.FromArgb("#E6F2FF") : Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
