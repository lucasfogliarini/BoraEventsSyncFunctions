namespace BoraCrawlers.Tests
{
	public class UhuuCrawlerTests
	{
		const string EVENTS_QUERY = "v/teatro-do-bourbon-country-72";

		[Fact]
		public async Task CrawlEventsAsync_Should_Return_Events()
		{
			var uhuuCrawler = new UhuuCrawler(EVENTS_QUERY);

			DateTime startDate = DateTime.Today;

			var events = await uhuuCrawler.CrawlEventsAsync();

			Assert.NotEmpty(events);
			var e = events.First(e => e.DateTime >= startDate);

			Assert.True(e.DateTime >= startDate);
			Assert.False(string.IsNullOrEmpty(e.EventLink));
			Assert.False(string.IsNullOrEmpty(e.Title));
			Assert.False(string.IsNullOrEmpty(e.Location));
		}
	}
}