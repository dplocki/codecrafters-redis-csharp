namespace codecrafters_redis.src.commands;

public interface ICommand
{
    public RespValue Execute(params string[] args);
}