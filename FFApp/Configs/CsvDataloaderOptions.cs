using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Configs
{
    public class CsvDataloaderOptions
    {
        public const string Name = "CsvLoader";
        public string InvestmentFilename { get; set; }
        public bool InvestmentFileHasHeader { get; set; }
        public string PortfolioGroupingFilename { get; set; }
        public bool PortfolioGroupingHasHeader { get; set; }
        public int UnknownPortfolioId { get; set; }

    }
}
