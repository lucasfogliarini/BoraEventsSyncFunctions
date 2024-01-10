using BoraCrawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public class AraujoViannaUhuuSync : CrawlerEventSync<AraujoViannaUhuuSync>
    {
		const string EVENTS_QUERY = "v/araujovianna-99";

        public AraujoViannaUhuuSync(ILoggerFactory loggerFactory) : base(loggerFactory, new UhuuCrawler(EVENTS_QUERY))
        {
        }

        [Function(nameof(AraujoViannaUhuuSync))]
        [ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
        public override async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddMonths(6);

			var events = await CrawlEventsAsync(startDate, endDate);
			return events;

		}
    }
}
