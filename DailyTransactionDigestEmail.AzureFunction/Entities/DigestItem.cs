using PlaidApi.Entities;
using System;

namespace DailyTransactionDigestEmail.AzureFunction.Entities
{
    public class DigestItem
    {
        public InstitutionAccount InstitutionAccount { get; set; }
        public GetTransactionsResult Transactions { get; set; }
        public bool Successful => Exception == null;
        public Exception Exception { get; set; }
    }
}