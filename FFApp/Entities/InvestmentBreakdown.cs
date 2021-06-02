using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Entities
{
    public class InvestmentBreakdown
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public decimal Value { get; set; }
        public DisplayType DisplayType { get; set; }
        public string LinkId { get; set; }
        public string Hierachy { get; set; }
        public IList<InvestmentBreakdown> Children { get; set; }
    }
}
