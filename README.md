# MaxPower

MaxPower is a prometheus exporter for SOLARMAX inverters. It is intended to query SOLARMAX inverter data using an reverse-engineered implementation of their proprietary MaxTalk protocol, called [MaxTalkSharp](/MaxTalkSharp).

## Usage with Docker

The easiest way to use MaxPower is with docker. Use the files from [the dashboard sample](usage-with-docker-compose) to run a MaxPower container along with Prometheus and Grafana for a local dashboard stack which runs perfectly fine on a Raspberry Pi.

---

Solar panel icon created by Freepik - [Flaticon](https://www.flaticon.com/free-icons/solar-panel)