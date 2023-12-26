using Bora.GoogleCalendar;

namespace BoraEventsSyncFunctions
{
	public class EventCreated : IEventInput
	{
		public string? Title { get; set; }
		public string? Description { get; set; }
		public DateTime? Start { get; set; }
		public DateTime? End { get; set; }
		public string? Location { get; set; }
		public string? ImageUrl { get; set; }
		public string? EventLink { get; set; }
		public GoogleCalendarColor? Color { get; set; }
		public bool? Public { get; set; } = true;
		public bool AddConference { get; set; } = true;
		/// <summary>
		/// Bora Calendar
		/// https://calendar.google.com/calendar/u/0/r/settings/calendar/MmM5ZWI0YTFhZDIwODFkOWZiMzJhY2VlYTg5YjA5OWQ1OTAwMzdkYmYxZDM4ZGExYjU1MGQzNGJjYWFlOWRlZEBncm91cC5jYWxlbmRhci5nb29nbGUuY29t
		/// </summary>
		public string CalendarId { get; set; } = "2c9eb4a1ad2081d9fb32aceea89b099d590037dbf1d38da1b550d34bcaae9ded@group.calendar.google.com";
        public bool CreateReminderTask { get; set; }
    }
}
