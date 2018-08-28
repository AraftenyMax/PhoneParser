using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using PhoneParser.EF;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using PhoneParser.Utils;

namespace PhoneParser.Parsers
{
    class CitrusParser: IParser
    {
        HtmlWeb Web;
        Phone phoneModel = new Phone();
        string ShopName = "Citrus";
        string DomainName;
        string LinksSelector;
        int Page = 1;
        List<string> ExcludeFields = new List<string>();    
        Dictionary<string, string> NodesData = new Dictionary<string, string>();
        string Url;

        public CitrusParser()
        {
            Console.WriteLine("Initializing CitrusParser");
            Web = new HtmlWeb();
			NodesData = ConfigLoader.LoadConfig(ShopName);
			Url = NodesData["Url"];
            LinksSelector = NodesData["DetailLink"];
            DomainName = NodesData["Domain"];
            Console.WriteLine("Initialization: success");
        }
        
        public void Parse()
        {
            Random random = new Random();
            HtmlDocument page = Web.Load(String.Format(Url, Page));
            while(true)
            {
                HtmlNodeCollection links = new HtmlNodeCollection(null);
                Console.WriteLine($"Parsing {Page} Page");
                try
                {
                    links = page.DocumentNode.SelectNodes(LinksSelector);
                    if (links == null)
                        break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Seems to be all pages parsed");
                    break;
                }
                finally
                {
                    List<Phone> phones = new List<Phone>();
                    foreach (HtmlNode link in links)
                    {
                        Phone phone = ParseSingleItem(link.Attributes["href"].Value);
                        phones.Add(phone);
                        System.Threading.Thread.Sleep(random.Next(500, 1000));
                    }
                    Page++;
                    page = Web.Load(String.Format(Url, Page));
                    SaveParsedData(phones);
                }
            }
        }

        public Phone ParseSingleItem(string url)
        {
            Web = new HtmlWeb();
            HtmlDocument htmlDoc = null;
            string phoneUrl = String.Format(DomainName, url);
            try
            {
                htmlDoc = Web.Load(phoneUrl);
            }
            catch (IOException)
            {
                htmlDoc = Web.Load(phoneUrl);
            }
            Phone phone = new Phone();
            foreach (KeyValuePair<string, string> Property in NodesData)
            {
                try
                {
                    string data = "";
                    HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode(NodesData[Property.Key]);
                    if(node.ChildNodes.Count == 1)
                    {
                        data = node.InnerHtml;
                    }
                    else
                    {
                        data = node.ParentNode.ParentNode.LastChild.InnerHtml;
                    }
                    phone.GetType().GetProperty(Property.Key).SetValue(phone, data, null);
                } catch(Exception)
                {
                    Console.WriteLine($"Oops! It seems to be unable to find element with such data: {Property.Key}");
                }
            }
            phone.ShopName = ShopName;
            phone.Url = phoneUrl;
            Console.WriteLine($"Parsed {phone.PhoneName}");
            return phone;
        }

        public void SaveParsedData(List<Phone> PhonesSet)
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
