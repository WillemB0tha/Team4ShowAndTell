using System.Text.Json;

namespace Producer;

public class Feed
{
    public string Link { get; set; }
    public string Title { get; set; }
    public string FeedType { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
    public DateTime PubDate { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}