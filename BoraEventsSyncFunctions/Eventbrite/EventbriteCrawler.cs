using AngleSharp.Dom;
using AngleSharp;
using System.Globalization;

namespace BoraEventsSyncFunctions.Eventbrite
{
	public class EventbriteCrawler
	{
		const string EVENTBRITE_DOMAIN = "https://www.eventbrite.com.br/";
        public string OrganizerSchedule { get; private set; }

		/// <summary>
		/// A crawler class for extracting event information from a 'https://www.eventbrite.com.br/'
		/// </summary>
		/// <param name="eventOrganizer">https://www.eventbrite.com.br/o/{eventOrganizer}</param>
		public EventbriteCrawler(string eventOrganizer)
        {
			OrganizerSchedule = $"{EVENTBRITE_DOMAIN}/o/{eventOrganizer}/";
		}
        public async Task<List<EventbriteEvent>> CrawlEventsAsync()
		{
			var document = await GetHtmlDocumentAsync(OrganizerSchedule);
			return ExtractEvents(document);
		}

		private static async Task<IDocument> GetHtmlDocumentAsync(string scheduleUrl)
		{
			var config = Configuration.Default.WithDefaultLoader();
			var context = BrowsingContext.New(config);

			using (var httpClient = new HttpClient())
			{
				var html = await httpClient.GetStringAsync(scheduleUrl);
				return await context.OpenAsync(req => req.Content(html));
			}
		}

		private static List<EventbriteEvent> ExtractEvents(IDocument document)
		{
			//var selector = "div[data-testid='organizer-profile__future-events']";
			var selector = "div[data-testid='organizer-profile__future-events'] .eds-show-up-mn .eds-event-card--consumer";
			var events = document.QuerySelectorAll(selector);

			List<EventbriteEvent> eventList = new();

			foreach (var eventElement in events)
			{
				var title = eventElement.QuerySelector("h3 .eds-event-card__formatted-name--is-clamped")?.TextContent.Trim();
				var dateTimeString = eventElement.QuerySelector(".eds-event-card-content__sub-title")?.TextContent.Trim();
				var eventLocation = eventElement.QuerySelector(".card-text--truncated__one")?.TextContent.Trim();
				var eventPrice = eventElement.QuerySelector(".eds-event-card-content__sub:nth-child(2)")?.TextContent.Trim();
				var eventLink = eventElement.QuerySelector("a.eds-event-card-content__action-link")?.GetAttribute("href");

				var eventData = new EventbriteEvent
				{
					Title = ParseCustomTitle(title),
					DateTime = ParseCustomDateTime(dateTimeString),
					Location = "❤️‍🔥 Valen Bar",
					EventLink = eventLink,
					Price = eventPrice,
				};

				eventList.Add(eventData);
			}

			return eventList;
		}

		static DateTime? ParseCustomDateTime(string dateTimeString)
		{
			string[] dateParts = dateTimeString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (dateParts.Length == 4)
			{
				Dictionary<string, string> monthMap = new()
				{
					{ "jan", "Jan" },
					{ "fev", "Feb" },
					{ "mar", "Mar" },
					{ "abr", "Apr" },
					{ "mai", "May" },
					{ "jun", "Jun" },
					{ "jul", "Jul" },
					{ "ago", "Aug" },
					{ "set", "Sep" },
					{ "out", "Oct" },
					{ "nov", "Nov" },
					{ "dez", "Dec" }
				};

				string formattedDate = $"{monthMap[dateParts[1]]} {dateParts[2]} {dateParts[3]}";
				string format = "MMM d, HH:mm";

				if (DateTime.TryParseExact(formattedDate, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime dateTime))
				{
					return dateTime;
				}
			}

			return null;
		}

		static string ParseCustomTitle(string title)
		{
			return title.Contains("VALEN") ? title.Substring(23) : title;
		}
	}
}
