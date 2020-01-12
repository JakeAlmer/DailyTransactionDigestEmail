using System;
using System.Collections.Generic;
using System.Text;

namespace DailyTransactionDigestEmail.AzureFunction.Entities
{
    public class InstitutionAccount
    {
        public string InstitutionNickName { get; set; }
        public string AccessToken { get; set; }
    }
}
