namespace MaxTalkSharp
{
	public interface IMaxTalkClient
	{
		Task<MaxValues> RequestAsync(string ip, int inverterId, int port, int timeout = 8000);
	}
}