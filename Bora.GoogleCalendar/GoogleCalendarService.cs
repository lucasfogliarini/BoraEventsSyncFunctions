using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Tasks.v1;
using Google.Apis.Services;
using System.ComponentModel.DataAnnotations;

namespace Bora.GoogleCalendar
{
	public class GoogleCalendarService
    {
        CalendarService _calendarService = null;
        TasksService _taskService = null;
        readonly GoogleDataStore _googleDataStore;

        public GoogleCalendarService(GoogleDataStore googleDataStore)
        {
            _googleDataStore = googleDataStore;
		}

        public async Task InitializeCalendarServiceAsync(string gmail)
        {
            var userCredential = await _googleDataStore.GetUserCredentialAsync(gmail);

            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential,
            });

            _taskService = new TasksService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential
            });
        }
        public async Task<Event> CreateAsync(IEventInput eventInput)
        {
            ValidateEvent(eventInput);
            var @event = ToGoogleEvent(eventInput);
			var request = _calendarService.Events.Insert(@event, eventInput.CalendarId);
            request.ConferenceDataVersion = 1;
            var gEvent = await request.ExecuteAsync();

            if (eventInput.CreateReminderTask)
            {
                var taskList = "MDExMTAxNTQ4OTU1MzE4NzMxMDI6MDow";//Tarefas
                var reminderTask = new Google.Apis.Tasks.v1.Data.Task
                {
                    Due = DateTime.Now.ToString("O"),                    
                    Title = $"{eventInput.Title} - {eventInput.Start.Value.ToShortDateString()}"
				};
                await _taskService.Tasks.Insert(reminderTask, taskList).ExecuteAsync();
			}

            return gEvent;
        }
        public async Task<Events> ListEventsAsync(string calendarId, string? search = null)
        {
			var eventsRequest = _calendarService.Events.List(calendarId);
			eventsRequest.Q = search;
			var events = await eventsRequest.ExecuteAsync();
            return events;
		}

		private static void ValidateEvent(IEventInput eventInput)
        {
            if (eventInput.Start.HasValue && eventInput.Start.Value < DateTime.Now)
                throw new ValidationException("O encontro precisa ser maior que agora ...");
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
