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
                        var heroCardResult = await r;
                        returnMessage.Attachments = new List<Attachment>(){ heroCardResult.ToAttachment() };

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
                else if(activity.Text == "查飯店v2")
                {
                    var returnMessage = activity.CreateReply();

                    

                    returnMessage.Attachments.Add(new Attachment()
                    {
                        Content = card,
                        ContentType = AdaptiveCard.ContentType
                    });

                    await context.PostAsync(returnMessage);
                    context.Wait(MessageReceivedAsync);
                }
                else if(activity.Text == "訂房v2")
                {
                    var reserveRoomForm = 
                        FormDialog.FromForm(RoomReservation.BuildForm,
                            FormOptions.PromptInStart);

                    context.Call(reserveRoomForm, AfterReserveRoomAsync);
                }
                else
                {
                    // 已經有姓名直接輸出 姓名 + 輸入内容
                    await context.PostAsync($"{name}: {activity.Text}");

                    context.Wait(MessageReceivedAsync);
                }
            }
        }

        private async Task GreetingAfterAsync(IDialogContext context,
            IAwaitable<string> result)
        {
            var name = await result;

            context.UserData.SetValue<string>("Name", name);

            await context.PostAsync($"{name} 您好，能夠幫助您什麽");

            context.Wait(MessageReceivedAsync);
        }

        private async Task AfterReserveRoomAsync(IDialogContext context
            , IAwaitable<RoomReservation> result)
        {
            RoomReservation reservation = null;

            try
            {
                reservation = await result;

                await context.PostAsync($"得到的結果：{Environment.NewLine} {JsonConvert.SerializeObject(reservation)}");
            }
            catch(FormCanceledException<RoomReservation> ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = $"您在 {ex.Last} 的時候退出了 -- 如果有遇到任何問題請告訴我們";
                }
                else
                {
                    reply = "機器人暫時罷工了，請稍後嘗試";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}