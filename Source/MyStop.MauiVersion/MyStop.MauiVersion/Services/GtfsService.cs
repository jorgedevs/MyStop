using MyStop.MauiVersion.CSVs;
using System.Diagnostics;
using System.IO.Compression;

namespace MyStop.MauiVersion.Services;



public class GtfsService : IGtfsService
{
    public async Task DownloadAndExtractGtfsAsync(string gtfsUrl, string targetFolderPath)
    {
        try
        {
            using var httpClient = new HttpClient();
            var zipBytes = await httpClient.GetByteArrayAsync(gtfsUrl);

            Directory.CreateDirectory(targetFolderPath);

            string tempZipPath = Path.Combine(targetFolderPath, "google_transit.zip");
            Directory.CreateDirectory(Path.GetDirectoryName(tempZipPath));
            File.WriteAllBytes(tempZipPath, zipBytes);

            string extractFolder = Path.Combine(targetFolderPath, "unzipped");

            if (Directory.Exists(extractFolder))
            {
                Directory.Delete(extractFolder, true);
            }

            Directory.CreateDirectory(extractFolder);
            ZipFile.ExtractToDirectory(tempZipPath, extractFolder);
            File.Delete(tempZipPath);
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
        }
    }

    public Agency ParseAgency(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        if (lines.Length < 2)
            return null; // No data

        var header = lines[0].Split(',');
        var values = lines[1].Split(',');

        return new Agency
        {
            agency_id = GetField("agency_id", header, values),
            agency_name = GetField("agency_name", header, values),
            agency_url = GetField("agency_url", header, values),
            agency_timezone = GetField("agency_timezone", header, values),
            agency_lang = GetField("agency_lang", header, values),
            agency_fare_url = GetField("agency_fare_url", header, values)
        };
    }

