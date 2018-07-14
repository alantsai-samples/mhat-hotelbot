using MHAT.HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
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
            throw new NotImplementedException();
        }
    }
}