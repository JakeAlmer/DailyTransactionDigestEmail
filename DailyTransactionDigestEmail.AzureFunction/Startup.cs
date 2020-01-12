using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DailyTransactionDigestEmail.AzureFunction.Startup))]
namespace DailyTransactionDigestEmail.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddFluentEmail("noreply@almer.us", "Transaction Digest")
                   .AddSendGridSender(Environment.GetEnvironmentVariable("SendGridApiKey"));
        }
    }
}
