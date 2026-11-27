using codecrafters_redis.src.commands;
using codecrafters_redis.src.memory;

public class XAddCommand(StreamsManager streamsManager) : ICommand
{
    public RespValue Execute(params string[] args)
    {
        if (args.Length < 3)
        {
            return new RespError("ERR wrong number of arguments for 'XADD' command");
        }

        var streamKey = args[0];
        var streamId = ParseId(args[1]);

        var fieldValues = args.Skip(2).ToArray();
        if (fieldValues.Length % 2 != 0)
        {
            return new RespError("ERR wrong number of field-value pairs");
        }

        var fields = new Dictionary<string, string>();
        for (int i = 0; i < fieldValues.Length; i += 2)
        {
            fields[fieldValues[i]] = fieldValues[i + 1];
        }

        try
        {
            var addedStreamId = streamsManager.Add(streamKey, streamId, fields);
            return new RespBulkString(System.Text.Encoding.UTF8.GetBytes(addedStreamId.ToString()));
        }
        catch (Exception ex)
        {
            return new RespError($"ERR {ex.Message}");
        }
    }

    private StreamId ParseId(string rawId) {
        if (rawId == "*") {
            return new StreamId(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), null);
        }

        var idTokens = rawId.Split('-');
        var millisecondsTime = long.Parse(idTokens[0]);
        var sequenceNumber = idTokens[1] == "*" ? null : (long?)long.Parse(idTokens[1]);

        return new StreamId(millisecondsTime, sequenceNumber);
    }
}