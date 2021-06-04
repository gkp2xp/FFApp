using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Components
{
    public interface IInvestmentService
    {
        IEnumerable<Entities.Investment> Fetch();
        Entities.Investment Get(Guid investmentId);
        decimal ComputeExpectedAnnualReturn(Entities.Investment investment, int numberOfDays, int expectedAnnualReturn);
    }
}
