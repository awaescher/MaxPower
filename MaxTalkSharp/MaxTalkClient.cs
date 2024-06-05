using System.Net.Sockets;
using System.Text;

namespace MaxTalkSharp;

public class MaxTalkClient : IMaxTalkClient
{
	public static string ENERGY_DAY = "KDY";
	public static string ENERGY_MONTH = "KMT";
	public static string ENERGY_YEAR = "KYR";
	public static string ENERGY_TOTAL = "KT0";

	public async Task<MaxValues> RequestAsync(string ip, int inverterId, int port, int timeout = 8000, CancellationToken cancellationToken = default)
    {
		using var client = new TcpClient();
		
		try
        {
            await client.ConnectAsync(ip, port, cancellationToken);
            client.SendTimeout = timeout;
            client.ReceiveTimeout = timeout;

		    var inverterStringId = inverterId.ToString().PadLeft(2, '0');
			var query = QueryBuilder.Build("FB", inverterStringId, $"{ENERGY_DAY};{ENERGY_MONTH};{ENERGY_YEAR};{ENERGY_TOTAL}");

            System.Diagnostics.Debug.WriteLine("Query: " + query);

			using var writer = new StreamWriter(client.GetStream(), Encoding.UTF8) { AutoFlush = true };
            using var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
            await writer.WriteAsync(query);

            char[] responseBuffer = new char[1024];
            int read = await reader.ReadAsync(responseBuffer, 0, responseBuffer.Length);

            if (read > 0)
            {
                var responseString = new string(responseBuffer[..read]);
                var response = MaxResponseParser.Parse(responseString);
                return MaxValuesMapper.Map(response);
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
        {
            throw new TimeoutException(ex.Message);
        }
        finally
        {
            client.Close();
        }

        return new MaxValues();
    }
}