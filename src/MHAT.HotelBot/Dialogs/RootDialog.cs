using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdaptiveCards;
using MHAT.HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace MHAT.HotelBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            context.UserData.TryGetValue<string>("Name", out string name);

            if(string.IsNullOrEmpty(name))
            {
                context.Call(new NameDialog(), GreetingAfterAsync);
            }
            else
            {
                if (activity.Text == "查飯店")
                {
                    context.Call(new SearchHotelDialog(), async (ctx, r) =>
                    {
                        var returnMessage = activity.CreateReply();
                        var attachments = await r;
                        returnMessage.Attachments = attachments;

                        await context.PostAsync(returnMessage);
                        context.Wait(MessageReceivedAsync);
                    });
                }
                else if(activity.Text == "明細")
                {
                    context.Call(new GetReceiptDialog(), async (ctx, r) =>
                    {
                        var returnMessage = activity.CreateReply();
                        var attachmentResult = await r;
                        returnMessage.Attachments = new List<Attachment>() { attachmentResult };

                        await context.PostAsync(returnMessage);
                        context.Wait(MessageReceivedAsync);
                    });
                }
                else if(activity.Text == "訂房")
                {
                    context.Call(new ReserveRoomDialog(), ReserverRoomAfterAsync);
                }
                else
                {
                    // 已經有姓名直接輸出 姓名 + 輸入内容
                    await context.PostAsync($"{name}: {activity.Text}");

                    context.Wait(MessageReceivedAsync);
                }
            }
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

                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ConfirmReservation(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmResult = await result;

            if(confirmResult)
            {
                await context.PostAsync($"訂單完成。訂單號：{DateTime.Now.Ticks}");
            }
            else
            {
                await context.PostAsync("訂房取消");
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task GreetingAfterAsync(IDialogContext context,
            IAwaitable<string> result)
        {
            var name = await result;

            context.UserData.SetValue<string>("Name", name);

            await context.PostAsync($"{name} 您好，能夠幫助您什麽");

            context.Wait(MessageReceivedAsync);
        }
    }
}