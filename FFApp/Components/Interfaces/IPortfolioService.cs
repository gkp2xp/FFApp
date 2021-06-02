using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Components
{
    public interface IPortfolioService
    {
        IEnumerable<Entities.InvestmentBreakdown> GetBreakdown();
    }
}
