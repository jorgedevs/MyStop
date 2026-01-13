using System.Diagnostics;

namespace MyStop.MauiVersion.Services;

/// <summary>
/// Interface for theme management based on time of day.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets whether the current theme is night mode.
    /// </summary>
    bool IsNight { get; }

    /// <summary>
    /// Event raised when the theme changes.
    /// </summary>
    event EventHandler<bool>? ThemeChanged;

    /// <summary>
    /// Initializes the theme service and starts monitoring for theme changes.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Forces a theme refresh based on current time and location.
    /// </summary>
    Task RefreshThemeAsync();

    /// <summary>
    /// Stops the theme monitoring timer.
    /// </summary>
    void StopMonitoring();
}

/// <summary>
/// Manages app theme based on sunrise/sunset times, similar to Google Maps.
/// </summary>
public class ThemeService : IThemeService
{
    private bool _isNight;
    private bool _isInitialized;
    private IDispatcherTimer? _themeTimer;
    private DateTime _cachedSunrise;
    private DateTime _cachedSunset;
    private DateTime _lastCalculationDate;

    public bool IsNight
    {
        get => _isNight;
        private set
        {
            if (_isNight != value || !_isInitialized)
            {
                _isNight = value;
                _isInitialized = true;
                ApplyTheme();
                ThemeChanged?.Invoke(this, value);
            }
        }
    }

    public event EventHandler<bool>? ThemeChanged;

    public ThemeService()
    {
        // Initialize with safe defaults until async calculation completes
        var now = DateTime.Now;
        _cachedSunrise = now.Date.AddHours(8);  // 8:00 AM default
        _cachedSunset = now.Date.AddHours(16).AddMinutes(30);  // 4:30 PM default (winter)
        _lastCalculationDate = DateTime.MinValue; // Force recalculation
    }

    public async Task InitializeAsync()
    {
        await RefreshThemeAsync();
        StartThemeMonitoring();
    }

    public async Task RefreshThemeAsync()
    {
        try
        {
            var now = DateTime.Now;

            // Recalculate sunrise/sunset if it's a new day or first run
            if (_lastCalculationDate.Date != now.Date)
            {
                await CalculateSunTimes();
            }

            // Determine if it's night: before sunrise OR after sunset
            var currentTime = now.TimeOfDay;
            var sunriseTime = _cachedSunrise.TimeOfDay;
            var sunsetTime = _cachedSunset.TimeOfDay;

            var isNightNow = currentTime < sunriseTime || currentTime > sunsetTime;

            IsNight = isNightNow;
        }
        catch (Exception)
        {
            // Fallback: winter hours for Vancouver
            var hour = DateTime.Now.Hour;
            IsNight = hour >= 17 || hour < 8;
        }
    }

    private async Task CalculateSunTimes()
    {
        try
        {
            var now = DateTime.Now;
            double latitude, longitude, utcOffset;

            try
            {
                // Try to get device location
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();
                    if (location == null)
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Low, TimeSpan.FromSeconds(5));
                        location = await Geolocation.GetLocationAsync(request);
                    }

                    if (location != null)
                    {
                        latitude = location.Latitude;
                        longitude = location.Longitude;
                        utcOffset = TimeZoneInfo.Local.GetUtcOffset(now).TotalHours;
                    }
                    else
                    {
                        (latitude, longitude, utcOffset) = GetVancouverDefaults(now);
                    }
                }
                else
                {
                    (latitude, longitude, utcOffset) = GetVancouverDefaults(now);
                }
            }
            catch (Exception)
            {
                (latitude, longitude, utcOffset) = GetVancouverDefaults(now);
            }

            var (sunrise, sunset) = SunCalculator.Calculate(now, latitude, longitude, utcOffset);
            _cachedSunrise = sunrise;
            _cachedSunset = sunset;
            _lastCalculationDate = now;
        }
        catch (Exception)
        {
            // Use winter defaults for Vancouver as fallback
            var now = DateTime.Now;
            _cachedSunrise = now.Date.AddHours(8);  // 8:00 AM
            _cachedSunset = now.Date.AddHours(16).AddMinutes(30);  // 4:30 PM
            _lastCalculationDate = now;
        }
    }

    private static (double Latitude, double Longitude, double UtcOffset) GetVancouverDefaults(DateTime date)
    {
        const double vancouverLat = 49.2827;
        const double vancouverLon = -123.1207;

        double offset;
        try
        {
            var pst = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            offset = pst.GetUtcOffset(date).TotalHours;
        }
        catch
        {
            try
            {
                var pst = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                offset = pst.GetUtcOffset(date).TotalHours;
            }
            catch
            {
                offset = -8;
            }
        }

        return (vancouverLat, vancouverLon, offset);
    }

    private void StartThemeMonitoring()
    {
        _themeTimer = Application.Current?.Dispatcher.CreateTimer();
        if (_themeTimer != null)
        {
            _themeTimer.Interval = TimeSpan.FromMinutes(1);
            _themeTimer.Tick += async (s, e) => await RefreshThemeAsync();
            _themeTimer.Start();
        }
    }

    public void StopMonitoring()
    {
        _themeTimer?.Stop();
        _themeTimer = null;
    }

    private void ApplyTheme()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                if (Application.Current?.Resources != null)
                {
                    var skyColor = _isNight ? Color.FromArgb("#133B4F") : Color.FromArgb("#06bfcc");
                    Application.Current.Resources["SkyColor"] = skyColor;
                }
            }
            catch
            {
                // Ignore theme application errors
            }
        });
    }
}
