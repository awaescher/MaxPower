using MaxTalkSharp;
using Prometheus;
using System.Collections.Concurrent;

namespace MaxPower.HostedServices;

public class ExporterService(MaxSettings maxSettings, IEnumerable<InverterConfiguration> inverters, IMaxTalkClient maxTalkClient, ILogger<ExporterService> logger) : BackgroundService
{
	private Counter _energyDay = Metrics.CreateCounter("EnergyDay", "The amount of energy created this day in kWh", ["ip", "id"]);
	private Counter _energyMonth = Metrics.CreateCounter("EnergyMonth", "The amount of energy created this month in kWh", ["ip", "id"]);
	private Counter _energyYear = Metrics.CreateCounter("EnergyYear", "The amount of energy created this year in kWh", ["ip", "id"]);
	private Counter _energyTotal = Metrics.CreateCounter("EnergyTotal", "The amount of energy created in total in kWh", ["ip", "id"]);

	public MaxSettings MaxSettings { get; } = maxSettings ?? throw new ArgumentNullException(nameof(maxSettings));
	public IEnumerable<InverterConfiguration> Inverters { get; } = inverters ?? throw new ArgumentNullException(nameof(inverters));
	public IMaxTalkClient MaxTalkClient { get; } = maxTalkClient ?? throw new ArgumentNullException(nameof(maxTalkClient));
	public ILogger Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		try
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				foreach (var inverter in Inverters)
				{
					Logger.LogInformation("Trying inverter \"{inverterId}\" at \"{inverterIp}:{inverterPort}\".", inverter.Id, inverter.Ip, inverter.Port);

					try
					{
						var data = await MaxTalkClient.RequestAsync(inverter.Ip, inverter.Id, inverter.Port);

						string[] labels = [inverter.Ip, inverter.Id.ToString()];
						_energyDay.WithLabels(labels).IncTo(data.EnergyDay);
						_energyMonth.WithLabels(labels).IncTo(data.EnergyMonth);
						_energyYear.WithLabels(labels).IncTo(data.EnergyYear);
						_energyTotal.WithLabels(labels).IncTo(data.EnergyTotal);
					}
					catch (Exception ex)
					{
						Logger.LogError(ex, "An error occured while executing inverter \"{inverterId}\" at \"{inverterIp}:{inverterPort}\".", inverter.Id, inverter.Ip, inverter.Port);
					}
				}

				Logger.LogInformation($"Entering sleep state for {MaxSettings.PollIntervalSeconds} seconds.");
				await Task.Delay(TimeSpan.FromSeconds(MaxSettings.PollIntervalSeconds), cancellationToken);
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
		}
	}
}