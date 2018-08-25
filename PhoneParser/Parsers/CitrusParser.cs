using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using PhoneParser.EF;

namespace PhoneParser.Parsers
{
    class CitrusParser: IParser
    {
        HtmlWeb Web;
        Phone phoneModel = new Phone();
        string Preffix = "Citrus";
        List<string> ExcludeFields = new List<string>();
        List<string> FindWithRawXPath = new List<string>();
        Dictionary<string, string> NodesData = new Dictionary<string, string>();
        Dictionary<string, string> PhoneData = new Dictionary<string, string>();
        string Url;
        public CitrusParser()
        {
            FindWithRawXPath.AddRange(ConfigurationManager.AppSettings[Preffix + "FindWithRawXPath"].Split(','));
            ExcludeFields.AddRange(ConfigurationManager.AppSettings["ExcludeFields"].Split(','));
            Url = ConfigurationManager.AppSettings["CitrusUrl"];
            var props = phoneModel.GetType().GetProperties();
            foreach (var property in props)
            {
                if (!ExcludeFields.Contains(property.Name))
                {
                    string appSettingsName = Preffix + property.Name;
                    NodesData[property.Name] = ConfigurationManager.AppSettings[appSettingsName];
                }
            }
        }
        
        public void Parse()
        {
            Web = new HtmlWeb();
            var htmlDoc = Web.Load(Url);
            foreach(KeyValuePair<string, string> Property in NodesData)
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
                    PhoneData[Property.Key] = data;
                    Console.WriteLine($"{Property.Key}: {PhoneData[Property.Key]}");
                } catch(Exception ex)
                {
                    Console.WriteLine($"Oops! It seems to be unable to find element with such data: {Property.Key}");
                }
            }
        }
    }
}
