using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using PhoneParser.EF;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace PhoneParser.Parsers
{
    class CitrusParser: IParser
    {
        HtmlWeb Web;
        Phone phoneModel = new Phone();
        string Preffix = "Citrus";
        string NoResultsId;
        string LinksSelector;
        int Page = 1;
        List<string> ExcludeFields = new List<string>();
        List<string> FindWithRawXPath = new List<string>();
        List<Phone> PhonesSet = new List<Phone>();
        Dictionary<string, string> NodesData = new Dictionary<string, string>();
        Dictionary<string, string> PhoneData = new Dictionary<string, string>();
        string Url;

        public CitrusParser()
        {
            Console.WriteLine("Initializing CitrusParser");
            Web = new HtmlWeb();
            FindWithRawXPath.AddRange(ConfigurationManager.AppSettings[Preffix + "FindWithRawXPath"].Split(','));
            ExcludeFields.AddRange(ConfigurationManager.AppSettings["ExcludeFields"].Split(','));
            Url = ConfigurationManager.AppSettings[Preffix + "Url"];
            NoResultsId = ConfigurationManager.AppSettings[Preffix + "NoResultsId"];
            LinksSelector = ConfigurationManager.AppSettings[Preffix + "DetailLink"];
            var props = phoneModel.GetType().GetProperties();
            foreach (var property in props)
            {
                if (!ExcludeFields.Contains(property.Name))
                {
                    string appSettingsName = Preffix + property.Name;
                    NodesData[property.Name] = ConfigurationManager.AppSettings[appSettingsName];
                }
            }
            Console.WriteLine("Initialization: success");
        }
        
        public void Parse()
        {
            HtmlDocument page = Web.Load(String.Format(Url, Page));
            while(page.DocumentNode.SelectSingleNode(NoResultsId).OuterLength == 0)
            {
                Console.WriteLine($"Parsing {Page} Page");
                HtmlNodeCollection links = page.DocumentNode.SelectNodes(LinksSelector);
                foreach(HtmlNode link in links)
                {
                    ParseSingleItem(link.Attributes["href"].Value);
                }
                Page++;
                page = Web.Load(String.Format(Url, Page));
            }
            SaveParsedData();
        }

        public void ParseSingleItem(string url)
        {
            Web = new HtmlWeb();
            var htmlDoc = Web.Load(url);
            Phone phone = new Phone();
            foreach (KeyValuePair<string, string> Property in NodesData)
            {
                try
                {
                    string data = "";
                    HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode(NodesData[Property.Key]);
                    if (FindWithRawXPath.Contains(Property.Key))
                    {
                        data = node.InnerHtml;
                    }
                    else
                    {
                        data = node.ParentNode.ParentNode.ChildNodes[2].InnerText;
                    }
                    phone.GetType().GetProperty(Property.Key).SetValue(phone, data, null);
                    PhonesSet.Add(phone);
                } catch(Exception ex)
                {
                    Console.WriteLine($"Oops! It seems to be unable to find element with such data: {Property.Key}");
                }
            }
            phone.ShopName = Preffix;
            Console.WriteLine($"Parsed {phone.PhoneName}");
        }

        public void SaveParsedData()
        {
            using (PhoneModel context = new PhoneModel())
            {
                Console.WriteLine($"Saving parsed {PhonesSet.Count} items...");
                context.Phones.AddRange(PhonesSet);
                context.SaveChanges();
                Console.WriteLine($"Successfully saved {PhonesSet.Count} items...");
            }
        }
    }
}
