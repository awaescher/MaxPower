using FluentAssertions;
using MaxTalkSharp;
using NUnit.Framework;

namespace Tests;

public partial class ResponseParserTests
{
	public class ParseMethod : ResponseParserTests
	{
		[Test]
		public void Handles_Empty_Strings()
		{
			var response = MaxResponseParser.Parse("");
			response.IsEmpty.Should().BeTrue();
		}

		[Test]
		public void Parses_Single_Value()
		{
			var response = MaxResponseParser.Parse("{01;FB;19|64:KDY=E2|0501}");

			response.Source.Should().Be("01");
			response.Destination.Should().Be("FB");
			response.Values.Single().Key.Should().Be("KDY");
			response.Values.Single().Value.Should().Be(226);
			response.CheckSum.Should().Be("0501");
		}

		[Test]
		public void Parses_Multiple_V1alues()
		{
			var response = MaxResponseParser.Parse("{03;FB;19|64:KDY=81;KDL=E2;KDX=E2|04F5}");

			response.Source.Should().Be("03");
			response.Destination.Should().Be("FB");

			response.Values.Values.Count.Should().Be(3);

			response.Values["KDY"].Should().Be(129);
			response.Values["KDL"].Should().Be(226);
			response.Values["KDX"].Should().Be(226);

			response.CheckSum.Should().Be("04F5");
		}
	}
}