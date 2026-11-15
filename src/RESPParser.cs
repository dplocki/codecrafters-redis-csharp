public class RESPParser
{
    private static readonly byte[] Terminator = "\r\n"u8.ToArray();
    private byte[] input = [];
    private int currentIndex;

    public RespValue Parse(byte[] input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (input.Length == 0)
        {
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));
        }

        this.input = input;
        currentIndex = 0;
        return ParseValue();
    }

    private RespValue ParseValue()
    {
        if (currentIndex >= input.Length)
        {
            throw new FormatException("Unexpected end of RESP input.");
        }

        byte type = input[currentIndex++];
        return type switch
        {
            (byte)'+' => new RespSimpleString(ReadUpToTerminator()),
            (byte)'-' => new RespError(ReadUpToTerminator()),
            (byte)':' => new RespInteger(ParseLong(ReadUpToTerminator())),
            (byte)'$' => ParseBulkString(),
            (byte)'*' => ParseArray(),
            _ => throw new FormatException($"Unknown RESP type byte: {type}."),
        };
    }

    private RespValue ParseBulkString()
    {
        long length = ParseLong(ReadUpToTerminator());
        if (length == -1)
        {
            return new RespBulkString(null);
        }

        if (length < 0 || length > int.MaxValue || currentIndex + length + Terminator.Length > input.Length)
        {
            throw new FormatException("Invalid RESP bulk string length.");
        }

        int byteLength = (int)length;
        var value = input.AsSpan(currentIndex, byteLength).ToArray();
        currentIndex += byteLength;
        ExpectTerminator();
        return new RespBulkString(value);
    }

    private RespValue ParseArray()
    {
        long count = ParseLong(ReadUpToTerminator());
        if (count == -1)
        {
            return new RespArray(null);
        }

        if (count < 0 || count > int.MaxValue)
        {
            throw new FormatException("Invalid RESP array length.");
        }

        var values = new List<RespValue>((int)count);
        for (int i = 0; i < count; i++)
        {
            values.Add(ParseValue());
        }

        return new RespArray(values);
    }

    private string ReadUpToTerminator()
    {
        int terminatorIndex = FindTerminator();
        string line = System.Text.Encoding.UTF8.GetString(input, currentIndex, terminatorIndex - currentIndex);
        currentIndex = terminatorIndex + Terminator.Length;
        return line;
    }

    private int FindTerminator()
    {
        for (int i = currentIndex; i <= input.Length - Terminator.Length; i++)
        {
            if (input[i] == Terminator[0] && input[i + 1] == Terminator[1])
            {
                return i;
            }
        }

        throw new FormatException("RESP terminator \\r\\n was not found.");
    }

    private void ExpectTerminator()
    {
        if (currentIndex > input.Length - Terminator.Length ||
            input[currentIndex] != Terminator[0] ||
            input[currentIndex + 1] != Terminator[1])
        {
            throw new FormatException("Invalid RESP bulk string terminator.");
        }

        currentIndex += Terminator.Length;
    }

    private static long ParseLong(string value)
    {
        if (long.TryParse(value, System.Globalization.NumberStyles.AllowLeadingSign,
                System.Globalization.CultureInfo.InvariantCulture, out long result))
        {
            return result;
        }

        throw new FormatException("Invalid RESP integer format.");
    }
}