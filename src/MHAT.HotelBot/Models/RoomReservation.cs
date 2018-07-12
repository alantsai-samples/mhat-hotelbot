using Microsoft.Bot.Builder.FormFlow;
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
        [Describe("入住日期")]
        [Prompt("請輸入您的 {&}")]
        public DateTime StartDate { get; set; }
        [Numeric(1, 5)]
        public int NumberOfNightToStay { get; set; }
        public int NumberOfOccupants { get; set; }
        public BedSizeOptions BedSize { get; set; }

        public static IForm<RoomReservation> BuildForm()
        {
            return new FormBuilder<RoomReservation>()
                .Message
                ("歡迎使用訂房功能。接下來將會問您一系列問題好讓我們幫您找到最好的房間。" +
                    $"{Environment.NewLine}有任何問題隨時打入Help將有幫助文字出現。")
                .Field(nameof(StartDate),
                        validate: async (state, value) =>
                        {
                            var result = new ValidateResult
                                { IsValid = true, Value = value };

                            var datetime = (DateTime)value;

                            result.Value = datetime.AddDays(1);

                            return result;
                        })
                  .Field(nameof(NumberOfNightToStay))
                  .Field(nameof(NumberOfOccupants))
                  .Field(nameof(BedSize))
                .Build();
        }
    }
}