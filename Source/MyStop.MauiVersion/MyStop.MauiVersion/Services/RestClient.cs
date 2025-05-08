using MyStop.MauiVersion.CSVs;
using MyStop.MauiVersion.DTOs;
using MyStop.MauiVersion.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace MyStop.MauiVersion.Utils;

public sealed class RestClient
{
    private readonly string API_KEY = "API_KEY";
    private const string RestServiceBaseAddress = "https://api.translink.ca/RTTIAPI/V1/stops/";
    private const string AcceptHeaderApplicationJson = "application/json";
    private HttpClient client;

    static RestClient _instance = null;
    public static RestClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RestClient();
            }
            return _instance;
        }
    }

    private RestClient()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri(RestServiceBaseAddress)
        };
        client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(AcceptHeaderApplicationJson));
    }

    public async Task<Stop> GetBusStopInfo(string busNumber)
    {
        var stop = new Stop();
        string jsonResponse = string.Empty;

        try
        {
            var response = await client.GetAsync($"{busNumber}?apikey={API_KEY}", HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            if (!response.IsSuccessStatusCode)
            {
                return stop;
            }

            jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Debug.WriteLine("::RESTCLIENT::GET_STOP_INFO = " + jsonResponse);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                return stop;
            }

            var stopInfo = JsonConvert.DeserializeObject<StopDTO>(jsonResponse);

            //stop.Name = stopInfo?.Name;
            //stop.StopNo = stopInfo?.StopNo;
            //stop.Routes = stopInfo?.Routes;
        }
        catch (HttpRequestException e)
        {
            Debug.WriteLine("CAUGHT EXCEPTION: " + e);
        }

        return stop;
    }

    public async Task<List<Schedule>> GetBusArrivalsTimes(string busNumber)
    {
        var schedules = new List<Schedule>();
        string jsonResponse = string.Empty;

        try
        {
            var response = await client.GetAsync($"{busNumber}/estimates?apiKey={API_KEY}", HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            if (!response.IsSuccessStatusCode)
            {
                return schedules;
            }

            jsonResponse = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("::RESTCLIENT::GET_SCHEDULES_INFO = " + jsonResponse);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                return schedules;
            }

            var nextBuses = JsonConvert.DeserializeObject<List<NextBusDTO>>(jsonResponse);

            if (nextBuses == null)
            {
                return schedules;
            }

            foreach (var nextBus in nextBuses)
            {
                if (nextBus.Schedules == null)
                {
                    return schedules;
                }

                foreach (var schedule in nextBus.Schedules)
                {
                    var scheduleItem = new Schedule();
                    scheduleItem.RouteNo = nextBus.RouteNo;
                    scheduleItem.Destination = schedule.Destination;
                    scheduleItem.ExpectedCountdown = schedule.ExpectedCountdown;
                    scheduleItem.ScheduleStatus = schedule.ScheduleStatus;
                    schedules.Add(scheduleItem);
                }
            }

            schedules.Sort((b0, b1) => b0.ExpectedCountdown.CompareTo(b1.ExpectedCountdown));
        }
        catch (Exception ex)
        {
            Debug.WriteLine("CAUGHT EXCEPTION: " + ex);
        }

        return schedules;
    }
}