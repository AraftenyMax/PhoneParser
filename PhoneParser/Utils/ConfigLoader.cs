using Newtonsoft.Json;
using PhoneParser.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneParser.Utils
{
	public static class ConfigLoader
	{
		public static ConfigModel LoadConfig(string shopName)
		{
			string path = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\" +
				$"Configs\\{shopName}.json";
			string json = File.ReadAllText(path);
			ConfigModel Config = JsonConvert.DeserializeObject<ConfigModel>(json);
		}
	}
}
