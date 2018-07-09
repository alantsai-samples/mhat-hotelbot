using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

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
                context.PrivateConversationData.
                    TryGetValue<bool>("IsAskName", out bool isAskName);

                if(isAskName)
                {
                    context.UserData.SetValue<string>("Name", activity.Text);

                    await context.PostAsync($"{activity.Text} 您好，能夠幫助您什麽");
                }
                else
                {
                    context.PrivateConversationData.SetValue<bool>("IsAskName", true);

                    await context.PostAsync("您的名字是？");
                }
            }
            else
            {
                if (activity.Text == "查飯店")
                {
                    // 返回飯店的圖片以及可以打開官網的按鈕

                    // 建立一個回復
                    var returnMessage = activity.CreateReply();

                    // 建立一個HeroCard
                    var herocard = new HeroCard()
                    {
                        Title = "xxx飯店",
                        Text = "5星級高級大飯店",
                        Images = new List<CardImage>()
                        {
                               new CardImage("https://cdn.pixabay.com/photo/2016/02/10/13/32/hotel-1191709_1280.jpg")
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction("openUrl", "官網", value: "http://www.google.com")
                        }
                    };

                    returnMessage.Attachments =new List<Attachment>() { herocard.ToAttachment() };

                    // 送出
                    await context.PostAsync(returnMessage);
                }
                else
                {
                    // 已經有姓名直接輸出 姓名 + 輸入内容
                    await context.PostAsync($"{name}: {activity.Text}");
                }
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}