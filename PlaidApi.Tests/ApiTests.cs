using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using PlaidApi.Entities;

namespace PlaidApi.Tests
{
    public class ApiTests
    {
        private PlaidApi plaidApi;
        private IConfigurationRoot config;

        [SetUp]
        public void Setup()
        {
            config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.test.json")
                    .Build();

            plaidApi = new PlaidApi(plaid_client_id: config["plaid_client_id"],
                                    plaid_secret: config["plaid_secret"],
                                    environment: PlaidEnvironment.Development,
                                    clientApp: "plaid api unit tests");
        }

        [Test]
        public async Task GetTransactions()
        {
            var transactionsResults = await plaidApi.GetTransactionsAsync(access_token: config["access_token"],
                                                                          startDate: DateTime.Today.AddDays(-30),
                                                                          endDate: DateTime.Today,
                                                                          count: 1);
            DumpToConsole(transactionsResults);
            Assert.AreEqual(1, transactionsResults.transactions.Count());
        }

        [Test]
        public async Task GetAccounts()
        {
            var result = await plaidApi.GetAccounts(access_token: config["access_token"]);
            DumpToConsole(result);
            Assert.IsNotEmpty(result.accounts);
        }

        private void DumpToConsole(Object o)
        {
            string json = JsonConvert.SerializeObject(o);
            Console.WriteLine(json);
        }
    }
}