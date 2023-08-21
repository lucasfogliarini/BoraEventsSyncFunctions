using BoraEventsSyncFunctions.Crawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
    public class PoaComedyClubSync : EventSync
    {
        const string ORGANIZER = "PoaComedyClub";

		public PoaComedyClubSync(ILoggerFactory loggerFactory) : base(new MinhaEntradaCrawler(ORGANIZER))
        {
            _logger = loggerFactory.CreateLogger<PoaComedyClubSync>();
		}

        [Function(nameof(PoaComedyClubSync))]
		[ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
		public override async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			InitLog(startDate, endDate);

			_boraCrawler.EventsSchedule = $"{_boraCrawler.EventsSchedule}?data-inicio={startDate:yyyy-MM-dd}&data-fim={endDate:yyyy-MM-dd}";

			var events = await _boraCrawler.CrawlEventsAsync();

			return events.Select(e => new EventCreated
			{
				Start = e.DateTime,
				Title = e.Title,
				EventLink = e.EventLink,
				ImageUrl = e.ImageUrl,
				Location = e.Location
			});
		}
	}
}
