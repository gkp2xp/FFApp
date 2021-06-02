using FFApp.Components;
using Microsoft.AspNetCore.Mvc;
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
        public PortfolioController(ILogger<PortfolioController> logger, IPortfolioService portfolio) {
            _logger = logger;
            _portfolio = portfolio;
        }

        [HttpGet]
        public IEnumerable<Models.InvestmentBreakdownModel> Get() {
            //System.Threading.Thread.Sleep(3000);

            var data = _portfolio.GetBreakdown().OrderBy(r => r.Hierachy);

            _logger.LogInformation($"Investment Count: {data.Count()}");

            return data.Select(inv => new Models.InvestmentBreakdownModel(inv));
        }

        [HttpPost]
        public IEnumerable<string> Post(string investmentId, int days, int expectedReturn) {

            Entities.Investment investment = null;



            return null;
        }
    }
}
