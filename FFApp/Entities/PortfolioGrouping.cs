using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Entities
{
    public class PortfolioGrouping
    {
        public int Id { get; set; }        
        public string Label { get; set; }
        public int ParentId { get; set; }
        public PortfolioGrouping Parent { get; set; }
    }
}
