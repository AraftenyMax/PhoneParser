using HtmlAgilityPack;
using PhoneParser.EF;
using PhoneParser.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneParser.Parsers
{
    class AllegroParser: IParser
    {
        HtmlWeb Web = new HtmlWeb();
		Dictionary<string, string> NodesData = new Dictionary<string, string>();
		int Page = 1;
        string ShopName = "Allegro";
		string UrlTemplate;
        string LinksSelector;

        public AllegroParser()
        {
			NodesData = ConfigLoader.LoadConfig(ShopName);
			LinksSelector = NodesData["DetailLink"];
			UrlTemplate = NodesData["Url"];
        }

        public bool IsUrlValid(string url)
        {
            return !url.Contains("redirect=");
        }

        public void Parse()
        {
			while (true)
			{
				Console.WriteLine($"Parsing {Page} page");
				List<Phone> phoneSet = new List<Phone>();
				string url = String.Format(UrlTemplate, Page);
				var htmlDoc = Web.Load(url);
				var urls = htmlDoc.DocumentNode.SelectNodes(LinksSelector);
				if (urls == null)
					break;
				foreach (var urlNode in urls)
				{
					string phoneUrl = urlNode.SelectSingleNode("a").Attributes["href"].Value;
					if (IsUrlValid(phoneUrl))
					{
						Phone p = ParseSingleItem(phoneUrl);
						phoneSet.Add(p);
					}
				}
				SaveParsedData(phoneSet);
				Page++;
			}
        }

		string GetSize(HtmlDocument doc)
		{
			List<string> configData = NodesData["Size"].Split(';').ToList();
			List<string> keywords = configData.GetRange(1, 3);
			string template = configData[0];
			string res = "0x1x2 mm";
			int counter = 0;
			foreach (string keyword in keywords)
			{
				string xpath = String.Format(template, keyword);
				try
				{
					var elem = doc.DocumentNode.SelectSingleNode(xpath)
						.ParentNode.ChildNodes[1].InnerText.Replace(" mm", "").Trim();
					res = res.Replace($"{counter}", elem);
					counter++;
				}
				catch (Exception)
				{
					res = res.Replace($"{counter}", "unknown");
				}
				counter++;
			}
			return res;
		}

		string GetPrice(HtmlDocument doc)
		{
			var nodes = doc.DocumentNode.SelectSingleNode(NodesData["Price"])
				.ChildNodes.ToList().GetRange(0, 2);
			List<string> price_tokens = new List<string>();
			foreach (var node in nodes) {
				string data = node.InnerText;
				price_tokens.Add(data);
			}
			string price = String.Join("", price_tokens).Replace(" ", "");
			return price;
		}

		Phone ParseSingleItem(string url)
		{
			var htmlDoc = Web.Load(url);
			Phone phone = new Phone();
			phone.Url = url;
			phone.ShopName = ShopName;
			phone.Size = GetSize(htmlDoc);
			phone.Price = GetPrice(htmlDoc);
			foreach(var prop in NodesData)
			{
				try
				{
					if (NodesData["ExcludedFields"].Contains(prop.Key))
						continue;
					string data = htmlDoc.DocumentNode.SelectSingleNode(prop.Value)
						.ParentNode.ChildNodes[1].InnerText.Trim();
					phone.GetType().GetProperty(prop.Key).SetValue(phone, data, null);
				} catch(Exception) {
					Console.WriteLine($"Unable to locate value for: {prop.Key}");
				}
			}
			return phone;
		}

		void SaveParsedData(List<Phone> phoneSet)
		{
			using(var context = new PhoneModel())
			{
				Console.WriteLine($"Saving parsed data from {Page} page");
				context.Phones.AddRange(phoneSet);
				context.SaveChanges();
				Console.WriteLine($"Successfully saved data from {Page} page");
			}
		}
    }
}
