using System.Diagnostics;

namespace MyStop.MauiVersion.Services;

/// <summary>
/// Calculates sunrise and sunset times using astronomical algorithms.
/// This is similar to how Google Maps determines day/night mode.
/// Based on the NOAA Solar Calculator algorithms.
/// </summary>
public static class SunCalculator
{
    /// <summary>
    /// Calculates sunrise and sunset times for a given date and location.
    /// </summary>
    /// <param name="date">The date to calculate for</param>
    /// <param name="latitude">Latitude in degrees (positive = North)</param>
    /// <param name="longitude">Longitude in degrees (positive = East, negative = West)</param>
    /// <param name="utcOffset">UTC offset in hours (e.g., -8 for PST, -7 for PDT)</param>
    /// <returns>Tuple containing sunrise and sunset times in local time</returns>
    public static (DateTime Sunrise, DateTime Sunset) Calculate(DateTime date, double latitude, double longitude, double utcOffset)
    {
        // Use a more accurate algorithm based on NOAA spreadsheet
        var jd = GetJulianDay(date.Year, date.Month, date.Day);
        var jc = (jd - 2451545) / 36525; // Julian century

        // Sun calculations
        var geomMeanLongSun = Mod(280.46646 + jc * (36000.76983 + 0.0003032 * jc), 360);
        var geomMeanAnomSun = 357.52911 + jc * (35999.05029 - 0.0001537 * jc);
        var eccentEarthOrbit = 0.016708634 - jc * (0.000042037 + 0.0000001267 * jc);
        var sunEqOfCtr = Math.Sin(ToRad(geomMeanAnomSun)) * (1.914602 - jc * (0.004817 + 0.000014 * jc))
                       + Math.Sin(ToRad(2 * geomMeanAnomSun)) * (0.019993 - 0.000101 * jc)
                       + Math.Sin(ToRad(3 * geomMeanAnomSun)) * 0.000289;
        var sunTrueLong = geomMeanLongSun + sunEqOfCtr;
        var sunAppLong = sunTrueLong - 0.00569 - 0.00478 * Math.Sin(ToRad(125.04 - 1934.136 * jc));
        var meanObliqEcliptic = 23 + (26 + ((21.448 - jc * (46.815 + jc * (0.00059 - jc * 0.001813)))) / 60) / 60;
        var obliqCorr = meanObliqEcliptic + 0.00256 * Math.Cos(ToRad(125.04 - 1934.136 * jc));
        var sunDeclin = ToDeg(Math.Asin(Math.Sin(ToRad(obliqCorr)) * Math.Sin(ToRad(sunAppLong))));

        var varY = Math.Tan(ToRad(obliqCorr / 2)) * Math.Tan(ToRad(obliqCorr / 2));
        var eqOfTime = 4 * ToDeg(varY * Math.Sin(2 * ToRad(geomMeanLongSun))
                     - 2 * eccentEarthOrbit * Math.Sin(ToRad(geomMeanAnomSun))
                     + 4 * eccentEarthOrbit * varY * Math.Sin(ToRad(geomMeanAnomSun)) * Math.Cos(2 * ToRad(geomMeanLongSun))
                     - 0.5 * varY * varY * Math.Sin(4 * ToRad(geomMeanLongSun))
                     - 1.25 * eccentEarthOrbit * eccentEarthOrbit * Math.Sin(2 * ToRad(geomMeanAnomSun)));

        // Hour angle for sunrise/sunset
        var haArg = Math.Cos(ToRad(90.833)) / (Math.Cos(ToRad(latitude)) * Math.Cos(ToRad(sunDeclin)))
                  - Math.Tan(ToRad(latitude)) * Math.Tan(ToRad(sunDeclin));
        haArg = Math.Max(-1, Math.Min(1, haArg)); // Clamp for polar regions
        var haSunrise = ToDeg(Math.Acos(haArg));

        // Solar noon (in minutes from midnight, LOCAL time)
        var solarNoon = (720 - 4 * longitude - eqOfTime + utcOffset * 60);

        // Sunrise and sunset times (in minutes from midnight)
        var sunriseMinutes = solarNoon - haSunrise * 4;
        var sunsetMinutes = solarNoon + haSunrise * 4;

        var sunrise = date.Date.AddMinutes(sunriseMinutes);
        var sunset = date.Date.AddMinutes(sunsetMinutes);

        return (sunrise, sunset);
    }

    private static double GetJulianDay(int year, int month, int day)
    {
        if (month <= 2)
        {
            year -= 1;
            month += 12;
        }
        var a = Math.Floor(year / 100.0);
        var b = 2 - a + Math.Floor(a / 4.0);
        return Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + b - 1524.5;
    }

    private static double Mod(double val, double mod) => val - mod * Math.Floor(val / mod);
    private static double ToRad(double deg) => deg * Math.PI / 180.0;
    private static double ToDeg(double rad) => rad * 180.0 / Math.PI;

    /// <summary>
    /// Determines if it's currently night time based on device location or defaults to Vancouver.
    /// </summary>
    public static async Task<bool> IsNightTimeAsync()
    {
        try
        {
            var now = DateTime.Now;
            double latitude, longitude, utcOffset;

            try
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
            catch
            {
                (latitude, longitude, utcOffset) = GetVancouverDefaults(now);
            }

            var (sunrise, sunset) = Calculate(now, latitude, longitude, utcOffset);

            return now.TimeOfDay < sunrise.TimeOfDay || now.TimeOfDay > sunset.TimeOfDay;
        }
        catch
        {
            var hour = DateTime.Now.Hour;
            return hour >= 17 || hour < 8; // Winter-appropriate fallback
        }
    }

    /// <summary>
    /// Synchronous version for simpler use cases - uses cached/default location.
    /// </summary>
    public static bool IsNightTime()
    {
        try
        {
            var now = DateTime.Now;
            var (latitude, longitude, utcOffset) = GetVancouverDefaults(now);
            var (sunrise, sunset) = Calculate(now, latitude, longitude, utcOffset);

            return now.TimeOfDay < sunrise.TimeOfDay || now.TimeOfDay > sunset.TimeOfDay;
        }
        catch
        {
            var hour = DateTime.Now.Hour;
            return hour >= 17 || hour < 8;
        }
    }

    /// <summary>
    /// Gets Vancouver, BC defaults (where TransLink operates).
    /// </summary>
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
                // Winter = PST (-8), Summer = PDT (-7)
                // DST starts 2nd Sunday March, ends 1st Sunday November
                offset = -8;
            }
        }

        return (vancouverLat, vancouverLon, offset);
    }
}
