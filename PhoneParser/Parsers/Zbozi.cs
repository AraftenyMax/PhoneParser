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
		List<string> ExcludedFields = new List<string>();

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
			ExcludedFields = NodesData["ExcludedFields"].Split().ToList();
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

		string GetPhoneName(HtmlDocument page)
		{
			string name;
			try
			{
				name = page.DocumentNode.SelectSingleNode(NodesData["PhoneName"]).InnerText;
			}
			catch (Exception)
			{
				name = "";
			}
			return name;
		}

		string GetSize(HtmlDocument page)
		{
			int size_count = 0;
			string format = "0x1x2";
			List<string> sizeKeywords = NodesData["Size"].Split(',').ToList();
			foreach(string keyword in sizeKeywords)
			{
				string data;
				try
				{
					string xpath = String.Format(DataXpathTemplate, keyword);
					data = page.DocumentNode.SelectSingleNode(xpath).InnerText;
				}
				catch (Exception)
				{
					Console.WriteLine("Unable to locate size char");
					data = "?";
				}
				format = format.Replace($"{size_count}", data);
				size_count++;
			}
			return format;
		}

		string GetAccessTypes(HtmlDocument page)
		{
			string data = "";
			List<string> keywords = NodesData["AccessTypes"].Split(',').ToList();
			List<string> choices = NodesData["Yes/No"].Split(',').ToList();
			foreach(string keyword in keywords)
			{
				try
				{
					string xpath = String.Format(DataXpathTemplate, keyword);
					data += page.DocumentNode.SelectSingleNode(xpath).InnerText == choices[0] ? keyword : "";
				}
				catch (Exception)
				{
					data += "";
				}
				finally
				{
					data += ",";
				}
			}
			return data;
		}

		string GetPrice(HtmlDocument page)
		{
			string data = "";
			try
			{
				data = page.DocumentNode.SelectSingleNode(NodesData["Price"]).InnerText;
			}
			catch (Exception) { }
			return data;
		}

		Phone ParseSingleItem(string url)
		{
			string phoneUrl = String.Format(DetailLinkTemplate, url);
			var page = Client.Load(phoneUrl);
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

			phone.Price = GetPrice(page);
			phone.Size = GetSize(page);
			phone.PhoneName = GetPhoneName(page);
			phone.AccessTypes = GetAccessTypes(page);
			phone.ShopName = ShopName;
			phone.Url = phoneUrl;
			return phone;
		}
	}
}
