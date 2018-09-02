using HtmlAgilityPack;
using PhoneParser.EF;
using PhoneParser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneParser.Parsers
{
	public class Zbozi
	{
		int Page = 1;

		string ShopName = "Zbozi";
		string DetailLinkXpath;
		string DetailLinkTemplate;
		string PhoneListLinkTemplate;
		string DataXpathTemplate;
		string ExcludedFields;

		HtmlWeb Client;
		Dictionary<string, string> NodesData;

		public Zbozi()
		{
			Client = new HtmlWeb();
			NodesData = ConfigLoader.LoadConfig(ShopName);
			DetailLinkTemplate = NodesData["Domain"];
			PhoneListLinkTemplate = NodesData["Url"];
			DataXpathTemplate = NodesData["DataTemplate"];
			DetailLinkXpath = NodesData["DetailLink"];
			ExcludedFields = NodesData["ExcludedFields"];
		}

		public void Parse()
		{
			string url = String.Format(PhoneListLinkTemplate, Page);
			while (true)
			{
				var htmlPage = Client.Load(url);
				List<Phone> PhonesSet = new List<Phone>();
				try
				{
					var linkNodes = htmlPage.DocumentNode.SelectNodes(DetailLinkXpath);
					foreach(var node in linkNodes)
					{
						Phone phone = ParseSingleItem(node.Attributes["href"].Value);
						PhonesSet.Add(phone);
					}
				}
				catch (Exception)
				{
					Console.WriteLine("All available pages are parsed");
					break;
				}
			}
		}

		public Phone ParseSingleItem(string url)
		{
			var page = Client.Load(String.Format(DetailLinkTemplate, url));
			Phone phone = new Phone();
			foreach(var prop in NodesData)
			{
				if (!ExcludedFields.Contains(prop.Key))
				{
					try
					{
						string xpath = String.Format(DataXpathTemplate, NodesData[prop.Key]);
						string data = page.DocumentNode.SelectSingleNode(xpath).InnerText;
						phone.GetType().GetProperty(prop.Key).SetValue(data, null);
					}
					catch (Exception)
					{
						Console.WriteLine($"Unable to locate: {prop.Key}");
					}
				}
			}
		}
	}
}
