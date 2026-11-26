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
        if (streamId.MillisecondsTime == 0 && streamId.SequenceNumber == 0)
        {
            throw new ArgumentException("The ID specified in XADD must be greater than 0-0");
        }

        if (streamId.MillisecondsTime < lastMillisecondsTime || (streamId.MillisecondsTime == lastMillisecondsTime && streamId.SequenceNumber.HasValue && streamId.SequenceNumber.Value <= lastSequenceNumber))
        {
            throw new ArgumentException("The ID specified in XADD is equal or smaller than the target stream top item");
        }

        lastMillisecondsTime = streamId.MillisecondsTime;

        if (streamId.SequenceNumber == null)
        {
            lastSequenceNumber++;
        }
        else
        {
            lastSequenceNumber = streamId.SequenceNumber.Value;
        }

        lastSequenceNumber = streamId.SequenceNumber!.Value;
        entries[streamId.MillisecondsTime] = fields;

        return new StreamId(lastMillisecondsTime, lastSequenceNumber);
    }
}
