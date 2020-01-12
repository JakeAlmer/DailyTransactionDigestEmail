using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DailyTransactionDigestEmail.AzureFunction.Entities;
using FluentEmail.Core;
using Newtonsoft.Json;
using PlaidApi;
using PlaidApi.Entities;

namespace DailyTransactionDigestEmail.AzureFunction.Business
{
    public class TransactionDigestEmailer
    {
        private readonly IFluentEmail _email;

        public TransactionDigestEmailer(IFluentEmail email)
        {
            _email = email;
        }

        public async Task SendDailyEmail()
        {
            string accountsJson = Environment.GetEnvironmentVariable("accounts");
            List<InstitutionAccount> accounts = JsonConvert.DeserializeObject<List<InstitutionAccount>>(accountsJson);
            var plaidApi = new PlaidApi.PlaidApi(plaid_client_id: Environment.GetEnvironmentVariable("plaid_client_id"),
                                                 plaid_secret: Environment.GetEnvironmentVariable("plaid_secret"),
                                                 environment: PlaidEnvironment.Development,
                                                 clientApp: "TransactionDigestEmailer");

            // get the transactions for yesterday for each account
            // its not perfect, if an account has issues later days won't catch up on those transactions but this is easy
            var accountTransactionResultsTasks = accounts
               .Select(async a => new DigestItem
                                  {
                                      InstitutionAccount = a,
                                      Transactions = await plaidApi.GetTransactionsAsync(access_token: a.AccessToken,
                                                                                         startDate: DateTime.Today.AddDays(-1),
                                                                                         endDate: DateTime.Today.AddDays(-1))
                                  });
            var accountTransactionResults = await Task.WhenAll(accountTransactionResultsTasks);

            await SendEmail(accountTransactionResults);
        }

        private async Task SendEmail(DigestItem[] digestItems)
        {
            string body = $"<table>";
            foreach (var result in digestItems)
            {
                foreach (var account in result.Transactions.accounts)
                {
                    var accountTransactions = result.Transactions.transactions.Where(p => p.account_id == account.account_id).ToList();
                    body += $"<tr><td colspan='2'><b>{result.InstitutionAccount.InstitutionNickName} - {account.official_name ?? account.name}</b></td></tr>";
                    if (accountTransactions.Any())
                    {
                        foreach (var transaction in accountTransactions)
                        {
                            body += "<tr>";
                            body += $"<td>{transaction.name}</td>";
                            body += $"<td style='text-align:right;padding-left:10px;'>{transaction.amount:c}</td>";
                            body += "</tr>";
                        }
                    }
                    else
                    {
                        body += $"<tr><td colspan='2'><i>No transactions found for {DateTime.Today.AddDays(-1).ToShortDateString()}</i></td></tr>";
                    }
                }
            }
            body += $"</table>";
            var response = await _email
                                .To("jake@almer.us", "Jake Almer")
                                .Subject($"Daily Transaction Digest for {DateTime.Today.AddDays(-1).ToShortDateString()}")
                                .Body(body, isHtml: true)
                                .SendAsync();

            if (!response.Successful) 
                throw new Exception("failed to send email: " + response.ErrorMessages[0]);
        }
    }
}
