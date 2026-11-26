namespace codecrafters_redis.src.memory;

public readonly struct StreamId(long millisecondsTime, long? sequenceNumber)
{
    public long MillisecondsTime { get; } = millisecondsTime;
    public long? SequenceNumber { get; } = sequenceNumber;

    public override string ToString()
    {
        return $"{MillisecondsTime}-{(SequenceNumber.HasValue ? SequenceNumber.Value.ToString() : "*")}";
    }
}
