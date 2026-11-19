public class Memory
{
    private readonly Dictionary<string, string> _store = [];

    public void Set(string key, string value)
    {
        _store[key] = value;
    }

    public string? Get(string key)
    {
        if (_store.TryGetValue(key, out string? value))
        {
            return value;
        }

        return null;
    }
}