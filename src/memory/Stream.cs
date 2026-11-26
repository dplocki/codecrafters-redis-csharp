namespace codecrafters_redis.src.memory;

internal class Stream()
{
    private readonly IDictionary<long, IDictionary<string, string>> entries = new Dictionary<long, IDictionary<string, string>>();
    private long lastMillisecondsTime = 0;
    private long lastSequenceNumber = 0;

    public long LastMillisecondsTime => lastMillisecondsTime;
    public long LastSequenceNumber => lastSequenceNumber;

    public StreamId Add(StreamId streamId, IDictionary<string, string> fields)
    {
        var newMillisecondsTime = streamId.MillisecondsTime;
        long newSequenceNumber;

        if (!streamId.SequenceNumber.HasValue)
        {
            if (entries.ContainsKey(newMillisecondsTime))
            {
                newSequenceNumber = 0;
            }
            else
            {
                newSequenceNumber = lastSequenceNumber + 1;
            }
        }
        else
        {
            newSequenceNumber = streamId.SequenceNumber.Value;
        }

        if (newMillisecondsTime == 0 && newSequenceNumber == 0)
        {
            throw new ArgumentException("The ID specified in XADD must be greater than 0-0");
        }

        if (newMillisecondsTime < lastMillisecondsTime
            || (newMillisecondsTime == lastMillisecondsTime && newSequenceNumber <= lastSequenceNumber))
        {
            throw new ArgumentException("The ID specified in XADD is equal or smaller than the target stream top item");
        }

        lastMillisecondsTime = newMillisecondsTime;
        lastSequenceNumber = newSequenceNumber;

        entries[newMillisecondsTime] = fields;

        return new StreamId(newMillisecondsTime, newSequenceNumber);
    }
}
