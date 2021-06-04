using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace FFApp.Test
{
    public class InvestmentControllerTest
    {
        public ServiceProvider serviceProvider;

        private TestServer _server;
        public HttpClient Client { get; private set; }

        [SetUp]
        public void Setup()
        {
            var sc = new ServiceCollection();

            sc.AddMemoryCache();
            sc.AddLogging();
            sc.AddScoped<Components.IDataLoader, Components.CsvDataLoader>();
            sc.AddScoped<Components.IPortfolioService, Components.PortfolioService>();
            sc.AddScoped<Components.IInvestmentService, Components.InvestmentService>();

            serviceProvider = sc.BuildServiceProvider();

            SetUpClient();
        }

        [Test]
        public void ComputeExpectedReturn()
        {
            var service = serviceProvider.GetService<Components.IInvestmentService>();
            var investment = new Entities.Investment { 
                Id = Guid.NewGuid(),
                Label = "Investment Test1",
                CurrentValue = 1000
            };

            var projectedReturn = service.ComputeExpectedAnnualReturn(investment, 365, 5);
            Assert.AreEqual(1050m, projectedReturn);
        }

        [Test]
        public async Task WebComputeExpectedReturn()
        {
            var request = new Models.ProjectModel { 
                id = new Guid("8284BCD4-ABBB-429A-A0C6-11B6C8A44677"),
                days = 365,
                expectedReturn = 5
            };
            var response0 = await Client.PostAsync("/api/investment/compute", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            response0.StatusCode.Should().BeEquivalentTo(220413.27);
        }

        [Test]
        public async Task WebComputeExpectedReturn_Investment_Does_Not_Exist()
        {
            var request = new Models.ProjectModel
            {
                id = Guid.Empty,
                days = 365,
                expectedReturn = 5
            };
            var response0 = await Client.PostAsync("/api/investment/compute", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            response0.StatusCode.Should().BeEquivalentTo(301);
        }

        [Test]
        public async Task WebComputeExpectedReturn_Negative_Days()
        {
            var request = new Models.ProjectModel
            {
                id = new Guid("8284BCD4-ABBB-429A-A0C6-11B6C8A44677"),
                days = -365,
                expectedReturn = 5
            };
            var response0 = await Client.PostAsync("/api/investment/compute", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            response0.StatusCode.Should().BeEquivalentTo(301);
        }

        [Test]
        public async Task WebFetchInvestmentList()
        {
            var response0 = await Client.GetAsync("/api/investment/list");
            response0.StatusCode.Should().BeEquivalentTo(200);

            var realData0 = JsonConvert.DeserializeObject<IEnumerable<Models.InvestmentModel>>(response0.Content.ReadAsStringAsync().Result);
            realData0.Count().Should().Equals(368);
        }

        private void SetUpClient() {

            var builder = new WebHostBuilder()
                .UseStartup<FFApp.Startup>()
                .ConfigureServices(services =>
                {

                });

            _server = new TestServer(builder);

            Client = _server.CreateClient();
        }
    }
}