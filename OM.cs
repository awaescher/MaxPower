using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OpenMax;

public class OM
{
    private Socket _socket;

    private const string idc = "IDC";
    private const string ul1 = "UL1";
    private const string tkk = "TKK";
    private const string il1 = "IL1";
    private const string sys = "SYS";
    private const string tnf = "TNF";
    private const string udc = "UDC";
    private const string pac = "PAC";
    private const string prl = "PRL";
    private const string kt0 = "KT0";



    public string reqData = "{FB;01;3E|64:IDC;UL1;TKK;IL1;SYS;TNF;UDC;PAC;PRL;KT0;SYS|0F66}";

    // void PublishMessage(string topic, string payload)
    // {
    //     // publish the message to the mqtt broker
    //     // accepts a JSON payload
    //     // publishs to the following line is for local broker
    //     // MQTTnet.MqttClient mqttClient = new MQTTnet.MqttClient("192.168.1.24");
    //     // mqttClient.Connect(mqttBrokerIp, mqttBrokerPort);
    //     // MQTTnet.MqttApplicationMessage message = new MQTTnet.MqttApplicationMessage();
    //     // message.Payload = Encoding.UTF8.GetBytes(payload);

    //     try
    //     {
    //         MQTTClient mqttClient = new MQTTClient("iot-2.messaging.internetofthings.ibmcloud.com");
    //         mqttClient.Connect("d:k4cp0:raspberrypi:b827ebc2478d", "Sz2(u_6!+h_MIe@&7Z");
    //         MQTTApplicationMessage message = new MQTTApplicationMessage();
    //         message.Topic = iotTopic;
    //         message.Payload = Encoding.UTF8.GetBytes(payload);

    //         mqttClient.Publish(message);
    //     }
    //     catch (Exception ex)
    //     {
    //         string template = "Publish.single: An exception of type {0} occured. Arguments:\n{1!r}";
    //         string message = String.Format(template, ex.GetType().Name, ex.Message);
    //         Console.WriteLine(message);
    //         throw;
    //     }
    // }

    List<string> GenData(string s)
    {
        var fieldMap = new Dictionary<string, string>()
        {
            { idc, "dc_current" },
            { ul1, "voltage_phase1" },
            { tkk, "inverter_temp" },
            { il1, "current_phase1" },
            { sys, "sys" },
            { tnf, "frequency" },
            { udc, "dc_voltage" },
            { pac, "power_output" },
            { prl, "relative_ouput" },
            { kt0, "total_yield" }
        };

        string[] t = s.Split('=');
        string f = t[0];

        double v = 0;
        if (f == sys) // remove the trailing ,0
            v = Convert.ToInt32(t[1].Substring(0, t[1].IndexOf(',')), 16);
        else
            v = Convert.ToInt32(t[1], 16);

        if (f == pac) // PAC values are *2 for some reason
            v /= 2;

        if (f == udc) // voltage levels need to be divide by 10
            v /= 10.0;

        if (f == idc || f == tnf) //current & frequency needs to be divided by 100
            v /= 100.0;

        return new List<string>() { fieldMap[f] + ": " + v };
    }

    string ConvertToJson(string data)
    {
        // convert the inverter message to JSON
        // -- data: {01;FB;70|64:IDC=407;UL1=A01;TKK=2C;IL1=46D;SYS=4E28,0;TNF=1383;UDC=B7D;PAC=16A6;PRL=2B;KT0=48C;SYS=4E28,0|1A5F}
        // -- return:

        // original:
        // ev = [genData(s) for s in  data[data.find(':')+1:data.find('|',data.find(':'))].split(';')]

        return data;

        // List<Tuple<string, double>> ev = GenData(s) for s in data[data.find(':') + 1:data.find('|', data.find(':'))].Split(';');
        // string outStr = "{{ \"d\": { ";
        // foreach (var e in ev)
        //     outStr += String.Format("\"{0}\": {1},", e.Item1, e.Item2);
        // outStr = outStr[:len(outStr) - 1] + "} }";
        // return outStr;
    }

    bool CheckMsg(string msg)
    {
        // check that the message is valid
        return true;
    }

    void PublishData(string data)
    {
        Console.WriteLine("publishing: " + data);
        string jsonData = ConvertToJson(data);
        Console.WriteLine("published: " + jsonData);
        //PublishMessage(iotTopic, jsonData);
    }

    public bool ConnectToInverter(string inverterIp, int inverterPort)
    {
        // https://github.com/bwurst/python-solarmax/blob/dd27d673ed368946805253b411def112cc0f3f4e/lib/solarmax.py#L86
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.SendTimeout = 5000;
        _socket.ReceiveTimeout = 5000;
        _socket.Connect(inverterIp, inverterPort);
        return _socket.Connected;
    }

    public string ReadData(string request)
    {
        _socket.Send(Encoding.UTF8.GetBytes(request));
        byte[] buffer = new byte[1024];
        int bytesRead = 0;
        string response = "";
        while (bytesRead == 0)
        {
            bytesRead = _socket.Receive(buffer, offset: 0, size: 1, SocketFlags.None);
            if (bytesRead > 0)
                response += Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        //return CheckMsg(response);
        return response;
    }
}