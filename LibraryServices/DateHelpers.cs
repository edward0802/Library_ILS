using LibraryDara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServices
{
    public static class DateHelpers
    {
        public static IEnumerable<string> NormalizeBranchHours(IEnumerable<BranchHours> lbHours)
        {
            var hours = new List<string>();

            foreach(var time in lbHours)
            {
                var day = NormalizeDay(time.DayOfWeek);
                var openTime = NormalizeTime(time.OpenTime);
                var closeTime = NormalizeTime(time.CloseTime);

                var timePattern = $"{day}: {openTime} - {closeTime}";
                hours.Add(timePattern);
            }
            return hours;
        }

        public static string NormalizeDay(int day)
        {
            // 1 -> Sunday, so we subtract 1
            return Enum.GetName(typeof(DayOfWeek), (day - 1));
        }
        public static string NormalizeTime(int time)
        {
            return TimeSpan.FromHours(time).ToString("hh':'mm");
        }

    }
}
