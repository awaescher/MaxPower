using System.Text;

public static class QueryBuilder
{
    private const int CHECKSUM_LENGTH = 4;

    public static string Build(string source, string destination, string dataSection)
    {
        var body = $"{source};{destination};XX|64:{dataSection}|";
        var bodyLengthInHex = (("{" + body + "}").Length + CHECKSUM_LENGTH).ToString("X2");
        body = body.Replace("XX", bodyLengthInHex);
        var checksum = CalculateChecksum(body);
        return "{" + body + checksum + "}";
    }

    private static string CalculateChecksum(string value)
    {
        return Encoding.UTF8.GetBytes(value)
            .Select(b => (int)b)
            .Sum()
            .ToString("X2")
            .ToUpper()
            .PadLeft(CHECKSUM_LENGTH, '0');
    }
}