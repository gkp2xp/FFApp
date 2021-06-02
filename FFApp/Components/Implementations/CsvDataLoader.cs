using FFApp.Configs;
using FFApp.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Components
{
    public class CsvDataLoader : IDataLoader
    {
        private readonly CsvDataloaderOptions _configuration = null;
        private readonly ILogger<PortfolioService> _logger;

        public CsvDataLoader(ILogger<PortfolioService> logger, IOptions<CsvDataloaderOptions> options)
        {
            _configuration = options.Value;
            _logger = logger;
        }

        private PortfolioGrouping GetUnknownPortfolio(List<PortfolioGrouping> portfolioGroupings)
        {
            var unknownPortfolioId = _configuration.UnknownPortfolioId;

            var unknowPortofio = portfolioGroupings.FirstOrDefault(p => p.Id == unknownPortfolioId);
            if (unknowPortofio == null) throw new Exception("Cannot find Unknow Portfolio");

            return unknowPortofio;
        }

        public IEnumerable<Investment> LoadInvestments()
        {
            var portfolioGroupings = LoadPortfolioGroupings()
                                        .OrderBy(p => p.Id).ToList();

            var unknownPortfolio = GetUnknownPortfolio(portfolioGroupings);

            var investments = new List<Investment>();
            PortfolioGrouping pg = null;

            foreach (var data in Helpers.CsvReader.Read(_configuration.InvestmentFilename, _configuration.InvestmentFileHasHeader))
            {
                var investment = ParseInvestment(data);
                if (pg == null || pg.Id != investment.PortfolioGroupingId) {
                    pg = portfolioGroupings.FirstOrDefault(p => p.Id == investment.PortfolioGroupingId);
                }

                if (pg == null) {
                    investment.PortfolioGrouping = unknownPortfolio;
                    investment.PortfolioGroupingId = unknownPortfolio.Id;                        
                } else {
                    investment.PortfolioGrouping = pg;
                }
                
                investments.Add(investment);
            }

            return investments;
        }

        public IEnumerable<PortfolioGrouping> LoadPortfolioGroupings()
        {
            var portfolioGroupings = new List<PortfolioGrouping>();
            PortfolioGrouping parent = null;

            foreach( var data in Helpers.CsvReader.Read(_configuration.PortfolioGroupingFilename, _configuration.PortfolioGroupingHasHeader)) {
                var pg = ParsePortfolio(data);
                if (parent == null || parent.Id != pg.ParentId) {
                    parent = portfolioGroupings.FirstOrDefault(p => p.Id == pg.ParentId);
                }
                pg.Parent = parent;
                portfolioGroupings.Add(pg);
            }

            return portfolioGroupings;
        }

        private PortfolioGrouping ParsePortfolio(string[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data), "");
            if (data.Length != 3) throw new ArgumentOutOfRangeException(nameof(data), "");

            if (!int.TryParse(data[0], out int id)) {
                _logger.LogError($"Portfolio id: {data[0]} is not valid");
                throw new Exception("");
            }


            int parentId;
            if (string.Compare(data[1], "NULL", StringComparison.OrdinalIgnoreCase) == 0) {
                parentId = 0;
            }
            else if (!int.TryParse(data[1], out parentId)) {
                _logger.LogError($"Portfolio parentId: {data[1]} is not valid");
                throw new Exception("");
            }
            

            return new PortfolioGrouping { Id = id, Label = data[2], ParentId = parentId };
        }

        private Investment ParseInvestment(string[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data), "");
            if (data.Length != 4) throw new ArgumentOutOfRangeException(nameof(data), "");

            if (!Guid.TryParse(data[0], out Guid id))
            {
                _logger.LogError($"Investment Id: {data[0]} is not valid");
                throw new Exception("");
            }

            if (!decimal.TryParse(data[2], out decimal currentValue))
            {
                _logger.LogError($"Investment currentValue: {data[2]} is not valid");
                throw new Exception("");
            }

            if (!int.TryParse(data[3], out int portfolioGroupinId))
            {
                _logger.LogError($"Investment portfolioGroupinId: {data[3]} is not valid");
                throw new Exception("");
            }


            return new Investment { 
                Id = id,
                Label = data[1], 
                CurrentValue = currentValue,
                PortfolioGroupingId = portfolioGroupinId,
            };
        }
    }
}
