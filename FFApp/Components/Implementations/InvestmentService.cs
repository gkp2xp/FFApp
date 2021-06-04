using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Components
{
    public class InvestmentService: IInvestmentService
    {
        private readonly ILogger<InvestmentService> _logger;
        private readonly IDataLoader _dataloader;
        public InvestmentService(IDataLoader dataloader, ILogger<InvestmentService> logger) {
            _logger = logger;
            _dataloader = dataloader;
        }

        public IEnumerable<Entities.Investment> Fetch() {
            _logger.LogInformation("Fetching investments from loader");

            return _dataloader.LoadInvestments();
        }
        public Entities.Investment Get(Guid investmentId) {
            _logger.LogInformation($"Getting investment for id: {investmentId}");

            var investment = Fetch().FirstOrDefault(i => i.Id == investmentId);
            if (investment == null) {
                _logger.LogInformation($"Investment id: {investmentId} was not found.");
                return null;
            }

            return investment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="investment">Investment</param>
        /// <param name="numberOfDays">Number of Days of investment</param>
        /// <param name="ear">Expected Annual Returning in percentage (e.g: 10 -> 10%, 125 -> 12.5%, 10000 -> 100%)</param>
        /// <returns></returns>
        public decimal ComputeExpectedAnnualReturn(Entities.Investment investment, int numberOfDays, int ear)
        {
            if (investment == null) throw new ArgumentNullException(nameof(investment));
            if (numberOfDays < 0) throw new ArgumentException(nameof(numberOfDays), "Number of days should be positif");

            var expectedReturn =  (double)ear / 100;
            var b = (double)(numberOfDays / 365);
            var cumulativeReturn = (decimal)Math.Pow(expectedReturn + 1, b) - 1;

            return investment.CurrentValue * (1 + cumulativeReturn);
        }
    }
}
