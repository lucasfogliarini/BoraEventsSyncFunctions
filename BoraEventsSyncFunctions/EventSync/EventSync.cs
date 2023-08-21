using BoraEventsSyncFunctions.Crawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public abstract class EventSync
	{
        protected ILogger _logger;
        protected BoraCrawler _boraCrawler;

		public EventSync(BoraCrawler boraCrawler)
        {
			_boraCrawler = boraCrawler;
		}

		public void InitLog(DateTime startDate, DateTime endDate)
		{
			_logger.LogWarning($"[{GetType().Name}] Criando eventos da semana de '{startDate.ToShortDateString()}' até '{endDate.ToShortDateString()}'. Crawling '{_boraCrawler.EventsSchedule}'");
		}

		public abstract Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer);
    }
}
