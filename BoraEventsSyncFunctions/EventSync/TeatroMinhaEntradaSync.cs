using BoraCrawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
    public class TeatroMinhaEntradaSync : CrawlerEventSync<TeatroMinhaEntradaSync>
	{
        const string EVENTS_QUERY = "?estado=RS&cidade=7994&categoria=4";//cidade: Porto Alegre, categoria: Teatro

		public TeatroMinhaEntradaSync(ILoggerFactory loggerFactory) : base(loggerFactory, new MinhaEntradaCrawler(EVENTS_QUERY))
        {
		}

        [Function(nameof(TeatroMinhaEntradaSync))]
		[ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
		public override async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			BoraCrawler.EventsSchedule = $"{BoraCrawler.EventsSchedule}&data-inicio={startDate:yyyy-MM-dd}&data-fim={endDate:yyyy-MM-dd}";

			var events = await CrawlEventsAsync(startDate, endDate);
			return events;
		}
	}
}
