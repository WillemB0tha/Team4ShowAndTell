using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Xml.Linq;
using Confluent.Kafka;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Producer;

var kafkaproducer = new KafkaProducerWorker<Feed>();
var mqueueproducer = new MQueuProducerWorker<Feed>();

CultureInfo culture = new("en-US");

List<string> authorIds = new List<string>()
{
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "raj-kumar-beniwal",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit",
    "sarath-lal7",
    "mahesh-chand",
    "vijai-anand-ramalingam",
    "vulpes",
    "manpreet-singh12",
    "jignesh-trivedi",
    "debasis-saha",
    "rohatash-kumar",
    "nitin-pandit"
};

Parallel.ForEach(authorIds, new ParallelOptions { MaxDegreeOfParallelism = 1000 }, author =>
{
    XDocument authorDoc = XDocument.Load("https://www.c-sharpcorner.com/members/" + author + "/rss");
   

    var entries = from item in authorDoc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                  select new Feed
                  {
                      Content = item.Elements().First(i => i.Name.LocalName == "description").Value,
                      Link = (item.Elements().First(i => i.Name.LocalName == "link").Value).StartsWith("/") ? "https://www.c-sharpcorner.com" + item.Elements().First(i => i.Name.LocalName == "link").Value : item.Elements().First(i => i.Name.LocalName == "link").Value,
                      PubDate = Convert.ToDateTime(item.Elements().First(i => i.Name.LocalName == "pubDate").Value, culture),
                      Title = item.Elements().First(i => i.Name.LocalName == "title").Value,
                      FeedType = (item.Elements().First(i => i.Name.LocalName == "link").Value).ToLowerInvariant().Contains("blog") ? "Blog" : (item.Elements().First(i => i.Name.LocalName == "link").Value).ToLowerInvariant().Contains("news") ? "News" : "Article",
                      Author = item.Elements().First(i => i.Name.LocalName == "author").Value
                  };

    List<Feed> feeds = entries.OrderByDescending(o => o.PubDate).ToList();

    Console.WriteLine("Feeds Found, {0} for Author {1}", feeds.Count, author); 

    for (int i = 0; i < feeds.Count; i++)
    {
        try
        {

            var report = kafkaproducer.ProduceAsync(feeds[i]).GetAwaiter().GetResult();
            mqueueproducer.ProduceAsync(feeds[i]).GetAwaiter().GetResult();
            Console.WriteLine("Produced: {0}", report.Value);

        }
        catch (ProduceException<long, string> e)
        {
            Console.WriteLine($"Permanent error: {e.Message} for message (value: '{e.DeliveryResult.Value}')");
            Console.WriteLine("Exiting producer...");
        }
    }
});



