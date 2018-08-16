using MHAT.HotelBot.Models;
using MHAT.HotelBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Dialogs
{
    [Serializable]
    public class SpeechTranslationDialog : IDialog<List<ResponseModel>>
    {
        public string BaseRecordPath { get { return Path.Combine(GetBaseBinPath(), @"App_Data\Record\"); } }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("請輸入：media@RecordMediaPath");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync
           (IDialogContext context, IAwaitable<IMessageActivity> inResult)
        {
            var activity =  context.Activity as IMessageActivity;

            var textSplit = activity.Text.Split('@');

            var mediaUrl = textSplit.Last();

            var tranlsatorService =
                new TranslatorSpeechService
                    (ConfigurationManager.AppSettings["TranslatorSpeechApiKey"]);

            var result = await tranlsatorService.TranslateSpeech(mediaUrl);

            context.Done(result);
        }

        public string GetBaseBinPath()
        {
            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.BaseDirectory;
            return basePath;
        }

    }

}