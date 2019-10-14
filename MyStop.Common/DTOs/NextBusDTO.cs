using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyStop
{
	[JsonObject("NextBus")]
	public class NextBusDTO
	{
		[JsonProperty("RouteNo")]
		public string RouteNo { get; set; }

		[JsonProperty("RouteName")]
		public string RouteName { get; set; }

		[JsonProperty("Direction")]
		public string Direction { get; set; }

		//[JsonProperty("RouteMap")]
		//public string RouteMap { get; set; }

		[JsonProperty("Schedules")]
		public ObservableCollection<ScheduleDTO> Schedules { get; set; }
	}
}
