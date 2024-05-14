namespace MaxTalkSharp;

public class MaxResponse
{
	public string Source { get; init; } = "";
	public string Destination { get; init; } = "";
	public Dictionary<string, int> Values { get; init; } = [];
	public string CheckSum { get; init; } = "";

	public bool IsEmpty => string.IsNullOrEmpty(CheckSum);

	public static MaxResponse Empty => new();
}
