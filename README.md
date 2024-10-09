# MaxPower

MaxPower is a prometheus exporter for SOLARMAX inverters.

The communication to the inverters is achieved with an [reverse-engineered](https://2007.blog.dest-unreach.be/2009/04/15/solarmax-maxtalk-protocol-reverse-engineered/) implementation of the proprietary MaxTalk protocol reimplemented in C# ([MaxTalkSharp](/MaxTalkSharp)).

## Usage with Docker

The easiest way to use MaxPower is with docker. Use the files from [the dashboard sample](usage-with-docker-compose) to run a MaxPower container along with Prometheus and Grafana for a local dashboard stack which runs perfectly fine on a Raspberry Pi.

### Connection issues and regular restarts

I found that the inverters turned inaccessible after ~24 hours of running MaxPower. Strangely enough, only restarting the MaxPower-container fixed it. I checked the code but could not find any leaks or possible connection issues. That's why I went with the crowbar tactic and added an additional docker container in my docker-compose.yml that restarts the MaxPower-container regularly. You might need it too:

```yaml
  restarter:
    image: docker:cli
    restart: unless-stopped
    volumes: ["/var/run/docker.sock:/var/run/docker.sock"]
    entrypoint: ["/bin/sh","-c"]
    command:
      - |
        while true; do
          echo "Restarting MaxPower ..."
          docker restart maxpower # use the service name of the MaxPower container defined in your docker-compose.yml
          sleep 3600
        done
```
---

Solar panel icon created by Freepik - [Flaticon](https://www.flaticon.com/free-icons/solar-panel)
