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
    public class GetReceiptDialog : IDialog<Attachment>
    {
        public Task StartAsync(IDialogContext context)
        {
            var receiptCard = new ReceiptCard()
                    {
                        Title = "訂房費用",
                        Total = "NT$ 120",
                        Tax = "NT$ 20",
                        Items = new List<ReceiptItem>()
                           {
                               new ReceiptItem()
                               {
                                    Title = "1大房",
                                    Price = "90",
                                    Quantity = "1",
                                    Image = new CardImage("https://cdn.pixabay.com/photo/2014/08/11/21/40/wall-416062__180.jpg")
                               },
                               new ReceiptItem()
                               {
                                   Title = "飲料",
                                   Price = "10",
                                   Quantity = "1",
                                   Image = new CardImage("https://cdn.pixabay.com/photo/2014/09/26/19/51/coca-cola-462776_1280.jpg")
                               }
                           }
                    };

            context.Done(receiptCard.ToAttachment());

            return Task.CompletedTask;
        }
    }
}