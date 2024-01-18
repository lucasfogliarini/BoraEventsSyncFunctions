using BoraCrawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
    public class CircoSymplaSync : CrawlerEventSync<StandUpSymplaSync>
	{
        const string EVENTS_QUERY = "porto-alegre-rs?s=circo";

		public CircoSymplaSync(ILoggerFactory loggerFactory) : base(loggerFactory, new SymplaCrawler(EVENTS_QUERY))
        {
		}

        [Function(nameof(CircoSymplaSync))]
		[ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
		public override async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			var events = await CrawlEventsAsync(startDate, endDate);
			return events;
		}
    }
}
