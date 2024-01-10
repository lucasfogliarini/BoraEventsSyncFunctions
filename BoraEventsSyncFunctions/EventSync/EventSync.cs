using BoraEventsSyncFunctions.BoraHttp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public abstract class EventSync<TEventSync>
	{
        protected ILogger _logger;

        public EventSync(ILoggerFactory loggerFactory)
        {
			_logger = loggerFactory.CreateLogger<TEventSync>();
		}

		public void InitLog(DateTime startDate, DateTime endDate, string eventsScheduleUrl)
		{
			string message = $"[{GetType().Name}] Criando eventos da semana de '{startDate.ToShortDateString()}' até '{endDate.ToShortDateString()}'. Buscando em '{eventsScheduleUrl}'";
			_logger.LogWarning(message);
		}

		public void LogEvents(IEnumerable<IEventInput> events)
		{
			string message = $"[{GetType().Name}] {events.Count()} eventos rastreados.";
			_logger.LogWarning(message);
		}

		public abstract Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer);
    }
}
