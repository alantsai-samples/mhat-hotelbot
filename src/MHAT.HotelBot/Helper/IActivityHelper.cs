using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHAT.HotelBot.Helper
{
    public static class IActivityHelper
    {
        public static IConnectorClient GetConnector(this IActivity activity)
        {
            return new ConnectorClient(new Uri(activity.ServiceUrl));
        }
    }
}