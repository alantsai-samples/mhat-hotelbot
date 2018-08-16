using MHAT.HotelBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Services
{
    public class TranslatorSpeechService
    {
        static string host = "wss://dev.microsofttranslator.com";
        static string path = "/speech/translate";

        public string SubscriptionKey { get; }

        public TranslatorSpeechService(string subscriptionKey)
        {
            SubscriptionKey = subscriptionKey;
        }

        async Task Send(ClientWebSocket client, string input_path)
        {
            var audio = File.ReadAllBytes(input_path);
            var audio_out_buffer = new ArraySegment<byte>(audio);

            await client.SendAsync(audio_out_buffer, WebSocketMessageType.Binary, true, CancellationToken.None);

            /* Make sure the audio file is followed by silence.
             * This lets the service know that the audio input is finished. */
            var silence = new byte[32000];
            var silence_buffer = new ArraySegment<byte>(silence);
            await client.SendAsync(silence_buffer, WebSocketMessageType.Binary, true, CancellationToken.None);

            //await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        async Task<List<ResponseModel>> Receive(ClientWebSocket client)
        {
            var outputResult = new List<ResponseModel>();

            var inbuf = new byte[102400];
            var segment = new ArraySegment<byte>(inbuf);

            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(segment, CancellationToken.None);
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Close:
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        break;
                    case WebSocketMessageType.Text:
                        var text = Encoding.UTF8.GetString(inbuf).TrimEnd('\0');
                        var addModel = JsonConvert.DeserializeObject<ResponseModel>(text);
                        outputResult.Add(addModel);
                        if (result.EndOfMessage)
                        {
                            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        }
                        break;
                    case WebSocketMessageType.Binary:
                        break;
                }
            }

            return outputResult;
        }

        public async Task<List<ResponseModel>> TranslateSpeech(string input_path)
        {
            var client = new ClientWebSocket();
            client.Options.SetRequestHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);

            string from = "en-US";
            string to = "zh-Hant";
            string api = "1.0";

            string uri = host + path +
                "?from=" + from +
                "&to=" + to +
                "&api-version=" + api;

            await client.ConnectAsync(new Uri(uri), CancellationToken.None);

            var send = Send(client, input_path);
            var receive = Receive(client);

            await Task.WhenAll(send, receive);

            var result = await receive;

            return result;
        }
    }
}