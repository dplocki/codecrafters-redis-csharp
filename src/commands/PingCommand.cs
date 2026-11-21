namespace codecrafters_redis.src.commands;

public sealed class PingCommand : ICommand
{
    public RespValue Execute(params string[] args)
    {
        return new RespSimpleString("PONG");
    }
}
