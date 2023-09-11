using BoraEventsSyncFunctions.Crawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
    public class TeatroMinhaEntradaSync : EventSync
    {
        const string EVENTS_QUERY = "?estado=RS&cidade=7994&categoria=4";//cidade: Porto Alegre, categoria: Teatro

		public TeatroMinhaEntradaSync(ILoggerFactory loggerFactory) : base(new MinhaEntradaCrawler(EVENTS_QUERY))
        {
            _logger = loggerFactory.CreateLogger<TeatroMinhaEntradaSync>();
		}

        [Function(nameof(TeatroMinhaEntradaSync))]
		[ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
		public override async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			_boraCrawler.EventsSchedule = $"{_boraCrawler.EventsSchedule}&data-inicio={startDate:yyyy-MM-dd}&data-fim={endDate:yyyy-MM-dd}";

			InitLog(startDate, endDate);

			var events = await _boraCrawler.CrawlEventsAsync();

			LogEvents(events);

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
