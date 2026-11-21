namespace codecrafters_redis.src.commands;

public sealed class TypeCommand(Memory memory) : ICommand
{
    public RespValue Execute(params string[] args)
    {
        if (args.Length != 1)
        {
            return new RespError("ERR wrong number of arguments for 'type' command");
        }

        var value = memory.GetType(args[0]);
        return value is null
            ? new RespSimpleString("none")
            : new RespSimpleString(value.Value.ToString().ToLowerInvariant());
    }
}
