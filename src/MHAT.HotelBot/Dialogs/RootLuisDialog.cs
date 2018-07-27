using MHAT.HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
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

        [LuisIntent("ReserveRoom")]
        public Task ReserveRoom
            (IDialogContext context, LuisResult result)
        {
            context.Call(new ReserveRoomDialog(), ReserverRoomAfterAsync);

            return Task.CompletedTask;
        }

        private async Task ReserverRoomAfterAsync(IDialogContext context,
           IAwaitable<RoomReservation> result)
        {
            var roomReserved = await result;

            if (roomReserved != null)
            {
                await context.PostAsync($"您的訂單資訊：{Environment.NewLine}" +
                    $"{JsonConvert.SerializeObject(roomReserved, Formatting.Indented)}");

                PromptDialog.Confirm(context, ConfirmReservation, "請確認訂房資訊");
            }
            else
            {
                await context.PostAsync($"訂單取得失敗");

                context.Wait(MessageReceived);
            }
        }

        private async Task ConfirmReservation(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmResult = await result;

            if (confirmResult)
            {
                await context.PostAsync($"訂單完成。訂單號：{DateTime.Now.Ticks}");
            }
            else
            {
                await context.PostAsync("訂房取消");
            }

            context.Wait(MessageReceived);
        }
    }
}