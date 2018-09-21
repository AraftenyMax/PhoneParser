using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace PhoneParser.Parsers
{
	class Validators
	{
		Dictionary<string, string> ValidationConfig;
		List<string> ValidatorNames;
		string Path;

		public void LoadValidators(string shopName)
		{
			Path = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\" +
				$"Configs\\{shopName}.json";
			string json = File.ReadAllText(Path);
			ValidationConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
		}
	}
}
