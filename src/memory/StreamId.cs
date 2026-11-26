namespace codecrafters_redis.src.memory;

public readonly struct StreamId(int millisecondsTime, int? sequenceNumber)
{
    public int MillisecondsTime { get; } = millisecondsTime;
    public int? SequenceNumber { get; } = sequenceNumber;

    public override string ToString()
    {
        return $"{MillisecondsTime}-{(SequenceNumber.HasValue ? SequenceNumber.Value.ToString() : "*")}";
    }
}
