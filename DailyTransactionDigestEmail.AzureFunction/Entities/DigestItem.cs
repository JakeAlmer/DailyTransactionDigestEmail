using PlaidApi.Entities;

namespace DailyTransactionDigestEmail.AzureFunction.Entities
{
    public class DigestItem
    {
        public InstitutionAccount InstitutionAccount { get; set; }
        public GetTransactionsResult Transactions { get; set; }
    }
}