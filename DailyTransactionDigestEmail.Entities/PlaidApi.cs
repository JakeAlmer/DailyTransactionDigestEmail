using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlaidApi.Entities;

namespace PlaidApi
{
    public class PlaidApi
    {
        private readonly string _plaidClientId;
        private readonly string _plaidSecret;
        private readonly PlaidEnvironment _environment;

        private string ApiUrl => $"https://{_environment}.plaid.com";

        private static readonly HttpClient client = new HttpClient();

        public PlaidApi(string plaid_client_id, string plaid_secret, PlaidEnvironment environment, string clientApp, string version = "2019-05-29")
        {
            _plaidClientId = plaid_client_id;
            _plaidSecret = plaid_secret;
            _environment = environment;
        }


        public async Task<GetTransactionsResult> GetTransactionsAsync(string access_token, DateTime startDate, DateTime endDate, string[] accountIds = null, int count = 100, int offset = 0)
        {
            var postContent = new
                              {
                                  client_id = _plaidClientId,
                                  secret = _plaidSecret,
                                  access_token = access_token,
                                  start_date = startDate.ToString("yyyy-MM-dd"),
                                  end_date = endDate.ToString("yyyy-MM-dd"),
                                  options = new
                                            {
                                             //   accounts_ids = accountIds,
                                                count = count,
                                                offset = offset
                                            }
                              };
            HttpResponseMessage postResults = await client.PostAsJsonAsync(url: ApiUrl + "/transactions/get",
                                                                           data: postContent);
            await postResults.EnsureSuccessStatusCodeWithResponseContent();
            return await postResults.Content.ReadAsJsonAsync<GetTransactionsResult>();
        }

        public async Task<GetAccountsResult> GetAccounts(string access_token)
        {
            var postContent = new
                              {
                                  client_id = _plaidClientId,
                                  secret = _plaidSecret,
                                  access_token = access_token
                              };
            HttpResponseMessage postResults = await client.PostAsJsonAsync(url: ApiUrl + "/accounts/get",
                                                                           data: postContent);
            await postResults.EnsureSuccessStatusCodeWithResponseContent();
            return await postResults.Content.ReadAsJsonAsync<GetAccountsResult>();
        }
    }

    public enum PlaidEnvironment
    {
        SandBox,
        Development,
        Production
    }
}
