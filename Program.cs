//var max = new OpenMax.SolarMax("192.168.178.241", 12345);
//await max.DetectInvertersAsync(CancellationToken.None);

var max = new OpenMax.OM();

foreach (var ip in new string[]{"192.168.178.241", "192.168.178.242", "192.168.178.243", "192.168.178.244", "192.168.178.245"})
{
    try
    {
        if (max.ConnectToInverter(ip, 12345))
        {
            max.ReadData("{UDC;PAC;PRL;KT0}");
            //max.ReadData("{FB;01;9C|64:DYR;DMT;DDY;PAC;KYR;KMT;KDY;KT0;TYP;PRL;UDC;UL1;UL2;UL3;IDC;IL1;IL2;IL3;TKK;SWV;PIN;SYS;SAL;SE1;SE2;SPR;SCD;UD01;UD02;UD03;ID01;ID02;ID03|282E}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ip {ip}: {ex.Message}");
    }
}