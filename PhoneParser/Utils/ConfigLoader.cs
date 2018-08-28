using Newtonsoft.Json;
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
		public static Dictionary<string, string> LoadConfig(string shopName)
		{
			string path = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\" +
				$"Configs\\{shopName}.json";
			string json = File.ReadAllText(path);
			Dictionary<string, string> config = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			return config;
		}
	}
}
