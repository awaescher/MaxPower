using MaxTalkSharp;

Inverter[] inverters = [new Inverter("192.168.178.241", 1),
						new Inverter("192.168.178.242", 2),
						new Inverter("192.168.178.243", 3),
						new Inverter("192.168.178.244", 4),
						new Inverter("192.168.178.245", 5)];

foreach (var inverter in inverters)
{
	try
	{
		var response = await MaxTalkClient.RequestAsync(inverter.Ip, 1, 12345);
		Console.WriteLine($"Inverter {inverter.Id}: {response.EnergyDay} day, {response.EnergyMonth} month, {response.EnergyYear} year, {response.EnergyTotal} total");
	}
	catch (TimeoutException)
	{
		// timeouts are expected, inverter may be offline
	}
	catch (Exception ex)
	{
		Console.WriteLine($"ERROR for inverter {inverter.Ip}: {ex.Message}");
	}
}

record Inverter(string Ip, int Id);
