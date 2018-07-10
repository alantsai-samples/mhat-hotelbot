using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHAT.HotelBot.Models
{
    public enum BedSizeOptions
    {
        King = 1,
        Queen,
        Single
    }

    [Serializable]
    public class RoomReservation
    {
        public DateTime StartDate { get; set; }
        public int NumberOfNightToStay { get; set; }
        public int NumberOfOccupants { get; set; }
        public BedSizeOptions BedSize { get; set; }
    }
}