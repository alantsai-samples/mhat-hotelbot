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
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("您的名字是？");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync
            (IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            context.Done(message.Text);
        }
    }
}