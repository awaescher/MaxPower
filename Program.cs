//var max = new OpenMax.SolarMax("192.168.178.241", 12345);
//await max.DetectInvertersAsync(CancellationToken.None);

// {FB;05;4E|64:E1D;E11;E1h;E1m;E1M;E2D;E21;E2h;E2m;E2M;E3D;E31;E3h;E3m;E3M|1270}
var test = QueryBuilder.Build("FB", "05", "E1D;E11;E1h;E1m;E1M;E2D;E21;E2h;E2m;E2M;E3D;E31;E3h;E3m;E3M");

// {FB;05;36|64:CAC;KHR;KDY;KMT;KYR;KT0;KLD;KLM;KLY|0D34}
test = QueryBuilder.Build("FB", "05", "CAC;KHR;KDY;KMT;KYR;KT0;KLD;KLM;KLY");

// {FB;05;3E|64:ADR;DDY;DMT;DYR;PIN;SWV;THR;TMI;TYP;FRD;LAN|0FC4}
test = QueryBuilder.Build("FB", "05", "ADR;DDY;DMT;DYR;PIN;SWV;THR;TMI;TYP;FRD;LAN");

// {FB;01;9C|64:DYR;DMT;DDY;PAC;KYR;KMT;KDY;KT0;TYP;PRL;UDC;UL1;UL2;UL3;IDC;IL1;IL2;IL3;TKK;SWV;PIN;SYS;SAL;SE1;SE2;SPR;SCD;UD01;UD02;UD03;ID01;ID02;ID03|282E}
test = QueryBuilder.Build("FB", "01", "DYR;DMT;DDY;PAC;KYR;KMT;KDY;KT0;TYP;PRL;UDC;UL1;UL2;UL3;IDC;IL1;IL2;IL3;TKK;SWV;PIN;SYS;SAL;SE1;SE2;SPR;SCD;UD01;UD02;UD03;ID01;ID02;ID03");

var max = new OpenMax.OM();

foreach (var ip in new string[]{"192.168.178.241", "192.168.178.242", "192.168.178.243", "192.168.178.244", "192.168.178.245"})
{
    try
    {
        if (max.ConnectToInverter(ip, 12345))
        {
            var query = QueryBuilder.Build("FB", "01", "KDY");
            var response = max.ReadData(query);
            Console.WriteLine(response.ToString());
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ip {ip}: {ex.Message}");
    }
}

