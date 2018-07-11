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
                context.PrivateConversationData.
                    TryGetValue<bool>("IsAskName", out bool isAskName);

                if(isAskName)
                {
                    context.UserData.SetValue<string>("Name", activity.Text);

                    await context.PostAsync($"{activity.Text} 您好，能夠幫助您什麽");
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    context.PrivateConversationData.SetValue<bool>("IsAskName", true);

                    await context.PostAsync("您的名字是？");
                    context.Wait(MessageReceivedAsync);
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
                    context.Wait(MessageReceivedAsync);
                }
                else if(activity.Text == "訂房")
                {
                    var returnMessage = activity.CreateReply();

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

                    returnMessage.Attachments = new List<Attachment>() { receiptCard.ToAttachment() };

                    await context.PostAsync(returnMessage);
                    context.Wait(MessageReceivedAsync);
                }
                else if(activity.Text == "查飯店v2")
                {
                    var returnMessage = activity.CreateReply();

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