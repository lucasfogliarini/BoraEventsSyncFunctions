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
            _logger.LogWarning($"[{GetType().Name}] Sincronizando evento '{eventCreated.Title}' ({eventCreated.Start}).");

            eventCreated.Description += $"\n {eventCreated.EventLink}";
            string gmail = "lucasfogliarini@gmail.com";
            await _googleCalendarService.CreateAsync(gmail, eventCreated);
        }

	}
}
