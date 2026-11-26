namespace codecrafters_redis.src.memory;

internal class Stream()
{
    private readonly IDictionary<int, IDictionary<string, string>> entries = new Dictionary<int, IDictionary<string, string>>();
    private int lastMillisecondsTime = 0;
    private int lastSequenceNumber = 0;

    public int LastMillisecondsTime => lastMillisecondsTime;
    public int LastSequenceNumber => lastSequenceNumber;

    public StreamId Add(StreamId streamId, IDictionary<string, string> fields)
    {
        var newMillisecondsTime = streamId.MillisecondsTime;
        var newSequenceNumber = streamId.SequenceNumber ?? (lastSequenceNumber + 1);

        if (newMillisecondsTime == 0 && newSequenceNumber == 0)
        {
            throw new ArgumentException("The ID specified in XADD must be greater than 0-0");
        }

        if (newMillisecondsTime < lastMillisecondsTime || (newMillisecondsTime == lastMillisecondsTime && newSequenceNumber <= lastSequenceNumber))
        {
            throw new ArgumentException("The ID specified in XADD is equal or smaller than the target stream top item");
        }

        lastMillisecondsTime = newMillisecondsTime;
        lastSequenceNumber = newSequenceNumber;

        entries[newMillisecondsTime] = fields;

        return new StreamId(newMillisecondsTime, newSequenceNumber);
    }
}
