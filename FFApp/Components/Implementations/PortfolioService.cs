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
                    Children = new List<Entities.InvestmentBreakdown>(),
                };

                breakdownPortfolioGrouping.Hierachy = BuildHierachy(breakdownPortfolioGrouping, grouping.PortfolioGroup, result) + breakdownPortfolioGrouping.Id + "/";

                breakdownPortfolioGrouping.Children = grouping.Investments.Select((investment, index) => new Entities.InvestmentBreakdown {
                    Id = investment.Id.ToString(),
                    Label = investment.Label,
                    DisplayType = Entities.DisplayType.Investment,
                    Value = investment.CurrentValue,
                    LinkId = breakdownPortfolioGrouping.Id,
                    Hierachy = breakdownPortfolioGrouping.Hierachy + index.ToString() + "/",
                    Children = new List<Entities.InvestmentBreakdown>(),
                }).ToList();
            }

            return result;
        }

        public IEnumerable<Entities.InvestmentBreakdown> Flatterning(Entities.InvestmentBreakdown investmentBreakDown)
        {
            if (investmentBreakDown.Children.Count == 0) return Enumerable.Empty<Entities.InvestmentBreakdown>();
            if (!investmentBreakDown.Children.Any(c => c.DisplayType == Entities.DisplayType.PortfolioGrouping)) {
                return investmentBreakDown.Children
                                          .OrderByDescending(c => c.Value)
                                          .ThenBy(c => c.Label);
            }

            List<Entities.InvestmentBreakdown> list = new List<Entities.InvestmentBreakdown>();
            foreach (var child in investmentBreakDown.Children.OrderByDescending(c => c.Value)
                                              .ThenBy(c => c.Label))
            {
                list.Add(child);
                list.AddRange(Flatterning(child));
            }

            return list;
        }


        private string BuildHierachy(Entities.InvestmentBreakdown current, Entities.PortfolioGrouping grouping, List<Entities.InvestmentBreakdown> result)
        {
            if (grouping != null && grouping.Parent == null) {
                result.Add(current);
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
                    Hierachy = string.Empty,
                    Children = new List<Entities.InvestmentBreakdown> { current }
                };                
                investmentBreakDownParent.Hierachy = BuildHierachy(investmentBreakDownParent, parent, result) + investmentBreakDownParent.Id + "/";
            }
            else {
                investmentBreakDownParent.Children.Add(current);
            }

            current.LinkId = investmentBreakDownParent.Id;
            investmentBreakDownParent.Value += current.Value;

            return investmentBreakDownParent.Hierachy;
        }
    }
}
