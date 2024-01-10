namespace BoraCrawlers.Tests
{
	public class EventbriteHttpClientTests
	{
		[Fact(Skip = "Precisa o token da api")]
		public async Task ListEventsAsync_Should_Return_Events()
		{
			var organizerId = "24177627927";
			const string EVENTBRITEAPI_DOMAIN = "https://www.eventbriteapi.com/v3/";
			var httpClient = new HttpClient
			{
				BaseAddress = new Uri(EVENTBRITEAPI_DOMAIN),
			};
			//httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "{token}");
			var eventBriteHttpClient = new EventbriteHttpClient(httpClient);

			DateTime startDate = DateTime.Today;

			var events = await eventBriteHttpClient.ListEventsAsync(organizerId);

			Assert.NotEmpty(events);

			var e = events.First(e=>e.Start.Local >= startDate);

			Assert.True(e.Start.Local >= startDate);
			Assert.False(string.IsNullOrEmpty(e.Url));
			Assert.False(string.IsNullOrEmpty(e.Name.Text));
			Assert.False(string.IsNullOrEmpty(e.Summary));
		}
	}
}