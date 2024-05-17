using MaxTalkSharp;
using Prometheus;

namespace MaxPower.HostedServices;

public class ExporterService(MaxSettings maxSettings, IEnumerable<InverterConfiguration> inverters, IMaxTalkClient maxTalkClient, ILogger<ExporterService> logger) : BackgroundService
{
	private Gauge _energyDay = Metrics.CreateGauge("maxpower_energy_day", "The amount of energy created today in kWh", ["ip", "id"]);
	private Gauge _energyMonth = Metrics.CreateGauge("maxpower_energy_month", "The amount of energy created this month in kWh", ["ip", "id"]);
	private Gauge _energyYear = Metrics.CreateGauge("maxpower_energy_year", "The amount of energy created this year in kWh", ["ip", "id"]);
	private Gauge _energyTotal = Metrics.CreateGauge("maxpower_energy_total", "The amount of energy created in total in kWh", ["ip", "id"]);

	public MaxSettings MaxSettings { get; } = maxSettings ?? throw new ArgumentNullException(nameof(maxSettings));
	public IEnumerable<InverterConfiguration> Inverters { get; } = inverters ?? throw new ArgumentNullException(nameof(inverters));
	public IMaxTalkClient MaxTalkClient { get; } = maxTalkClient ?? throw new ArgumentNullException(nameof(maxTalkClient));
	public ILogger Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			foreach (var inverter in Inverters)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				Logger.LogInformation("Reading data from inverter \"{inverterId}\" at \"{inverterIp}:{inverterPort}\" ...", inverter.Id, inverter.Ip, inverter.Port);

				try
				{
					var data = await MaxTalkClient.RequestAsync(inverter.Ip, inverter.Id, inverter.Port);

					string[] labels = [inverter.Ip, inverter.Id.ToString()];
					_energyDay.WithLabels(labels).Set(data.EnergyDay);
					_energyMonth.WithLabels(labels).Set(data.EnergyMonth);
					_energyYear.WithLabels(labels).Set(data.EnergyYear);
					_energyTotal.WithLabels(labels).Set(data.EnergyTotal);

					Logger.LogInformation("Inverter \"{inverterId}\" made \"{energyDay} kWh today.", inverter.Id, data.EnergyDay);
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
}