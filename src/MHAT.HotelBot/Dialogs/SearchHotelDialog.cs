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
    public class SearchHotelDialog : IDialog<HeroCard>
    {
        public Task StartAsync(IDialogContext context)
        {
            // 返回飯店的圖片以及可以打開官網的按鈕

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

            context.Done(herocard);

            return Task.CompletedTask;
        }
    }
}