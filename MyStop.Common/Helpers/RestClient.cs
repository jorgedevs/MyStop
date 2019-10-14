using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using MyStop.Models;

namespace MyStop
{
	public sealed class RestClient
	{
        private const string RestServiceBaseAddress = "http://api.translink.ca/RTTIAPI/V1/stops/";
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
            client = new HttpClient() { BaseAddress = new Uri(RestServiceBaseAddress) };
            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(AcceptHeaderApplicationJson));
        }

        public async Task<Stop> GetStopInfoAsync(string busNumber)
        {
            Stop stop = new Stop();
            string jsonResponse = string.Empty;

            try
            {
                var response = await client.GetAsync(busNumber + "?apikey=GfGjjbw8OuxcGMNZiWgf", HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode)
                    return stop;

                jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Debug.WriteLine("::RESTCLIENT::GET_STOP_INFO = "+jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("CAUGHT EXCEPTION: " + e);
            }

            if (string.IsNullOrEmpty(jsonResponse))
                return stop;

            StopDTO stopInfo = new StopDTO();
            stopInfo = await Task.Run(() => JsonConvert.DeserializeObject<StopDTO>(jsonResponse)).ConfigureAwait(false);
            stop.Name = stopInfo.Name;
            stop.StopNo = stopInfo.StopNo;
            stop.Routes = stopInfo.Routes;

            return stop;
        }

        public async Task<List<Schedule>> GetSchedulesAsync(string busNumber)
        {
            List<Schedule> schedules = new List<Schedule>();
            string jsonResponse = string.Empty;

            try
            {
                HttpResponseMessage response = await client.GetAsync(busNumber + "/estimates?apiKey=GfGjjbw8OuxcGMNZiWgf&fake=", HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    return schedules;

                jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
              
                Debug.WriteLine("::RESTCLIENT::GET_SCHEDULES_INFO = " + jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("CAUGHT EXCEPTION: " + e);
            }

            if (string.IsNullOrEmpty(jsonResponse))
                return schedules;

            try
            {
            List<NextBusDTO> nextBuses = await Task.Run(() => JsonConvert.DeserializeObject<List<NextBusDTO>>(jsonResponse)).ConfigureAwait(false);

            foreach (var nextBus in nextBuses)
            {
                foreach (var schedule in nextBus.Schedules)
                {
                    Schedule scheduleItem = new Schedule();
                    scheduleItem.RouteNo = nextBus.RouteNo;
                    scheduleItem.Destination = schedule.Destination;
                    scheduleItem.ExpectedCountdown = schedule.ExpectedCountdown;
                    scheduleItem.ScheduleStatus = schedule.ScheduleStatus;
                    schedules.Add(scheduleItem);
                }
            }
            schedules.Sort((b0, b1) => b0.ExpectedCountdown.CompareTo(b1.ExpectedCountdown));
            }
            catch(Exception ex)
            {
                Debug.WriteLine("CAUGHT EXCEPTION: " + ex);
            }

            return schedules;
        }

        string GetStopInfoSample()
        {
            return @"
            {
	            ""StopNo"": 50023,
                ""Name"": ""WB DAVIE ST NS HAMILTON ST"",
	            ""BayNo"": ""N"",
	            ""City"": ""VANCOUVER"",
	            ""OnStreet"": ""DAVIE ST"",
	            ""AtStreet"": ""HAMILTON ST"",
	            ""Latitude"": 49.274860,
	            ""Longitude"": -123.122530,
	            ""WheelchairAccess"": 1,
	            ""Distance"": -1,
	            ""Routes"": ""006""
            }";
        }

        string GetSchedulesSample()
        {
            return @"
            [{
	            ""RouteNo"": ""002"",
	            ""RouteName"": ""MACDONALD\/DOWNTOWN "",
	            ""Direction"": ""EAST"",
	            ""RouteMap"": {
		            ""Href"": ""http:\/\/nb.translink.ca\/geodata\/002.kmz""
	            },
	            ""Schedules"": [{
			            ""Pattern"": ""EB1"",
			            ""Destination"": ""DOWNTOWN"",
			            ""ExpectedLeaveTime"": ""11:29pm 2017-06-13"",
			            ""ExpectedCountdown"": 4,
			            ""ScheduleStatus"": "" "",
			            ""CancelledTrip"": false,
			            ""CancelledStop"": false,
			            ""AddedTrip"": false,
			            ""AddedStop"": false,
			            ""LastUpdate"": ""11:21:47 pm""
		            }, {
			            ""Pattern"": ""EB1"",
			            ""Destination"": ""DOWNTOWN"",
			            ""ExpectedLeaveTime"": ""11:46pm 2017-06-13"",
			            ""ExpectedCountdown"": 21,
			            ""ScheduleStatus"": "" "",
			            ""CancelledTrip"": false,
			            ""CancelledStop"": false,
			            ""AddedTrip"": false,
			            ""AddedStop"": false,
			            ""LastUpdate"": ""10:24:10 pm""
		            }, {
			            ""Pattern"": ""EB1"",
			            ""Destination"": ""DOWNTOWN"",
			            ""ExpectedLeaveTime"": ""12:03am"",
			            ""ExpectedCountdown"": 38,
			            ""ScheduleStatus"": "" "",
			            ""CancelledTrip"": false,
			            ""CancelledStop"": false,
			            ""AddedTrip"": false,
			            ""AddedStop"": false,
			            ""LastUpdate"": ""10:41:03 pm""
		            }, {
			            ""Pattern"": ""EB1"",
			            ""Destination"": ""DOWNTOWN"",
			            ""ExpectedLeaveTime"": ""12:34am"",
			            ""ExpectedCountdown"": 69,
			            ""ScheduleStatus"": "" "",
			            ""CancelledTrip"": false,
			            ""CancelledStop"": false,
			            ""AddedTrip"": false,
			            ""AddedStop"": false,
			            ""LastUpdate"": ""11:12:03 pm""
		            }, {
			            ""Pattern"": ""EB1"",
			            ""Destination"": ""DOWNTOWN"",
			            ""ExpectedLeaveTime"": ""1:04am"",
			            ""ExpectedCountdown"": 99,
			            ""ScheduleStatus"": ""*"",
			            ""CancelledTrip"": false,
			            ""CancelledStop"": false,
			            ""AddedTrip"": false,
			            ""AddedStop"": false,
			            ""LastUpdate"": ""01:03:33 am""
		            }
	            ]
            }]";
        }
    }
}

