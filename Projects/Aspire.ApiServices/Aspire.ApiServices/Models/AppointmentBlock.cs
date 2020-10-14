using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Aspire.ApiServices.Models
{
    public class WeekSchedule: AppointmentBlock
    {
        public bool IsBooked { get; set; }
    }
    public class AppointmentBlock
    {
        public int WeekOfYear { get; set; }
        public int DayOfWeek { get; set; }
        public int HourBlock { get; set; }
        /// <summary>
        /// 1 on the hour, 2 on the half hour
        /// </summary>
        public int HourSegment { get; set; }

        [JsonIgnore]
        public DateTime get
        {
            get
            {
       
                var startDate = new DateTime(2020, 1, 1);
                var offset = (int)startDate.DayOfWeek;
                var days = (((WeekOfYear - 1) * 7) + DayOfWeek) - offset;
                var min = HourSegment == 1 ? 0 : 30;
                return startDate.AddDays(days).AddHours(HourBlock).AddMinutes(min);
            }
        }
    }

    public class Blocks
    {
        private static List<AppointmentBlock> _block;

        public static List<AppointmentBlock> AllPossibleBlocks
        {
            get
            {
                if (_block == null)
                {
                    _block = new List<AppointmentBlock>();
                    for (var d = 1; d < 6; d++)
                    {
                        for (var h = 8; h < 17; h++)
                        {
                            if (h != 12)
                            {
                                _block.Add(new AppointmentBlock()
                                {
                                    DayOfWeek = d,
                                    HourBlock = h,
                                    HourSegment = 1
                                });
                                if (h != 16)
                                {
                                    _block.Add(new AppointmentBlock()
                                    {
                                        DayOfWeek = d,
                                        HourBlock = h,
                                        HourSegment = 2
                                    });
                                }
                            }
                        }
                    }
                }
                return _block;

            }
        }
    }
}
