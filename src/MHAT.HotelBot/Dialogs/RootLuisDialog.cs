using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHAT.HotelBot.Dialogs
{
    [LuisModel("<modelId>", "<key>")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
    }
}