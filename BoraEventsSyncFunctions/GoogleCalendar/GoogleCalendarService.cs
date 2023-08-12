using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.ComponentModel.DataAnnotations;

namespace BoraEventsSyncFunctions.GoogleCalendar
{
    public class GoogleCalendarService
    {
        CalendarService _calendarService;
		readonly AccountDataStore _accountDataStore;

		public GoogleCalendarService(AccountDataStore accountDataStore)
        {
            _accountDataStore = accountDataStore;
		}

        public async Task CreateAsync(EventCreated eventCreated)
        {
            ValidateEvent(eventCreated);
            await InitializeCalendarServiceAsync();
            var @event = ToCalendarEvent(eventCreated);
            @event.Start ??= new EventDateTime() { DateTime = DateTime.Now.AddDays(1) };
            @event.End ??= @event.Start;
            var request = _calendarService.Events.Insert(@event, "primary");
            request.ConferenceDataVersion = 1;
            await request.ExecuteAsync();
        }

        private async Task InitializeCalendarServiceAsync()
        {
            var userCredential = await _accountDataStore.GetUserCredentialAsync();

            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential,
            });
        }

        private static void ValidateEvent(EventCreated eventInput)
        {
            if (eventInput.Start.HasValue && eventInput.Start.Value < DateTime.Now)
                throw new ValidationException("O encontro precisa ser maior que agora ...");
        }

        private static Event ToCalendarEvent(EventCreated eventInput)
        {
            var @event = new Event
            {
                Location = eventInput.Location,
                Summary = eventInput.Title,
                Description = eventInput.Description
            };

            if (eventInput.Public != null)
            {
                @event.Visibility = eventInput.Public.Value ? "public" : "private";
            }

            if (eventInput.Start.HasValue)
            {
                var eventStart = eventInput.Start.Value;
                @event.Start = new EventDateTime { DateTime = eventStart };
                @event.End = new EventDateTime { DateTime = eventStart.AddHours(1) };
            }

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
