namespace codecrafters_redis.src.memory;

internal class Stream()
{
    private readonly IDictionary<int, IDictionary<string, string>> entries = new Dictionary<int, IDictionary<string, string>>();
    private int lastMillisecondsTime = 0;
    private int lastSequenceNumber = 0;

    public int LastMillisecondsTime => lastMillisecondsTime;
    public int LastSequenceNumber => lastSequenceNumber;

    public void Add(int millisecondsTime, int sequenceNumber, IDictionary<string, string> fields)
    {       
        if (millisecondsTime < lastMillisecondsTime || (millisecondsTime == lastMillisecondsTime && sequenceNumber <= lastSequenceNumber))
        {
            throw new ArgumentException("The provided ID is not greater than the last ID in the stream.");
        }

        lastMillisecondsTime = millisecondsTime;
        lastSequenceNumber = sequenceNumber;

        entries[millisecondsTime] = fields;
    }
}