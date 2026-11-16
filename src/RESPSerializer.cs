using System.Globalization;
using System.Text;

public static class RESPSerializer
{
    private static readonly byte[] Terminator = "\r\n"u8.ToArray();

    public static byte[] Serialize(RespValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        using var output = new MemoryStream();
        WriteValue(output, value);
        return output.ToArray();
    }

    private static void WriteValue(Stream output, RespValue value)
    {
        switch (value)
        {
            case RespSimpleString simpleString:
                WriteLine(output, (byte)'+', simpleString.Value);
                break;
            case RespError error:
                WriteLine(output, (byte)'-', error.Value);
                break;
            case RespInteger integer:
                WriteLine(output, (byte)':', integer.Value.ToString(CultureInfo.InvariantCulture));
                break;
            case RespBulkString { Value: null }:
                WriteLine(output, (byte)'$', "-1");
                break;
            case RespBulkString bulkString:
                WriteLength(output, (byte)'$', bulkString.Value!.Length);
                output.Write(bulkString.Value);
                output.Write(Terminator);
                break;
            case RespArray { Values: null }:
                WriteLine(output, (byte)'*', "-1");
                break;
            case RespArray array:
                WriteLength(output, (byte)'*', array.Values!.Count);
                foreach (RespValue item in array.Values)
                {
                    WriteValue(output, item);
                }
                break;
            default:
                throw new NotSupportedException($"Unsupported RESP value type: {value.GetType().Name}.");
        }
    }

    private static void WriteLine(Stream output, byte prefix, string value)
    {
        if (value.IndexOfAny(['\r', '\n']) >= 0)
        {
            throw new ArgumentException("RESP line values cannot contain CR or LF.", nameof(value));
        }

        output.WriteByte(prefix);
        output.Write(Encoding.UTF8.GetBytes(value));
        output.Write(Terminator);
    }

    private static void WriteLength(Stream output, byte prefix, int length)
    {
        output.WriteByte(prefix);
        output.Write(Encoding.ASCII.GetBytes(length.ToString(CultureInfo.InvariantCulture)));
        output.Write(Terminator);
    }
}