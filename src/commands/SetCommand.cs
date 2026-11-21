namespace codecrafters_redis.src.commands;

public sealed class SetCommand(Memory memory) : ICommand
{
    public string Name => "SET";

    public RespValue Execute(params string[] args)
    {
        if (args.Length < 2)
        {
            return new RespError("ERR wrong number of arguments for 'set' command");
        }

        TimeSpan expiry;

        if (args.Length >= 4)
        {
            if (args[2].Equals("PX", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(args[3], out int milliseconds) || milliseconds < 0)
                {
                    return new RespError("ERR invalid PX value");
                }

                expiry = TimeSpan.FromMilliseconds(milliseconds);
            }
            else if (args[2].Equals("EX", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(args[3], out int seconds) || seconds < 0)
                {
                    return new RespError("ERR invalid EX value");
                }

                expiry = TimeSpan.FromSeconds(seconds);
            }
            else
            {
                return new RespError("ERR unknown option");
            }
        }
        else if (args.Length == 2)
        {
            expiry = TimeSpan.FromMilliseconds(100);
        }
        else
        {
            return new RespError("ERR syntax error");
        }

        memory.Set(args[0], args[1], StoredValueTypes.String, expiry);
        return new RespSimpleString("OK");
    }
}
