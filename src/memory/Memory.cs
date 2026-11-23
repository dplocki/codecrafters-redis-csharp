namespace codecrafters_redis.src.memory;

public class Memory
{
    private readonly Dictionary<string, MemoryPice> _store = [];

    public void Set(string key, string value, StoredValueTypes type, TimeSpan expiry)
    {
        _store[key] = new MemoryPice(key, value, type, DateTime.UtcNow, expiry);
    }

    public string? GetValue(string key)
    {
        var memoryPice = LoadFromMemory(key);

        return memoryPice?.Value ?? null;
    }

    public StoredValueTypes? GetType(string key)
    {
        var memoryPice = LoadFromMemory(key);

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
