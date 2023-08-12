using System.Net.Http.Headers;
using System.Text;

namespace BoraEventsSyncFunctions
{
	internal class BoraHttpClient
	{
		readonly HttpClient _httpClient = new();
		const string BORA_API_DOMAIN = "https://bora.azurewebsites.net";

        public BoraHttpClient()
        {
			var jwToken = GetTokenAsync().Result;
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwToken.jwToken);
		}

        public async Task PostEventAsync(EventCreated eventCreated, string user = "lucasfogliarini")
		{
			var url = $"{BORA_API_DOMAIN}/events?user={user}";
			await PostAsync(url, eventCreated);
		}

		public async Task<JwToken> GetTokenAsync()
		{
			var account = new AuthenticationInput
			{
				Name = "Lucas Fogliarini",
				Email = "lucasfogliarini@gmail.com"
			};
			var url = $"{BORA_API_DOMAIN}/token";
			HttpResponseMessage response = await PostAsync(url, account);
			var jwTokenString = await response.Content.ReadAsStringAsync();
			return System.Text.Json.JsonSerializer.Deserialize<JwToken>(jwTokenString)!;
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
	internal class AuthenticationInput
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string PhotoUrl { get; set; } = "na";
		public string Provider { get; set; } = "BORA_SYNC";
	}

	internal class JwToken
	{
		public string? jwToken { get; set; }
	}
}
