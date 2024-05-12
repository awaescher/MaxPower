using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OpenMax;

public class Inverter
{
    public int ID { get; set; }
    public Dictionary<string, string> Settings { get; set; }
}

public class SolarMax
{
    private string _host;
    private int _port;
    private TcpClient _client;
    private NetworkStream _stream;
    private BinaryReader _reader;
    private Dictionary<int, Inverter> _inverters;
    private bool _allInvertersDetected;
    private CancellationTokenSource _cancellation;

    public SolarMax(string host, int port)
    {
        _host = host;
        _port = port;
        _inverters = new Dictionary<int, Inverter>();
    }

    public void DetectInverters()
    {
        if (!_allInvertersDetected)
        {
            _cancellation = new CancellationTokenSource();
            Task.Run(() => DetectInvertersAsync(_cancellation.Token), _cancellation.Token);
        }
    }

    public Dictionary<int, Inverter> Inverters()
    {
        if (!_allInvertersDetected)
        {
            DetectInverters();
        }
        return _inverters;
    }

    public Task DetectInvertersAsync(CancellationToken cancellationToken)
    {
        //try
        //{
            using (_client = new TcpClient())
            {
                _client.Connect(_host, _port);
                _stream = _client.GetStream();
                _reader = new BinaryReader(_stream);

                foreach (var inverterId in Inverters().Keys)
                {
                    var query = BuildQuery(inverterId, "ADR", "TYP", "PIN");
                    SendQuery(query);
                    var response = ReceiveResponse();
                    if (!response.Success)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }

                _allInvertersDetected = true;
            }
        //}
        // catch (Exception ex) when (ex is System.Net.Sockets.SocketException || ex is System.IO.IOException)
        // {
        //     cancellationToken.ThrowIfCancellationRequested();
        // }

        return Task.CompletedTask;
    }

    private string BuildQuery(int inverterId, params string[] types)
    {
        var query = new StringBuilder("FB;");
        query.AppendFormat("{0:02d};", inverterId);
        query.Append("FB;");
        foreach (var type in types)
        {
            query.AppendFormat("{0};", type);
        }
        query.Append(";");
        var length = Encoding.ASCII.GetBytes(query.ToString()).Length;
        query.AppendFormat("{0:02d}", (length / 2) - 2);
        query.Append(";C8;");
        query.Append(Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(query.ToString())).Substring(0, length));
        return query.ToString();
    }

    private void SendQuery(string query)
    {
        _reader.BaseStream.Write(Encoding.ASCII.GetBytes(query), 0, Encoding.ASCII.GetByteCount(query));
    }

    private Response ReceiveResponse()
    {
        var response = new Response();
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytesRead = _stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                break;
            }
            response.Data += Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        return response;
    }
}

public class Response
{
    public int InverterId { get; set; }

    public string Data { get; set; }

    public bool Success { get; set; }
}