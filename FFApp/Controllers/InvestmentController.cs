using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : ControllerBase
    {
        private readonly ILogger<InvestmentController> _logger;
        private readonly Components.IInvestmentService _investmentService;
        public InvestmentController(Components.IInvestmentService investmentService, ILogger<InvestmentController> logger) {

            _investmentService = investmentService;

            _logger = logger;
        }

        [HttpGet]
        [Route("list")]
        public IEnumerable<Models.InvestmentModel> FetchInvestments()
        {
            _logger.LogInformation("Fetching investments...");

            try
            {
                var investments = _investmentService.Fetch();
                _logger.LogInformation($"Investments count: {investments.Count()}");

                return investments.Select(investment => new Models.InvestmentModel(investment));
            }
            catch (Exception ex) {
                _logger.LogError("Failed to fetch investments.", ex);
                throw;
            }
        }

        [HttpPost]
        [Route("compute")]
        public decimal Compute(Models.ProjectModel request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.days <= 0) throw new Exception("");
            if (request.expectedReturn <= 0) throw new Exception("");

            var investment = _investmentService.Get(request.id);
            if (investment == null) return 0m;

            return _investmentService.ComputeExpectedAnnualReturn(investment, request.days, request.expectedReturn);
        }
    }
}
