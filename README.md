# MaxPower

MaxPower is a prometheus exporter for SOLARMAX inverters.

The communication to the inverters is achieved with an [reverse-engineered](https://2007.blog.dest-unreach.be/2009/04/15/solarmax-maxtalk-protocol-reverse-engineered/) implementation of the proprietary MaxTalk protocol reimplemented in C# ([MaxTalkSharp](/MaxTalkSharp)).

## Usage with Docker

The easiest way to use MaxPower is with docker. Use the files from [the dashboard sample](usage-with-docker-compose) to run a MaxPower container along with Prometheus and Grafana for a local dashboard stack which runs perfectly fine on a Raspberry Pi.

---

Solar panel icon created by Freepik - [Flaticon](https://www.flaticon.com/free-icons/solar-panel)