using AngleSharp.Dom;

namespace BoraCrawlers
{
    public class EventbriteCrawler : BoraCrawler
    {
        const string EVENTBRITE_DOMAIN = "https://www.eventbrite.com.br";

        /// <summary>
        /// A crawler class for extracting event information from a 'https://www.eventbrite.com.br/'
        /// </summary>
        /// <param name="eventOrganizer">https://www.eventbrite.com.br/o/{eventOrganizer}</param>
        public EventbriteCrawler(string eventOrganizer)
        {
            EventsSchedule = $"{EVENTBRITE_DOMAIN}/o/{eventOrganizer}/";
        }
        protected override List<CrawledEvent> ExtractEvents(IDocument document)
        {
            var selector = "div[data-testid='organizer-profile__future-events'] .eds-show-up-mn .eds-event-card--consumer";
            var events = document.QuerySelectorAll(selector);

            List<CrawledEvent> eventList = new();

            foreach (var eventElement in events)
            {
                var title = eventElement.QuerySelector("h3 .eds-event-card__formatted-name--is-clamped")?.TextContent.Trim();
                var dateTimeString = eventElement.QuerySelector(".eds-event-card-content__sub-title")?.TextContent.Trim();
                var eventLocation = eventElement.QuerySelector(".card-text--truncated__one")?.TextContent.Trim();
                var eventPrice = eventElement.QuerySelector(".eds-event-card-content__sub:nth-child(2)")?.TextContent.Trim();
                var eventLink = eventElement.QuerySelector("a.eds-event-card-content__action-link")?.GetAttribute("href");

                var eventData = new CrawledEvent
				{
                    Title = ParseCustomTitle(title),
                    Location = "❤️‍🔥 Valen Bar",
                    EventLink = eventLink,
                    Price = eventPrice,
                };

                if (dateTimeString != null)
                {
                    string[] dateParts = dateTimeString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dateParts.Length != 4)
                    {
                        continue;
                    }
                    string d = dateParts[2].Replace(",", "");
                    string MMMBr = dateParts[1];
                    string HHmm = dateParts[3];
                    eventData.DateTime = ParseDateTime(d, MMMBr, HHmm);
                }

                eventList.Add(eventData);
            }

            return eventList;
        }
        static string ParseCustomTitle(string title)
        {
            return title.Contains("VALEN") ? title.Substring(23) : title;
        }
    }
}
