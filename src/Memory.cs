public class Memory
{
    private readonly Dictionary<string, MemoryPice> _store = [];

    public void Set(string key, string value, StoredValueTypes type, TimeSpan expiry)
    {
        _store[key] = new MemoryPice(key, value, type, DateTime.UtcNow, expiry);
    }

    public string? GetValue(string key)
    {
        MemoryPice? memoryPice = LoadFromMemory(key);

        return memoryPice?.Value ?? null;
    }

    public StoredValueTypes? GetType(string key)
    {
        MemoryPice? memoryPice = LoadFromMemory(key);

        return memoryPice?.Type ?? null;
    }

    private MemoryPice? LoadFromMemory(string key)
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

        return memoryPice;
    }
}

internal class MemoryPice(string key, string value, StoredValueTypes type, DateTime from, TimeSpan expiry)
{
    public string Key { get; } = key;
    public string Value { get; } = value;
    public StoredValueTypes Type { get; } = type;
    public DateTime From { get; } = from;
    public TimeSpan Expiry { get; } = expiry;
}
