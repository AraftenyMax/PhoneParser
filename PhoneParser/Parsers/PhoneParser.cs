using Newtonsoft.Json;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parsers.EF;

namespace Parsers.PhoneParser
{
	class PhonesParser
	{
		dynamic Config;
		HtmlWeb Browser = new HtmlWeb();
		string DetailLinkSelector;
		string DetailUrlTemplate;
		string PageUrlTemplate;
		string PhoneNameSelector;

		public PhonesParser()
		{
		}

		public void CheckShop(string shopName)
		{
			int Page = 1;
			LoadConfig("ShopName");
			while (true)
			{
				string url = String.Format(PageUrlTemplate, Page);
				HtmlDocument html = Browser.Load(url);
				HtmlNodeCollection linkNodes = new HtmlNodeCollection(null);
				try
				{
					linkNodes = html.DocumentNode.SelectNodes(DetailLinkSelector);
				}
				catch (Exception)
				{
					break;
				}
				int phoneNumber = 0;
				List<Phone> phones = new List<Phone>();
				foreach(var node in linkNodes)
				{
					string detailLink = ResolveDetailUrl(node.Attributes["href"].Value);
					string phoneName = html.DocumentNode.SelectNodes(PhoneNameSelector)[phoneNumber].InnerText;
					if(!CurrentlyInDB(phoneName, shopName))
					{
						Phone p = ParseSingleItem(detailLink);
						phones.Add(p);
					}
					phoneNumber++;
					SaveParsedData(phones);
				}
			}
		}

		bool CurrentlyInDB(string phoneName, string shopName)
		{
			bool exists;
			using (PhoneModel context = new PhoneModel())
			{
				Phone p = context.Phones.FirstOrDefault(e => e.PhoneName == phoneName && e.ShopName == shopName);
				exists = p == null ? true : false;
			}
			return exists;
		}


		Phone ParseSingleItem(string url)
		{
			Phone p = new Phone();
			HtmlDocument html = Browser.Load(url);
			ParseSimpleSection(html, p);
			return p;
		}

		void ParseSimpleSection(HtmlDocument html, Phone phone)
		{
			if (Config.SimpleXPathSelectors)
			{
				foreach(var prop in Config.SimpleXPathSelectors)
				{
					try
					{
						string xpath = Config.SimpleXPathSelectors[prop.Key];
						string data = html.DocumentNode.SelectSingleNode(xpath).InnerText;
						phone.GetType().GetProperty(prop.Key).SetValue(phone, data, null);
					}
					catch (Exception) { }
				}
			}
		}

		string ResolveDetailUrl(string detailLink)
		{
			try
			{
				return String.Format(Config.UrlResolveRules.DetailLinkTemplate, detailLink);
			}
			catch (Exception)
			{
				return detailLink;
			}
		}

		void LoadConfig(string shopName)
		{
			string path = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\" +
				$"Configs\\{shopName}.json";
			string json = File.ReadAllText(path);
			Config = JsonConvert.DeserializeObject(json);

			DetailLinkSelector = Config.UrlResolveRules.DetailLinkSelector;
			DetailUrlTemplate = Config.UrlResolveRules.DetailUrlTemplate;
			PageUrlTemplate = Config.UrlResolveRules.PageUrlTemplate;
			PhoneNameSelector = Config.CheckXpaths.PhoneNameSelector;
		}

		void SaveParsedData(List<Phone> phones)
		{
			using(PhoneModel context = new PhoneModel())
			{
				try
				{
					context.Phones.AddRange(phones);
					context.SaveChanges();
				}
				catch (Exception) { }
			}
		}
	}
}
