using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhoneParser.EF;

namespace PhoneParser.Utils
{
	class Rewriter
	{
		Phone p = new Phone();
		List<string> Shops = new List<string>() { "Citrus", "Allegro" };

		public void Rewrite()
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter();
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				var props = p.GetType().GetProperties();
				writer.Formatting = Formatting.Indented;
				foreach (string shopname in Shops)
				{
					Dictionary<string, string> Config = new Dictionary<string, string>();
					string path = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\Configs\\{shopname}.json";
					foreach (PropertyInfo prop in props)
					{
						try
						{
							Config[prop.Name] = ConfigurationManager.AppSettings[shopname + prop.Name];
						}catch(Exception) { }
					}
					using(StreamWriter file = File.CreateText(path))
					{
						JsonSerializer serializer = new JsonSerializer();
						serializer.Formatting = Formatting.Indented;
						serializer.Serialize(file, Config);
					}
				}
			}
		}
	}
}
