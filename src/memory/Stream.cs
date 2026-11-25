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
            throw new ArgumentException($"ERR The ID specified in XADD must be greater than ${lastMillisecondsTime}-{lastSequenceNumber}");
        } 

        lastMillisecondsTime = millisecondsTime;
        lastSequenceNumber = sequenceNumber;

        entries[millisecondsTime] = fields;
    }
}