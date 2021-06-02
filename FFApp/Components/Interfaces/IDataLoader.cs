using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Components
{
    public interface IDataLoader
    {
        IEnumerable<Entities.PortfolioGrouping> LoadPortfolioGroupings();

        IEnumerable<Entities.Investment> LoadInvestments();
    }
}
