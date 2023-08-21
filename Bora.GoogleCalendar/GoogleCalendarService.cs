using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SendUpdatesEnum = Google.Apis.Calendar.v3.EventsResource.PatchRequest.SendUpdatesEnum;

namespace Bora.GoogleCalendar
{
	public class GoogleCalendarService
    {
        CalendarService _calendarService = null;
        readonly GoogleDataStore _googleDataStore;

        public GoogleCalendarService(GoogleDataStore googleDataStore)
        {
            _googleDataStore = googleDataStore;
		}

        public async Task CreateAsync(string gmail, IEventInput eventInput)
        {
            ValidateEvent(eventInput);
            await InitializeCalendarServiceAsync(gmail);
            var @event = ToGoogleEvent(eventInput);
            var request = _calendarService.Events.Insert(@event, eventInput.CalendarId);
            request.ConferenceDataVersion = 1;
            var gEvent = await request.ExecuteAsync();
        }

		private static void ValidateEvent(IEventInput eventInput)
        {
            if (eventInput.Start.HasValue && eventInput.Start.Value < DateTime.Now)
                throw new ValidationException("O encontro precisa ser maior que agora ...");
        }
        private async Task InitializeCalendarServiceAsync(string gmail)
        {
            var userCredential = await _googleDataStore.GetUserCredentialAsync(gmail);

            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential,
            });
        }
        private static Event ToGoogleEvent(IEventInput eventInput)
        {
            var @event = new Event
            {
                Location = eventInput.Location,
                Summary = eventInput.Title,
                Description = eventInput.Description,
                
            };

            if (eventInput.Color.HasValue)
			{
                @event.ColorId = ((int)eventInput.Color).ToString();
			}

            if (eventInput.Public != null)
            {
                @event.Visibility = eventInput.Public.Value ? "public" : "private";
            }

			eventInput.Start ??= DateTime.Now.AddDays(1);
			var eventStart = eventInput.Start.Value;
			eventInput.End ??= eventStart.AddHours(1);

			@event.Start = new EventDateTime { DateTimeDateTimeOffset = eventStart };
			@event.End = new EventDateTime { DateTimeDateTimeOffset = eventInput.End };

			if (eventInput.AddConference)
            {
                AddGoogleMeet(@event);
            }

            return @event;
        }
        private static void AddGoogleMeet(Event @event)
        {
            @event.ConferenceData = new ConferenceData
            {
                CreateRequest = new CreateConferenceRequest
                {
                    RequestId = $"{@event.Summary}-{@event.Start?.DateTime}",
                    ConferenceSolutionKey = new ConferenceSolutionKey
                    {
                        Type = "hangoutsMeet"
                    }
                }
            };
        }
    }
}
