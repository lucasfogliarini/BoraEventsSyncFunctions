using AngleSharp.Dom;

namespace BoraEventsSyncFunctions.Crawlers
{
	public class SymplaCrawler : BoraCrawler
    {
        const string SYMPLA_DOMAIN = "https://www.sympla.com.br/";

        /// <summary>
        /// A crawler class for extracting event information from a 'https://www.sympla.com.br/'
        /// </summary>
        /// <param name="eventsQuery">https://www.sympla.com.br/eventos/{eventsQuery}</param>
        public SymplaCrawler(string eventsQuery)
        {
            EventsSchedule = $"{SYMPLA_DOMAIN}eventos/{eventsQuery}";
        }

        protected override List<CrawledEvent> ExtractEvents(IDocument document)
        {
            var events = document.QuerySelectorAll(".sympla-card");

            List<CrawledEvent> eventList = new();

            foreach (var eventNode in events)
            {
                var title = eventNode.QuerySelector("[class^='EventCardstyle__EventTitle']")?.TextContent;
                var dateTimeString = eventNode.QuerySelector("[class^='EventCardstyle__EventDate']")?.TextContent;
                var location = eventNode.QuerySelector("[class^='EventCardstyle__EventLocation']")?.TextContent;
                var eventLink = eventNode.GetAttribute("href");

                var eventData = new CrawledEvent
                {
                    Title = title,
                    Location = location,
                    EventLink = eventLink
                };

                if (dateTimeString != null)
                {
                    string[] dateParts = dateTimeString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dateParts.Length != 5)
                    {
                        continue;
                    }
                    string d = dateParts[1];
                    string MMMBr = dateParts[2];
                    string HHmm = dateParts[4];
                    eventData.DateTime = ParseDateTime(d, MMMBr, HHmm);
                }

                eventList.Add(eventData);
            }

            return eventList;
        }
    }
}
