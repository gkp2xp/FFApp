using FFApp.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly ILogger<PortfolioController> _logger;
        private readonly IPortfolioService _portfolio;
        private readonly IMemoryCache _cache;

        private static string CACHE_ENTRY = "InvestmentBreakdown";

        public PortfolioController(ILogger<PortfolioController> logger, IPortfolioService portfolio, IMemoryCache cache) {
            _logger = logger;
            _portfolio = portfolio;
            _cache = cache;
        }

        [HttpGet]
        public IEnumerable<Models.InvestmentBreakdownModel> Get() {
            //System.Threading.Thread.Sleep(3000);
            
            List<Entities.InvestmentBreakdown> list;

            if (!_cache.TryGetValue(CACHE_ENTRY, out list)){
                _logger.LogInformation("cache missed. Cold load in process...");

                list = ColdLoad();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
                _cache.Set(CACHE_ENTRY, list, cacheEntryOptions);
            }else {
                _logger.LogInformation("Data loaded from cache");
            }

            _logger.LogInformation($"Investment Count: {list.Count}");

            return list.Select(inv => new Models.InvestmentBreakdownModel(inv));
        }

        private List<Entities.InvestmentBreakdown> ColdLoad() {
            _logger.LogInformation("Data loaded from data store");

            try
            {
                List<Entities.InvestmentBreakdown> list = new List<Entities.InvestmentBreakdown>();

                foreach (var item in _portfolio.GetBreakdown()
                                     .OrderByDescending(r => r.Value)
                                     .ThenBy(r => r.Label))
                {
                    list.Add(item);
                    var children = _portfolio.Flatterning(item);
                    if (children.Count() != 0) list.AddRange(children);
                }

                return list;
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public IEnumerable<string> Post(string investmentId, int days, int expectedReturn) {

            //Entities.Investment investment = null;



            return null;
        }
    }
}
