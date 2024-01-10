using BoraCrawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public class TeatroBourbounCountryUhuuSync : CrawlerEventSync<TeatroBourbounCountryUhuuSync>
    {
		const string EVENTS_QUERY = "v/teatro-do-bourbon-country-72";

        public TeatroBourbounCountryUhuuSync(ILoggerFactory loggerFactory) : base(loggerFactory, new UhuuCrawler(EVENTS_QUERY))
        {
        }

        [Function(nameof(TeatroBourbounCountryUhuuSync))]
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
