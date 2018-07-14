using AdaptiveCards;
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
    public class SearchHotelDialog : IDialog<List<Attachment>>
    {
        public Task StartAsync(IDialogContext context)
        {
            // 返回飯店的圖片以及可以打開官網的按鈕

            // 建立一個HeroCard
            HeroCard herocard = GetHeroCard();

            AdaptiveCard adCard = GetAdaptiveCard();

            context.Done(new List<Attachment>() { herocard.ToAttachment(),
                    new Attachment()
                    {
                        Content = adCard,
                        ContentType = AdaptiveCard.ContentType
                    }
             });

            return Task.CompletedTask;
        }

        private static HeroCard GetHeroCard()
        {
            return new HeroCard()
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
        }

        private AdaptiveCard GetAdaptiveCard()
        {
            var card = new AdaptiveCard();
            var columSet = new ColumnSet();
            columSet.Columns.Add(new Column()
            {
                Size = "1",
                Items = new List<CardElement>()
                         {
                             new TextBlock()
                             {
                                  Text = "豪華大飯店",
                                  Weight = TextWeight.Bolder,
                                  Size = TextSize.ExtraLarge
                             },
                             new TextBlock()
                             {
                                 Text = "4.2 ★★★☆ (120) ",
                                 IsSubtle = true,
                                   Wrap = false
                             }
                         }
            });

            columSet.Columns.Add(new Column()
            {
                Size = "1",
                Items = new List<CardElement>()
                        {
                            new Image()
                            {
                                Url = "https://cdn.pixabay.com/photo/2016/02/10/13/32/hotel-1191709_1280.jpg",
                                Size = ImageSize.Auto
                            }
                        }
            });

            card.Body.Add(columSet);

            card.Actions.Add(new OpenUrlAction()
            {
                Title = "官網",
                Url = "http://wwww.google.com"
            });

            return card;
        }
    }
}