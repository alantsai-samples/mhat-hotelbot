using MHAT.HotelBot.Helper;
using MHAT.HotelBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Dialogs
{
    [Serializable]
    public class DrinkPriceCheckerDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("請上傳飲料圖片或者圖片的網址");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync
            (IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var CustomVisionServiceInstance = 
                new CustomVisionService
                    (ConfigurationManager.AppSettings["CustomVision.ProjectId"],
                    ConfigurationManager.AppSettings["CustomVision.Key"]);

            var messageResult = await result;

            var connector = messageResult.GetConnector();

            var finalResult = string.Empty;

            var imageAttachment = messageResult
                .Attachments
                ?.FirstOrDefault
                    (a => a.ContentType.Contains("image"));

            if (imageAttachment != null)
            {
                using (var stream = await connector.GetImageStream(imageAttachment))
                {
                    finalResult = await CustomVisionServiceInstance.GetTag(stream);
                }
            }
            else if (Uri.IsWellFormedUriString(messageResult.Text, UriKind.Absolute))
            {
                finalResult = await CustomVisionServiceInstance.GetTag(messageResult.Text);
            }

            switch (finalResult)
            {
                case "coke":
                    finalResult = "可樂：20元";
                    break;
                case "sprite":
                    finalResult = "雪碧：10元";
                    break;
                case "pepsi":
                    finalResult = "百事可樂：50元";
                    break;
                default:
                    finalResult = "找不到對應的飲料，請重新拍照";
                    break;
            }

            context.Done(finalResult);
        }
    }
}