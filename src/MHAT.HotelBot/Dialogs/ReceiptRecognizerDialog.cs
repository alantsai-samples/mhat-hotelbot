using MHAT.HotelBot.Helper;
using MHAT.HotelBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Dialogs
{
    [Serializable]
    public class ReceiptRecognizerDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("請上傳發票圖片或者發票圖片的網址");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync
            (IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var messageResult = await result;

            var cvs = new OCRService();

            var finalResult = string.Empty;

            // 上傳圖片的處理
            if (messageResult.Attachments?.Any(a => a.ContentType.Contains("image")) ?? false)
            {
                var attachment =
                    messageResult.Attachments.FirstOrDefault(x => x.ContentType.Contains("image"));

                var imageStream = await
                    messageResult.GetConnector().GetImageStream(attachment);

                var ocrResult = await cvs.GetOcrResultAsync(imageStream, "zh-Hant");

                finalResult = ProcessImageOcrResult(context, ocrResult);
            }
            // 圖片網址的處理
            else if (Uri.IsWellFormedUriString(messageResult.Text, UriKind.Absolute))
            {
                var ocrResult = await cvs.GetOcrResultAsync(messageResult.Text, "zh-Hant");

                finalResult = ProcessImageOcrResult(context, ocrResult);
            }

            context.Done(finalResult);
        }

        private string ProcessImageOcrResult(IDialogContext context, Microsoft.ProjectOxford.Vision.Contract.OcrResults ocrResult)
        {
            var result = string.Empty;

            // 偷懶，發票號碼格式是：AA-12345678
            // 因此找出第3個字母是-的就算是發票號碼
            var foundErrorCode = ocrResult.Regions.SelectMany(x => x.Lines)
                                .SelectMany(x => x.Words)
                                .FirstOrDefault(x => x.Text.Length > 3 
                                    && x.Text.Substring(2, 1) == "-");

            if (foundErrorCode != null)
            {
                result = foundErrorCode.Text;
            }

            return result;
        }
    }
}