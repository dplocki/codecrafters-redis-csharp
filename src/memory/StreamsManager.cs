namespace codecrafters_redis.src.memory;

public class StreamsManager
{
    private readonly Dictionary<string, IDictionary<string, IDictionary<string, string>>> streams = [];

    public bool IsStream(string key)
    {
        return streams.ContainsKey(key);
    }

    public void Add(string key, string id, Dictionary<string, string> fields)
    {
        if (!streams.ContainsKey(key))
        {
            streams[key] = new Dictionary<string, IDictionary<string, string>>();
        }

        var streamEntries = streams[key];
        streamEntries[id] = fields;
    }
}
