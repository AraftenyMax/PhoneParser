using HtmlAgilityPack;
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
        string Preffix = "Allegro";
        string Domain;
        string LinksSelector;

        public AllegroParser()
        {

        }

        public bool IsUrlValid(string url)
        {
            return url.Contains("redirect=");
        }

        public void Parse()
        {

        }
    }
}
