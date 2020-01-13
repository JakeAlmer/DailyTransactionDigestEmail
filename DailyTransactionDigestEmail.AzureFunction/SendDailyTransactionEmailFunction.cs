using System;
using System.Threading.Tasks;
using DailyTransactionDigestEmail.AzureFunction;
using DailyTransactionDigestEmail.AzureFunction.Business;
using FluentEmail.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DailyTransactionDigestEmail
{
    public class SendDailyTransactionEmailFunction
    {
        private readonly IFluentEmail _email;

        public SendDailyTransactionEmailFunction(IFluentEmail email)
        {
            _email = email;
        }

        [FunctionName("SendDailyTransactionEmailFunction")]
        public async Task Run(
            [TimerTrigger("0 0 7 * * *"  // 7am everyday
#if DEBUG
                , RunOnStartup=true
#endif
            )]TimerInfo timer,
            ILogger log)
        {
            TransactionDigestEmailer digestEmailer = new TransactionDigestEmailer(_email);
            await digestEmailer.SendDailyEmail();
        }
    }
}
