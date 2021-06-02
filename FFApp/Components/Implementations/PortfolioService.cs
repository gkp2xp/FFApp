using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Components
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IDataLoader _dataloader;
        private readonly ILogger<PortfolioService> _logger;
        public PortfolioService(IDataLoader dataloader, ILogger<PortfolioService> logger)
        {
            _dataloader = dataloader;
            _logger = logger;
        }

        public IEnumerable<Entities.InvestmentBreakdown> GetBreakdown()
        {
            var investments = _dataloader.LoadInvestments()
                                         .ToList();

            var groupings = investments.GroupBy(i => i.PortfolioGrouping)
                                        .OrderBy(i => i.Key.Parent == null ? 0 : i.Key.Parent.Id)
                                        .ThenBy(i => i.Key.Id)
                                        .Select((i, index) => new { PortfolioGroup = i.Key, Investments = i.ToList() });

            var result = new List<Entities.InvestmentBreakdown>();

            foreach (var grouping in groupings)
            {
                var breakdownPortfolioGrouping = new Entities.InvestmentBreakdown {
                    Id = grouping.PortfolioGroup.Id.ToString(),
                    Label = grouping.PortfolioGroup.Label,
                    DisplayType = Entities.DisplayType.PortfolioGrouping,
                    Value = grouping.Investments.Sum(i => i.CurrentValue),
                    LinkId = string.Empty,
                    Hierachy = string.Empty,
                };

                result.Add(breakdownPortfolioGrouping);

                breakdownPortfolioGrouping.Hierachy = BuildHierachy(breakdownPortfolioGrouping, grouping.PortfolioGroup, result) + breakdownPortfolioGrouping.Id + "/";

                var breakdownInvestments = grouping.Investments.Select((investment, index) => new Entities.InvestmentBreakdown {
                    Id = investment.Id.ToString(),
                    Label = investment.Label,
                    DisplayType = Entities.DisplayType.Investment,
                    Value = investment.CurrentValue,
                    LinkId = breakdownPortfolioGrouping.Id,
                    Hierachy = breakdownPortfolioGrouping.Hierachy + index.ToString() + "/",
                });

                result.AddRange(breakdownInvestments);
            }

            return result;
        }

        private int GetHierachyLevel(Entities.PortfolioGrouping grouping)
        {
            var hierachyLevel = 0;
            var current = grouping.Parent;
            while (current != null) {
                hierachyLevel++;
                current = current.Parent;
            }

            return hierachyLevel;
        }

        private string BuildHierachy(Entities.InvestmentBreakdown current, Entities.PortfolioGrouping grouping, List<Entities.InvestmentBreakdown> result)
        {
            if (grouping == null || grouping.Parent == null) {
                return "/";
            }

            var parent = grouping.Parent;
            var investmentBreakDownParent = result.FirstOrDefault(r => r.Id == parent.Id.ToString());
            if (investmentBreakDownParent == null) {
                investmentBreakDownParent = new Entities.InvestmentBreakdown {
                    Id = parent.Id.ToString(),
                    Label = parent.Label,
                    DisplayType = Entities.DisplayType.PortfolioGrouping,
                    Value = 0,
                    LinkId = string.Empty,
                    Hierachy = string.Empty
                };
                result.Add(investmentBreakDownParent);
                investmentBreakDownParent.Hierachy = BuildHierachy(investmentBreakDownParent, parent.Parent, result) + investmentBreakDownParent.Id + "/";
            }

            current.LinkId = investmentBreakDownParent.Id;
            investmentBreakDownParent.Value += current.Value;

            return investmentBreakDownParent.Hierachy;

        }
    }
}
