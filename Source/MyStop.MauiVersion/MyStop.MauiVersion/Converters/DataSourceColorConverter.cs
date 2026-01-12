using System.Globalization;

namespace MyStop.MauiVersion.Converters;

/// <summary>
/// Converts DataSourceText to a color for the badge background.
/// "Realtime" = Green, "Schedule" = Gray
/// </summary>
public class DataSourceColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string dataSource)
        {
            return dataSource switch
            {
                "Realtime" => Color.FromArgb("#4CAF50"), // Green
                "Schedule" => Color.FromArgb("#757575"), // Gray
                _ => Color.FromArgb("#757575")
            };
        }

        return Color.FromArgb("#757575");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
