using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PlaidApi
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessStatusCodeWithResponseContent(this HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                string content = await message.Content.ReadAsStringAsync();
                throw new HttpRequestException($"response returned http {message.StatusCode}, body content: {content}");
            }
        }
    }
}
