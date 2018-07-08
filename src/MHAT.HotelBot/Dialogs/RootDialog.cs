using System;
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
                // 已經有姓名直接輸出 姓名 + 輸入内容
                await context.PostAsync($"{name}: {activity.Text}");
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}