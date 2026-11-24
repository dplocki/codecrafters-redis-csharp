namespace codecrafters_redis.src.memory;

public class StreamsManager
{
    private readonly Dictionary<string, Stream> streams = [];

    public bool IsStream(string key)
    {
        return streams.ContainsKey(key);
    }

    public void Add(string key, int millisecondsTime, int sequenceNumber, IDictionary<string, string> fields)
    {
        if (!streams.ContainsKey(key))
        {
            streams[key] = new Stream();
        }

        var stream = streams[key];
        stream.Add(millisecondsTime, sequenceNumber, fields);
    }
}
