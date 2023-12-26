using Bora.GoogleCalendar;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
    public class GoogleCalendarSync
    {
        readonly ILogger _logger;
        readonly GoogleCalendarService _googleCalendarService;

        public GoogleCalendarSync(ILoggerFactory loggerFactory, GoogleCalendarService googleCalendarService)
        {
            _logger = loggerFactory.CreateLogger<GoogleCalendarSync>();
            _googleCalendarService = googleCalendarService;
		}

        [Function(nameof(GoogleCalendarSync))]
        public async Task RunAsync([ServiceBusTrigger("%AzureServiceBusEventCreatedQueue%",
                                            Connection = "AzureServiceBusConnectionString")]
                                            EventCreated eventCreated)
        {
            string gmail = "lucasfogliarini@gmail.com";
            await _googleCalendarService.InitializeCalendarServiceAsync(gmail);

			_logger.LogWarning($"[{GetType().Name}] Sincronizando evento '{eventCreated.Title}' ({eventCreated.Start}).");
            var events = await _googleCalendarService.ListEventsAsync(eventCreated.CalendarId, eventCreated.EventLink);
            var alreadyCreated = events.Items.Count > 0;
			if (alreadyCreated)
            {
                _logger.LogWarning($"Esse evento não será criado, pois já existe um com o mesmo link no seu calendário. ${eventCreated.EventLink}");
				return;
            }

            eventCreated.Description += $"\n {eventCreated.EventLink}";
            await _googleCalendarService.CreateAsync(eventCreated);
            _logger.LogInformation($"Evento criado, '{eventCreated.EventLink}'.");
		}
	}
}
