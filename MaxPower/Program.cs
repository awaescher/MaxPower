using Prometheus;
using System.Reflection;
using Serilog;
using MaxPower.HostedServices;
using MaxTalkSharp;

namespace MaxPower;

public class Program
{
	public static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.WriteTo.Debug()
			.CreateLogger();

		Log.Logger.Information($"Booting MaxPower {Assembly.GetExecutingAssembly().GetName().Version} ...");

		var builder = WebApplication.CreateBuilder(args);

		var maxSettings = builder.Configuration.GetSection("MaxSettings").Get<MaxSettings>() ?? new MaxSettings();
		var inverters = builder.Configuration.GetSection("Inverters").Get<InverterConfiguration[]>() ?? [];

		Log.Logger.Information($"Found {(inverters.Length == 1 ? "1 inverter" : $"{inverters.Length} inverters")} to process.");

		builder.Services.AddSingleton(maxSettings);
		builder.Services.AddSingleton<IEnumerable<InverterConfiguration>>(inverters);

		if (maxSettings.UseMockData)
			builder.Services.AddTransient<IMaxTalkClient, FakeMaxTalkClient>();
		else
			builder.Services.AddTransient<IMaxTalkClient, MaxTalkClient>();

		builder.Services.AddHostedService<ExporterService>();

		builder.Host.UseSerilog();

		var app = builder.Build();

		app.UseHttpsRedirection();
		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapMetrics();
		});

		app.Run();
	}
}

internal class FakeMaxTalkClient : IMaxTalkClient
{
	public Task<MaxValues> RequestAsync(string ip, int inverterId, int port, int timeout = 8000)
	{
		return Task.FromResult(new MaxValues 
		{ 
			Source = "FP",
			Destination = inverterId.ToString().PadLeft(2, '0'),
			EnergyDay = 10 + inverterId,
			EnergyMonth = 360 + inverterId,
			EnergyYear = 5300 + inverterId,
			EnergyTotal = 78100 + inverterId,
		});
	}
}