    public List<Calendar> ParseCalendar(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new Calendar
                {
                    service_id = GetField("service_id", header, values),
                    monday = GetField("monday", header, values),
                    tuesday = GetField("tuesday", header, values),
                    wednesday = GetField("wednesday", header, values),
                    thursday = GetField("thursday", header, values),
                    friday = GetField("friday", header, values),
                    saturday = GetField("saturday", header, values),
                    sunday = GetField("sunday", header, values),
                    start_date = GetField("start_date", header, values),
                    end_date = GetField("end_date", header, values),
                };
            })
            .ToList();
    }

    public List<CalendarDate> ParseCalendarDates(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new CalendarDate
                {
                    service_id = GetField("service_id", header, values),
                    date = GetField("date", header, values),
                    exception_type = GetField("exception_type", header, values),
                };
            })
            .ToList();
    }

    public List<DirectionNamesException> ParseDirectionNamesExceptions(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new DirectionNamesException
                {
                    route_name = GetField("route_name", header, values),
                    direction_id = GetField("direction_id", header, values),
                    direction_name = GetField("direction_name", header, values),
                    direction_do = GetField("direction_do", header, values),
                };
            })
            .ToList();
    }

    public List<Direction> ParseDirections(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new Direction
                {
                    direction = GetField("direction", header, values),
                    direction_id = GetField("direction_id", header, values),
                    route_id = GetField("route_id", header, values),
                    route_short_name = GetField("route_short_name", header, values),
                    LineIdConst = GetField("LineIdConst", header, values),
                };
            })
            .ToList();
    }

    public FeedInfo ParseFeedInfo(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        if (lines.Length < 2)
            return null; // No data

        var header = lines[0].Split(',');
        var values = lines[1].Split(',');

        return new FeedInfo
        {
            feed_publisher_name = GetField("feed_publisher_name", header, values),
            feed_publisher_url = GetField("feed_publisher_url", header, values),
            feed_lang = GetField("feed_lang", header, values),
            feed_start_date = GetField("feed_start_date", header, values),
            feed_end_date = GetField("feed_end_date", header, values),
            feed_version = GetField("feed_version", header, values)
        };
    }

    public List<RouteNamesException> ParseRouteNamesExceptions(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new RouteNamesException
                {
                    route_id = GetField("route_id", header, values),
                    route_name = GetField("route_name", header, values),
                    route_do = GetField("route_do", header, values),
                    name_type = GetField("name_type", header, values),
                };
            })
            .ToList();
    }

    public List<Route> ParseRoutes(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new Route
                {
                    route_id = GetField("route_id", header, values),
                    agency_id = GetField("agency_id", header, values),
                    route_short_name = GetField("route_short_name", header, values),
                    route_long_name = GetField("route_long_name", header, values),
                    route_desc = GetField("route_desc", header, values),
                    route_type = GetField("route_type", header, values),
                    route_url = GetField("route_url", header, values),
                    route_color = GetField("route_color", header, values),
                    route_text_color = GetField("route_text_color", header, values),
                };
            })
            .ToList();
    }

    public List<Shape> ParseShapes(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new Shape
                {
                    shape_id = GetField("shape_id", header, values),
                    shape_pt_lat = GetField("shape_pt_lat", header, values),
                    shape_pt_lon = GetField("shape_pt_lon", header, values),
                    shape_pt_sequence = GetField("shape_pt_sequence", header, values),
                    shape_dist_traveled = GetField("shape_dist_traveled", header, values)
                };
            })
            .ToList();
    }

    public List<SignupPeriod> ParseSignupPeriods(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new SignupPeriod
                {
                    sign_id = GetField("sign_id", header, values),
                    from_date = GetField("from_date", header, values),
                    to_date = GetField("to_date", header, values)
                };
            })
            .ToList();
    }

    public List<StopOrderException> ParseStopOrderExceptions(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new StopOrderException
                {
                    route_name = GetField("route_name", header, values),
                    direction_name = GetField("direction_name", header, values),
                    direction_do = GetField("direction_do", header, values),
                    stop_id = GetField("stop_id", header, values),
                    stop_name = GetField("stop_name", header, values),
                    stop_do = GetField("stop_do", header, values)
                };
            })
            .ToList();
    }

    public List<Stop> ParseStops(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new Stop
                {
                    stop_id = GetField("stop_id", header, values),
                    stop_code = GetField("stop_code", header, values),
                    stop_name = GetField("stop_name", header, values),
                    stop_desc = GetField("stop_desc", header, values),
                    stop_lat = GetField("stop_lat", header, values),
                    stop_lon = GetField("stop_lon", header, values),
                    zone_id = GetField("zone_id", header, values),
                    stop_url = GetField("stop_url", header, values),
                    location_type = GetField("location_type", header, values),
                    parent_station = GetField("parent_station", header, values),
                    wheelchair_boarding = GetField("wheelchair_boarding", header, values)
                };
            })
            .ToList();
    }

    public List<StopTime> ParseStopTimes(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new StopTime
                {
                    trip_id = GetField("trip_id", header, values),
                    arrival_time = GetField("arrival_time", header, values),
                    departure_time = GetField("departure_time", header, values),
                    stop_id = GetField("stop_id", header, values),
                    stop_sequence = GetField("stop_sequence", header, values),
                    stop_headsign = GetField("stop_headsign", header, values),
                    pickup_type = GetField("pickup_type", header, values),
                    drop_off_type = GetField("drop_off_type", header, values),
                    shape_dist_traveled = GetField("shape_dist_traveled", header, values),
                    timepoint = GetField("timepoint", header, values)
                };
            })
            .ToList();
    }

    public List<Transfer> ParseTransfers(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new Transfer
                {
                    from_stop_id = GetField("from_stop_id", header, values),
                    to_stop_id = GetField("to_stop_id", header, values),
                    transfer_type = GetField("transfer_type", header, values),
                    min_transfer_time = GetField("min_transfer_time", header, values),
                    from_trip_id = GetField("from_trip_id", header, values),
                    to_trip_id = GetField("to_trip_id", header, values)
                };
            })
            .ToList();
    }

    public List<Trip> ParseTrips(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var header = lines[0].Split(',');

        return lines
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var values = line.Split(',');

                return new Trip
                {
                    route_id = GetField("route_id", header, values),
                    service_id = GetField("service_id", header, values),
                    trip_id = GetField("trip_id", header, values),
                    trip_headsign = GetField("trip_headsign", header, values),
                    trip_short_name = GetField("trip_short_name", header, values),
                    direction_id = GetField("direction_id", header, values),
                    block_id = GetField("block_id", header, values),
                    shape_id = GetField("shape_id", header, values),
                    wheelchair_accessible = GetField("wheelchair_accessible", header, values),
                    bikes_allowed = GetField("bikes_allowed", header, values)
                };
            })
            .ToList();
    }

    private string GetField(string fieldName, string[] headers, string[] values)
    {
        int index = Array.IndexOf(headers, fieldName);
        return index >= 0 && index < values.Length ? values[index] : null;
    }
}