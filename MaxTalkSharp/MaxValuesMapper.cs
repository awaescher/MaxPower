namespace MaxTalkSharp;

public static class MaxValuesMapper
{
	public static MaxValues Map(MaxResponse response)
	{
		return new MaxValues
		{
			Source = response.Source,
			Destination = response.Destination,
			EnergyDay = response.Values["KDY"] * 0.1f,
			EnergyMonth = response.Values["KMT"],
			EnergyYear = response.Values["KYR"],
			EnergyTotal = response.Values["KT0"]
		};
	}
}