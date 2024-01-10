using System.Text.Json;

namespace BoraCrawlers
{
	public class EventbriteHttpClient
	{
		readonly HttpClient _httpClient;
		public string EventsSchedule { get; private set; }

		public EventbriteHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
		public async Task<IEnumerable<EventbriteEvent>> ListEventsAsync(string organizerId)
		{
			var token = _httpClient.DefaultRequestHeaders.Authorization?.Parameter;
			ArgumentException.ThrowIfNullOrEmpty(_httpClient.DefaultRequestHeaders.Authorization?.Parameter);

			var requestUri = $"organizers/{organizerId}/events?order_by=start_desc&token={token}";
			HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
			var eventsString = await response.Content.ReadAsStringAsync();
			var events = JsonSerializer.Deserialize<EventbriteEvents>(eventsString, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});
			return events!.Events;
		}
	}

	public class EventbriteEvents
	{
        public IEnumerable<EventbriteEvent> Events { get; set; }
    }

	public class EventbriteEvent 
	{
        public EventbriteText Name { get; set; }
        public EventbriteDateTime Start { get; set; }
		public EventbriteDateTime End { get; set; }
		public string Summary { get; set; }
        public string Url { get; set; }
    }

	public class EventbriteText 
	{
        public string Text { get; set; }
	}
	public class EventbriteDateTime 
	{
        public string Timezone { get; set; }
        public DateTimeOffset Local { get; set; }
		public DateTimeOffset Utc { get; set; }
	}
}
