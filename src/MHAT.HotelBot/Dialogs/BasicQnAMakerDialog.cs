using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Dialogs
{
    [Serializable]
    public class BasicQnAMakerDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("請輸入您的問題");

            context.Wait(MessageReceviedAsync);
        }

        private async Task MessageReceviedAsync
            (IDialogContext context, IAwaitable<object> result)
        {
            var message = context.Activity;

            var qnaKBId = ConfigurationManager
                .AppSettings["QnAKnowledgebaseId"];
            var endpointHostName = ConfigurationManager
                .AppSettings["QnAEndpointHostName"];

            if (!string.IsNullOrEmpty(qnaKBId))
            {
                if (string.IsNullOrEmpty(endpointHostName))
                    await context.Forward(new BasicQnAMakerPreviewDialog(),
                        AfterAnswerAsync, message, CancellationToken.None);
                else
                    await context.Forward(new BasicQnAMakerDialogGA(),
                        AfterAnswerAsync, message, CancellationToken.None);
            }
            else
            {
                await context.PostAsync("請設定好QnA Maker的knowledge id以及key");
            }
        }

        private Task AfterAnswerAsync(IDialogContext context,
            IAwaitable<IMessageActivity> result)
        {
            context.Done(default(object));
            return Task.CompletedTask;
        }

        public static string GetNoFoundText()
        {
            var message = "没找到您问题的答案。";

            var key = "DefaultNoFoundText";

            if (ConfigurationManager.AppSettings[key] != null
                && string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) == false)
            {
                message = ConfigurationManager.AppSettings[key].ToString();
            }

            return message;
        }

        // Dialog for QnAMaker Preview service
        [Serializable]
        public class BasicQnAMakerPreviewDialog : QnAMakerDialog
        {
            // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
            // Parameters to QnAMakerService are:
            // Required: subscriptionKey, knowledgebaseId, 
            // Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
            public BasicQnAMakerPreviewDialog() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnAAuthKey"], ConfigurationManager.AppSettings["QnAKnowledgebaseId"], BasicQnAMakerDialog.GetNoFoundText(), 0.5)))
            { }
        }

        // Dialog for QnAMaker GA service
        [Serializable]
        public class BasicQnAMakerDialogGA : QnAMakerDialog
        {
            // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
            // Parameters to QnAMakerService are:
            // Required: qnaAuthKey, knowledgebaseId, endpointHostName
            // Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
            public BasicQnAMakerDialogGA() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnAAuthKey"], ConfigurationManager.AppSettings["QnAKnowledgebaseId"], BasicQnAMakerDialog.GetNoFoundText(), 0.5, 1, ConfigurationManager.AppSettings["QnAEndpointHostName"])))
            { }

        }
    }
}