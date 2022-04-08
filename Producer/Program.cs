﻿using System.Globalization;
using System.Net;
using System.Xml.Linq;
using HtmlAgilityPack;
using Producer;

var producer = new ProducerWorker<Feed>();
CultureInfo culture = new("en-US");

List<string> authorIds = new List<string>()
{
    //List Authors here
};

foreach (var author in authorIds)
{
    
    XDocument doc = XDocument.Load("https://www.c-sharpcorner.com/members/" + author + "/rss");
    if (doc == null)
    {
        Console.WriteLine("Bad Bad, very bad");
    }
    var entries = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
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

    for (int i = 0; i < feeds.Count; i++)
    {
        producer.ProduceAsync(feeds[i]).GetAwaiter().GetResult();
    }
}



