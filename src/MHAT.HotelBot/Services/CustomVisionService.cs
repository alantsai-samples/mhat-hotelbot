using MHAT.HotelBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Services
{
    public class CustomVisionService
    {
        public string ProjectId { get; }
        public string PredictionKey { get; }

        public string PredictionBaseUrl
        {
            get
            {
                return $"https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/{ProjectId}";
            }
        }

        public string PredictionImageUrl
        {
            get
            {
                return $"{PredictionBaseUrl}/image";
            }
        }

        public string PredictionImageUrlUrl
        {
            get
            {
                return $"{PredictionBaseUrl}/url";
            }
        }

        public CustomVisionService(string projectId, string predictionKey)
        {
            ProjectId = projectId;
            PredictionKey = predictionKey;
        }

        public async Task<string> GetTag(string imageUrl)
        {
            var result = string.Empty;

            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", PredictionKey);

            // Prediction URL - replace this example URL with your valid prediction URL.
            string url = PredictionImageUrlUrl;

            HttpResponseMessage response;

            using (var content = new StringContent($"{{ \"Url\": \"{imageUrl}\"}}"))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(url, content);
                var json = await response.Content.ReadAsStringAsync();
                result = GetMostPossibleTagName(json);
            }

            return result;
        }

        public async Task<string> GetTag(Stream stream)
        {
            var result = string.Empty;

            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", PredictionKey);

            // Prediction URL - replace this example URL with your valid prediction URL.
            string url = PredictionImageUrl;

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetStreamAsByteArray(stream);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                var json = await response.Content.ReadAsStringAsync();
                result = GetMostPossibleTagName(json);
            }

            return result;
        }

        private string GetMostPossibleTagName(string json)
        {
            var model = JsonConvert.DeserializeObject<PredicationResponse>(json);

            return $"{model.predictions.FirstOrDefault().tagName}";
        }

        private byte[] GetStreamAsByteArray(Stream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}