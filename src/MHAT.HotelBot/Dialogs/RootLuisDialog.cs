using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Dialogs
{
    [LuisModel("<modelId>", "<key>")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None
            (IDialogContext context, LuisResult result)
        {
            await context.PostAsync("無法理解您的請求");

            context.Wait(MessageReceived);
        }

        [LuisIntent("SearchHotel")]
        public Task SearchHotel
            (IDialogContext context, LuisResult result)
        {
            context.Call(new SearchHotelDialog(), async (ctx, r) =>
            {
                var imessageActivity = ctx.Activity as Activity;

                var returnMessage = imessageActivity.CreateReply();
                var attachments = await r;
                returnMessage.Attachments = attachments;

                await context.PostAsync(returnMessage);
                context.Wait(MessageReceived);
            });

            return Task.CompletedTask;
        }
    }
}