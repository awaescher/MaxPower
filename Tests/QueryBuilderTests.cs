using FluentAssertions;
using MaxTalkSharp;
using NUnit.Framework;

namespace Tests;

public class QueryBuilderTests
{
	public class BuildMethod : QueryBuilderTests
	{
		[TestCase(
			"FB", "01", "CAC;KHR;KDY;KMT;KYR;KT0;KLD;KLM;KLY",
			"{FB;01;36|64:CAC;KHR;KDY;KMT;KYR;KT0;KLD;KLM;KLY|0D30}")]
		[TestCase(
			"FB", "05", "ADR;DDY;DMT;DYR;PIN;SWV;THR;TMI;TYP;FRD;LAN",
			"{FB;05;3E|64:ADR;DDY;DMT;DYR;PIN;SWV;THR;TMI;TYP;FRD;LAN|0FC4}")]
		[TestCase(
			"01", "02", "DYR;DMT;DDY;PAC;KYR;KMT;KDY;KT0;TYP;PRL;UDC;UL1;UL2;UL3;IDC;IL1;IL2;IL3;TKK;SWV;PIN;SYS;SAL;SE1;SE2;SPR;SCD;UD01;UD02;UD03;ID01;ID02;ID03",
			"{01;02;9C|64:DYR;DMT;DDY;PAC;KYR;KMT;KDY;KT0;TYP;PRL;UDC;UL1;UL2;UL3;IDC;IL1;IL2;IL3;TKK;SWV;PIN;SYS;SAL;SE1;SE2;SPR;SCD;UD01;UD02;UD03;ID01;ID02;ID03|2808}")]
		public void Test(string source, string destination, string query, string expected)
		{
			QueryBuilder.Build(source, destination, query).Should().Be(expected);
		}
	}
}