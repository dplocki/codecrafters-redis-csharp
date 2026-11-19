public class Memory
{
    private readonly Dictionary<string, MemoryPice> _store = [];

    public void Set(string key, string value, TimeSpan expiry)
    {
        _store[key] = new MemoryPice(key, value, DateTime.UtcNow, expiry);
    }

    public string? Get(string key)
    {
        if (!_store.TryGetValue(key, out MemoryPice? memoryPice))
        {
            return null;
        }

        if (DateTime.UtcNow - memoryPice.From > memoryPice.Expiry)
        {
            _store.Remove(key);
            return null;
        }

        return memoryPice.Value;
    }
}

internal class MemoryPice(string key, string value, DateTime from, TimeSpan expiry)
{
    public string Key { get; } = key;
    public string Value { get; } = value;
    public DateTime From { get; } = from;
    public TimeSpan Expiry { get; } = expiry;
}
