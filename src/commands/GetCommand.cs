namespace codecrafters_redis.src.commands;

public sealed class GetCommand(Memory memory) : ICommand
{
    public RespValue Execute(params string[] args)
    {
        if (args.Length != 1)
        {
            return new RespError("ERR wrong number of arguments for 'get' command");
        }

        var value = memory.GetValue(args[0]);
        return value is null
            ? new RespBulkString(null)
            : new RespBulkString(System.Text.Encoding.UTF8.GetBytes(value));
    }
}
