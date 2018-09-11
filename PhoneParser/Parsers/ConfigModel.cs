using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneParser.Parsers
{
	class ConfigModel
	{
		public Dictionary<string, string> CheckXpaths { get; set; }
		public Dictionary<string, string> UrlResolveRules { get; set; }
		public Dictionary<string, string> SingleXPathSelectors { get; set; }
	}
}
