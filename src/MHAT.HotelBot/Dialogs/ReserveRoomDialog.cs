using MHAT.HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MHAT.HotelBot.Dialogs
{
    [Serializable]
    public class ReserveRoomDialog : IDialog<RoomReservation>
    {
        public Task StartAsync(IDialogContext context)
        {
            var reserveRoomForm =
                        FormDialog.FromForm(RoomReservation.BuildForm,
                            FormOptions.PromptInStart);

            context.Call(reserveRoomForm, AfterReserveRoomAsync);

            return Task.CompletedTask;
        }

        private async Task AfterReserveRoomAsync(IDialogContext context
           , IAwaitable<RoomReservation> result)
        {
            RoomReservation reservation = null;

            try
            {
                reservation = await result;

                //await context.PostAsync($"得到的結果：{Environment.NewLine} {JsonConvert.SerializeObject(reservation)}");
            }
            catch (FormCanceledException<RoomReservation> ex)
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
                context.Done(reservation);
            }
        }
    }
}