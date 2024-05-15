﻿using MaxTalkSharp;
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
		try
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				foreach (var inverter in Inverters)
				{
					Logger.LogInformation("Reading data from inverter \"{inverterId}\" at \"{inverterIp}:{inverterPort}\" ...", inverter.Id, inverter.Ip, inverter.Port);

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