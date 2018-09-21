using System;
using Parsers.EF;
using HtmlAgilityPack;
using Parsers.PhoneParser;
using PhoneParser.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class UnitTest1
	{
		PhonesParser parser;

		[TestMethod]
		public void TestPageLoader()
		{
			string expectedTitle = "Google";
			HtmlDocument doc = parser.GetPage("https://www.google.com/");
			Assert.Equals(expectedTitle, doc.DocumentNode.SelectSingleNode("//title/text()").InnerText);
		}

		[TestMethod]
		public void TestParseSingleItem()
		{
			string expectedPhoneName = "Meizu M5s 16Gb Gold", expectedPrice = "2 799";
			string url = @"https://www.citrus.ua/smartfony/m5s-16gb-gold-meizu-609620.html";
			parser.LoadConfig("Citrus");
			Phone p = parser.ParseSingleItem(url);
			Assert.Equals(expectedPhoneName, p.PhoneName);
			Assert.Equals(expectedPhoneName, p.Price);
		}

		[TestMethod]
		public void TestConfigLoader()
		{
			string expectedUrlPattern = "https://allegro.pl/kategoria/smartfony-i-telefony-komorkowe-165?order=m&p={0}";
			string expectedScreenDiag = "//div[contains(text(), 'Przekątna ekranu:')]/following::div[1]";
			string expectedResultFormat = "{0}x{1}x{2}";
			parser.LoadConfig("Allegro");
			Assert.Equals()
		}
	}
}
