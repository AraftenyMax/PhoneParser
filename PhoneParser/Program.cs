using PhoneParser.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneParser
{
    class Program
    {
        static void Main(string[] args)
        {
            CitrusParser parser = new CitrusParser();
            parser.Parse();
            Console.ReadKey();
        }
    }
}
