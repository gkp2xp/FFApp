using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Models
{
    public class InvestmentBreakdownModel
    {
        public InvestmentBreakdownModel() { }

        public InvestmentBreakdownModel(Entities.InvestmentBreakdown ib) {
            this.id = ib.Id;
            this.label = ib.Label;
            this.value = string.Format("{0:0.00}", ib.Value);
            this.displayType = ib.DisplayType;
            this.linkId = ib.LinkId;
            this.hierachyLevel = ib.Hierachy.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public string id { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public Entities.DisplayType displayType { get; set; }
        public string linkId { get; set; }
        public int hierachyLevel { get; set; }
    }
}
