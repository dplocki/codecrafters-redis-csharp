using codecrafters_redis.src.memory;

namespace codecrafters_redis.src.commands;

public sealed class TypeCommand(Memory memory, StreamsManager streamsManager) : ICommand
{
    public RespValue Execute(params string[] args)
    {
        if (args.Length != 1)
        {
            return new RespError("ERR wrong number of arguments for 'type' command");
        }

        var key = args[0];
        var value = memory.GetType(key);
        if (value is not null) {
            return new RespSimpleString(value.Value.ToString().ToLowerInvariant());
        }
    
        if (streamsManager.IsStream(key))
        {
            return new RespSimpleString(StoredValueTypes.Stream.ToString().ToLowerInvariant());
        }

        return new RespSimpleString("none");
    }
}
