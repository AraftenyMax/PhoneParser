using Newtonsoft.Json;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Parsers.EF;
using PhoneParser.Parsers;
using PhoneParser.Utils;
using System.Threading;

namespace Parsers.PhoneParser
{
	class PhonesParser
	{
		ConfigModel Config;
		HtmlWeb Browser = new HtmlWeb();
		LogManager Log = new LogManager();
		Random SleepPeeker = new Random();
		string DetailLinkSelector;
		string DetailUrlTemplate;
		string PageUrlTemplate;
		string PhoneNameSelector;

		public PhonesParser()
		{
		}

		HtmlDocument GetPage(string url)
		{
			HtmlDocument page;
			while (true)
			{
				try
				{
					page = Browser.Load(url);
				}
				catch (Exception)
				{
					Log.WriteToLog(LogManager.Messages["CannotReceivePage"], url);
					Thread.Sleep(SleepPeeker.Next(1000, 2000));
					continue;
				}
				break;
			}
			return page;
		}

		public void CheckShop(string shopName)
		{
			int Page = 1;
			LoadConfig(shopName);
			while (true)
			{
				string url = String.Format(PageUrlTemplate, Page);
				HtmlDocument html = GetPage(url);
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
					string phoneName = "";
					string detailLink = ResolveDetailUrl(node.Attributes["href"].Value);
					try {
						phoneName = html.DocumentNode.SelectNodes(PhoneNameSelector)[phoneNumber].InnerText;
					} catch (Exception)
					{
						Log.WriteToLog(LogManager.Messages["ParserFinish"]);
						SaveParsedData(phones);
						break;
					}
					if(!CurrentlyInDB(phoneName, shopName))
					{
						Phone p = ParseSingleItem(detailLink);
						phones.Add(p);
					}
					phoneNumber++;
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
			HtmlDocument html = GetPage(url);
			ParseSimpleSection(html, p);
			return p;
		}

		void ParseSimpleSection(HtmlDocument html, Phone phone)
		{
			if (Config.SingleXPathSelectors != null)
			{
				foreach(var prop in Config.SingleXPathSelectors)
				{
					try
					{
						string xpath = Config.SingleXPathSelectors[prop.Key];
						string data = html.DocumentNode.SelectSingleNode(xpath).InnerText.Trim();
						phone.GetType().GetProperty(prop.Key).SetValue(phone, data, null);
					}
					catch (Exception e)
					{
						Log.WriteToLog(LogManager.Messages["ExceptionOccurred"], e.Message);
					}
				}
			}
		}

		void ParseComplicatedSection(Phone p, HtmlDocument page)
		{
			if (Config.ComplicatedXPathSelectors != null)
			{
				foreach(var prop in Config.ComplicatedXPathSelectors)
				{
					foreach(ComplicatedXpathSelector selector in prop.Value)
					{
						List<object> data = new List<object>();
						foreach (string xpath in selector.XPaths)
						{
							try
							{
								data.Add(page.DocumentNode.SelectSingleNode(xpath).InnerText.Trim());
							}
							catch(Exception e)
							{
								Log.WriteToLog(LogManager.Messages["ExceptionOccurred"], e.Message);
								continue;
							}
						}
						string parsedData = String.Format(selector.ResultFormat, data.ToArray());
						p.GetType().GetProperty(prop.Key).SetValue(p, parsedData, null);
					}
				}
			}
		}

		string ResolveDetailUrl(string detailLink)
		{
			try
			{
				return String.Format(Config.UrlResolveRules["DetailLinkTemplate"], detailLink);
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
			Config = JsonConvert.DeserializeObject<ConfigModel>(json);

			DetailLinkSelector = Config.CheckXpaths["DetailLinkSelector"];
			DetailUrlTemplate = Config.UrlResolveRules["DetailLinkTemplate"];
			PageUrlTemplate = Config.UrlResolveRules["PageLinkTemplate"];
			PhoneNameSelector = Config.CheckXpaths["PhoneNameSelector"];
			Log.WriteToLog(LogManager.Messages["ShopParserInit"], shopName);
		}

		void SaveParsedData(List<Phone> phones)
		{
			Log.WriteToLog(LogManager.Messages["SavingParsedData"]);
			using(PhoneModel context = new PhoneModel())
			{
				try
				{
					context.Phones.AddRange(phones);
					context.SaveChanges();
				}
				catch (Exception) { }
			}
			Log.WriteToLog(LogManager.Messages["SaveDataSuccess"]);
		}
	}
}
