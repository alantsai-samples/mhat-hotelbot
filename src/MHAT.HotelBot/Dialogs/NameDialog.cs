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
    public class NameDialog : IDialog<string>
    {
        private int Attempt = 3;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("您的名字是？");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync
            (IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if(string.IsNullOrEmpty(message.Text))
            {
                Attempt = Attempt - 1;

                if (Attempt > 0)
                {
                    await context.PostAsync("請您輸入您的名字");

                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("取不到名字"));
                }
            }

            context.Done(message.Text);
        }
    }
}