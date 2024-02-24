using BoraEventsSyncFunctions.BoraHttp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public class GoogleCalendarSync
    {
        readonly ILogger _logger;
        readonly BoraHttpClient _boraHttpClient;

		public GoogleCalendarSync(ILoggerFactory loggerFactory, BoraHttpClient boraHttpClient)
        {
            _logger = loggerFactory.CreateLogger<GoogleCalendarSync>();
            _boraHttpClient = boraHttpClient;
		}

        [Function(nameof(GoogleCalendarSync))]
        public async Task RunAsync([ServiceBusTrigger("%AzureServiceBusEventCreatedQueue%",
                                            Connection = "AzureServiceBusConnectionString")]
                                            EventCreated eventCreated)
        {
            try
            {
				_logger.LogWarning($"[{GetType().Name}] Sincronizando evento '{eventCreated.Title}' ({eventCreated.Start}).");
				var eventsFilter = new EventsFilterInput
				{
					CalendarId = eventCreated.CalendarId,
					Query = eventCreated.EventLink,
					TimeMax = DateTime.Now.AddYears(1)
				};
				var events = await _boraHttpClient.GetEventsAsync(eventCreated.BoraUser, eventsFilter);
				var alreadyCreated = events.Any();
				if (alreadyCreated)
				{
					_logger.LogWarning($"Esse evento não será criado, pois já existe um evento com o mesmo link no seu calendário. '{eventCreated.EventLink}'");
					return;
				}

				eventCreated.Description += $"\n {eventCreated.EventLink}";
				eventCreated.CreateReminderTask = eventCreated.Start > DateTime.Today.AddDays(10);
				await _boraHttpClient.PostEventAsync(eventCreated);
				_logger.LogInformation($"Evento criado, '{eventCreated.EventLink}'.");
			}
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
				throw;
            }
		}
	}
}
