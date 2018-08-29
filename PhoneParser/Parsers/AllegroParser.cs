﻿using HtmlAgilityPack;
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
            return url.Contains("redirect=");
        }

        public void Parse()
        {
			while (true)
			{
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
						Phone p = ParseSingleItem(url);
						phoneSet.Add(p);
					}
				}
				SaveParsedData(phoneSet);
			}
        }

		string getSize(HtmlDocument doc)
		{
			List<string> configData = NodesData["Size"].Split(',').ToList();
			List<string> xpaths = configData.GetRange(1, 3);
			string template = configData[0];
			string res = "";
			foreach (string xpath in xpaths)
				res += doc.DocumentNode.SelectSingleNode(xpath).ParentNode.ChildNodes[1].InnerText;
			return res;
		}

		Phone ParseSingleItem(string url)
		{
			var htmlDoc = Web.Load(url);
			Phone phone = new Phone();
			phone.Url = url;
			phone.ShopName = ShopName;
			phone.Size = getSize(htmlDoc);
			foreach(var prop in NodesData)
			{
				try
				{
					string data = htmlDoc.DocumentNode.SelectSingleNode(prop.Value).ParentNode.ChildNodes[1].InnerText;
					phone.GetType().GetProperty(prop.Key).SetValue(phone, data, null);
				} catch(Exception) { }
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
