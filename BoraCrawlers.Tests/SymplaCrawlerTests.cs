namespace BoraCrawlers.Tests
{
	public class SymplaCrawlerTests
	{
		const string EVENTS_QUERY = "porto-alegre-rs/stand-up-comedy";

		[Fact]
		public async Task CrawlEventsAsync_Should_Return_Events()
		{
			var symplaCrawler = new SymplaCrawler(EVENTS_QUERY);

			DateTime startDate = DateTime.Today;

			var events = await symplaCrawler.CrawlEventsAsync();

			Assert.NotEmpty(events);
			var e = events.First(e => e.DateTime >= startDate);

			Assert.True(e.DateTime >= startDate);
			Assert.False(string.IsNullOrEmpty(e.EventLink));
			Assert.False(string.IsNullOrEmpty(e.Title));
			Assert.False(string.IsNullOrEmpty(e.Location));
		}
	}
}