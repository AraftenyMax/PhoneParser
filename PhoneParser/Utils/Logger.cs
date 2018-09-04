using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneParser.Utils
{
	class Logger
	{
		string ShopName;
		string PathToLogsFolder;
		public Logger(string name)
		{
			ShopName = name;
			PathToLogsFolder = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\" +
				$"Logs\\{name}.txt";
		}


	}
}
