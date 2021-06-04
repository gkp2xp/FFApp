using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Models
{
    public class InvestmentModel
    {
        public InvestmentModel() { }

        public InvestmentModel(Entities.Investment investment) {
            this.id = investment.Id.ToString();
            this.label = investment.Label;
            this.value = string.Format("{0:0.00}", investment.CurrentValue);
            this.portfolioGroup = investment.PortfolioGrouping?.Label;
        }

        public string id { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public string portfolioGroup { get; set; }
    }
}
