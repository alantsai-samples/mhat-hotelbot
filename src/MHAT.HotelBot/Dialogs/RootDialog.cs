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
                // 還沒有姓名
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