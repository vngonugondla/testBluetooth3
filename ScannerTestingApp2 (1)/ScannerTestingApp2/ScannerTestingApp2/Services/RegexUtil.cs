using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScannerTestingApp.Services
{
    public static class RegexUtil
    {
        public readonly static Regex ZebraReaderRegex = new Regex(@"RFD\d{2}");
        public readonly static Regex EpcRegex = new Regex("1A[0-9A-Z]{22}");
    }
}
