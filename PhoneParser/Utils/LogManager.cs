using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneParser.Utils
{
	class LogManager
	{
		StreamWriter Writer;

		public LogManager()
		{
			Writer = new StreamWriter(Path);
		}
		public string ShopName { get; set; }
		string Path = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\" +
				$"Configs\\{DateTime.Now}.txt";
		public static Dictionary<string, string> Messages = new Dictionary<string, string>
		{
			{"NewItemFound","New item with name {0} is found!"},
			{"Page","Parsing {0} page."},
			{"ShopParserInit","Parser for {0} initialized successfully."},
			{"SavingParsedData","Saving parsed data."},
			{"SaveDataSuccess",  "Parsed data saved successfully."},
			{"ExceptionOccurred", "{0}"},
			{"ParserFinish", "Parser work is finished."},
			{"CannotReceivePage", "Cannot obtain page with url {0}. Trying reconnect..."}
		};

		public void WriteToLog(string msg, params object[] args)
		{
			if(args != null)
			{
				msg = String.Format(msg, args.Select(x => x.ToString()).ToArray());
			}
			Writer.WriteLine(msg);
		}
	}
}
