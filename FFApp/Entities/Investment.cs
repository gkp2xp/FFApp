using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Entities
{
    public class Investment
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public decimal CurrentValue { get; set; }
        public int PortfolioGroupingId { get; set; }
        public PortfolioGrouping PortfolioGrouping { get; set; }
    }
}
