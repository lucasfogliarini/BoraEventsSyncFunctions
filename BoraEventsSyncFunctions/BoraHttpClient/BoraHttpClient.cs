using System.Net.Http.Headers;
using System.Text;

namespace BoraEventsSyncFunctions.BoraHttp
{
	public class BoraHttpClient
    {
        readonly HttpClient _httpClient = new();
        const string BORA_API_DOMAIN = "https://bora.azurewebsites.net";

        public BoraHttpClient()
        {
            var jwToken = GetTokenAsync().Result;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwToken.jwToken);
        }

        public async Task PostEventAsync(EventCreated eventCreated)
        {
            var url = $"{BORA_API_DOMAIN}/events?user={eventCreated.BoraUser}";
            await PostAsync(url, eventCreated);
        }

        public async Task<JwToken> GetTokenAsync()
        {
            var authenticationInput = new AuthenticationInput
            {
                Name = "Lucas Fogliarini",
                Email = "lucasfogliarini@gmail.com"
            };
            var url = $"{BORA_API_DOMAIN}/token";
            HttpResponseMessage response = await PostAsync(url, authenticationInput);
            var jwTokenString = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<JwToken>(jwTokenString)!;
        }

        public async Task<IEnumerable<EventOutput>> GetEventsAsync(string user, EventsFilterInput eventsFilterInput)
        {
            var queryParams = eventsFilterInput.ToQueryParamsString();
			var url = $"{BORA_API_DOMAIN}/events?user={user}&{queryParams}";
			HttpResponseMessage response = await _httpClient.GetAsync(url);
            var eventsString = await response.Content.ReadAsStringAsync();
            var events = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<EventOutput>>(eventsString)!;
            return events;
        }

        private async Task<HttpResponseMessage> PostAsync(string url, object bodyContent)
        {
            var jsonContent = System.Text.Json.JsonSerializer.Serialize(bodyContent);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }

    public class JwToken
    {
        public string? jwToken { get; set; }
    }
}
