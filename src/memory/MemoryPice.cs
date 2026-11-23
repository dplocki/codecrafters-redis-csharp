namespace codecrafters_redis.src.memory;

internal class MemoryPice(string key, string value, StoredValueTypes type, DateTime from, TimeSpan expiry)
{
    public string Key { get; } = key;
    public string Value { get; } = value;
    public StoredValueTypes Type { get; } = type;
    public DateTime From { get; } = from;
    public TimeSpan Expiry { get; } = expiry;
}
