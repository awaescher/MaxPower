namespace MaxTalkSharp;

public class MaxValues
{
	public string Source { get; init; } = "";
	public string Destination { get; init; } = "";

	/// <summary>
	/// KDY
	/// </summary>
	public float EnergyDay { get; set; }

	/// <summary>
	/// KMT
	/// </summary>
	public int EnergyMonth { get; set; }

	/// <summary>
	/// KYR
	/// </summary>
	public int EnergyYear { get; set; }

	/// <summary>
	/// KT0
	/// </summary>
	public int EnergyTotal { get; set; }

	public bool IsEmpty => string.IsNullOrEmpty(Source);
}
