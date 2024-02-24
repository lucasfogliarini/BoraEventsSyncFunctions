using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BoraEventsSyncFunctions.BoraHttp
{
	public class BoraHttpClient
    {
        readonly HttpClient _httpClient;

        public BoraHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task PostEventAsync(EventCreated eventCreated)
        {
            var url = $"/events?user={eventCreated.BoraUser}";
            await PostAsync(url, eventCreated);
        }

        public async Task<IEnumerable<EventOutput>> GetEventsAsync(string user, EventsFilterInput eventsFilterInput)
        {
			var requestUri = $"/events?user={user}";
            var getRequest = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
                Content = JsonContent.Create(eventsFilterInput)
			};
			HttpResponseMessage response = await _httpClient.SendAsync(getRequest);
            var eventsString = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<IEnumerable<EventOutput>>(eventsString)!;
            return events;
        }

        private async Task<HttpResponseMessage> PostAsync(string url, object bodyContent)
        {
            var jsonContent = JsonSerializer.Serialize(bodyContent);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            var errorDetails = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(errorDetails!.detail);
            }
            return response;
        }
    }
}
