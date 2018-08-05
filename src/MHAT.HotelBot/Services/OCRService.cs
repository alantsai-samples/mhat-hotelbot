using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Services
{
    public class OCRService
    {
        public OCRService()
        {
            VisionServiceClientInstance =
                 new VisionServiceClient
                    (ConfigurationManager.AppSettings["ComputerVision.Key"],
                    ConfigurationManager.AppSettings["ComputerVision.Url"]);
        }

        public VisionServiceClient VisionServiceClientInstance { get; }

        public async Task<OcrResults> GetOcrResultAsync
            (Stream imageStream, string languageCode = "unk")
        {
            return await VisionServiceClientInstance.RecognizeTextAsync(imageStream, languageCode);
        }

        public async Task<OcrResults> GetOcrResultAsync
            (string imageUrl, string languageCode = "unk")
        {
            return await VisionServiceClientInstance.RecognizeTextAsync
                (imageUrl, languageCode);
        }
    }
}