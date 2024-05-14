namespace MaxTalkSharp;

public class MaxResponseParser
{
	public static MaxResponse Parse(string input)
	{
		// {03;FB;19|64:KDY=81;KDL=E2;KDX=E2|04F5}

		input ??= "";

		var isValid = input.StartsWith('{') && input.EndsWith('}') && input.Count(c => c == '|') == 2;

		if (!isValid)
			return MaxResponse.Empty;

		// split into main components
		var parts = input.Trim('{', '}').Split('|');

		// value pairs like KDY=81 and KDL=E2
		var valueParts = parts[1].Replace("64:", "").Split(';');
		var valuePairs = valueParts.Select(value => value.Split('=')).ToArray() ?? [];

		// source and destination
		var sourceAndDestination = parts[0].Split(';');

		return new MaxResponse
		{
			Source = sourceAndDestination[0],
			Destination = sourceAndDestination[1],
			Values = valuePairs.ToDictionary(valuePair => valuePair[0], valuePair => HexToInt(valuePair[1])),
			CheckSum = parts[2]
		};
	}

	private static int HexToInt(string value) => int.Parse(value, System.Globalization.NumberStyles.HexNumber);
}
