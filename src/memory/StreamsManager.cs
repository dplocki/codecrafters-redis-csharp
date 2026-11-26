namespace codecrafters_redis.src.memory;

public class StreamsManager
{
    private readonly Dictionary<string, Stream> streams = [];

    public bool IsStream(string key)
    {
        return streams.ContainsKey(key);
    }

    public StreamId Add(string key, StreamId streamId, IDictionary<string, string> fields)
    {
        if (!streams.ContainsKey(key))
        {
            streams[key] = new Stream();
        }

        var stream = streams[key];
        return stream.Add(streamId, fields);
    }
}
