using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Helper
{
    public static class IConnectorClientHelper
    {

        public static async Task<Stream> GetImageStream
            (this IConnectorClient connector, Attachment imageAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(imageAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", 
                            await connector.GetTokenAsync());

                    httpClient.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue
                            ("application/octet-stream"));
                }

                return await httpClient.GetStreamAsync(uri);
            }
        }

        public static async Task<string> GetTokenAsync(this IConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
            {
                return await credentials.GetTokenAsync();
            }

            return null;
        }
    }
}