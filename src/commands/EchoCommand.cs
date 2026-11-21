namespace codecrafters_redis.src.commands;

public sealed class EchoCommand : ICommand
{
    public string Name => "ECHO";

    public RespValue Execute(params string[] args)
    {
        if (args.Length != 1)
        {
            return new RespError("ERR wrong number of arguments for 'echo' command");
        }

        return new RespBulkString(System.Text.Encoding.UTF8.GetBytes(args[0]));
    }
}
