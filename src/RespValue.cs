public abstract record RespValue;

public sealed record RespSimpleString(string Value) : RespValue;

public sealed record RespInteger(long Value) : RespValue;

public sealed record RespBulkString(byte[]? Value) : RespValue;

public sealed record RespArray(IReadOnlyList<RespValue>? Values) : RespValue;

public sealed record RespError(string Value) : RespValue;