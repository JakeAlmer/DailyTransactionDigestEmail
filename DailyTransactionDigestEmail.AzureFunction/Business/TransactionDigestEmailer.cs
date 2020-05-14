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
               .Select(async a =>
               {
                   var result = new DigestItem { InstitutionAccount = a };
                   try
                   {
                       result.Transactions = await plaidApi.GetTransactionsAsync(access_token: a.AccessToken,
                                                                              startDate: DateTime.Today.AddDays(-1),
                                                                              endDate: DateTime.Today.AddDays(-1));
                   }
                   catch (Exception ex)
                   {
                       result.Exception = ex;
                   }

                   return result;
               });
            var accountTransactionResults = await Task.WhenAll(accountTransactionResultsTasks);

            await SendEmail(accountTransactionResults);
        }

        private async Task SendEmail(DigestItem[] digestItems)
        {
            string body = $"<table>";
            foreach (var result in digestItems)
            {
                if (result.Successful)
                {
                    foreach (var account in result.Transactions.accounts)
                    {
                        var accountTransactions = result.Transactions.transactions.Where(p => p.account_id == account.account_id).ToList();
                        if (accountTransactions.Any())
                        {
                            body += $"<tr><td colspan='3'><b>{result.InstitutionAccount.InstitutionNickName} - {account.official_name ?? account.name}</b></td></tr>";
                            foreach (var transaction in accountTransactions)
                            {
                                body += $"<tr style='color:{(transaction.pending ? "darkgray" : "black")};'>";
                                body += $"<td>{transaction.name}</td>";
                                body += $"<td style='text-align:right;padding-left:10px;'>{transaction.amount:c}</td>";
                                body += $"<td>{(transaction.pending ? "(pending)" : "")}</td>";
                                body += "</tr>";
                            }
                        }
                    }
                }
                else
                {
                    body += $"<tr><td colspan='3'><b>{result.InstitutionAccount.InstitutionNickName}</b></td></tr>";
                    body += $"<tr><td colspan='3'>{result.Exception}</td></tr>";
                }
            }
            body += $"</table>";
            var response = await _email
                                .To(Environment.GetEnvironmentVariable("report_email"))
                                .Subject($"Daily Transaction Digest for {DateTime.Today.AddDays(-1).ToShortDateString()}")
                                .Body(body, isHtml: true)
                                .SendAsync();

            if (!response.Successful) 
                throw new Exception("failed to send email: " + response.ErrorMessages[0]);
        }
    }
}
