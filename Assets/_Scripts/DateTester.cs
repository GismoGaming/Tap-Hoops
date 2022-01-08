using System;
using UnityEngine;

namespace Gismo.Date
{
    public class DateTester : MonoBehaviour
    {
        public DateRange test;

        [ContextMenu("Test")]
        public void TEst()
        {
            Debug.Log(test.CurrentDayWithinRange());
        }
    }

    public enum Month { Jan = 1, Feb = 2, Mar = 3, Apr = 4, May = 5, Jun = 6, Jul = 7, Aug = 8, Sep = 9, Oct = 10, Nov = 11, Dec = 12 };
    [Serializable]
    public class DateRange
    {
        public DateInformation startDate;
        public DateInformation endDate;

        public bool CurrentDayWithinRange()
        {
            DateInformation currentDate = new DateInformation()
            {
                day = DateTime.Now.Day,
                month = (Month)DateTime.Now.Month,
            };

            if (startDate.TranslateToDateTime().CompareTo(endDate.TranslateToDateTime()) <= 0)
            {
                return startDate.TranslateToDateTime().CompareTo(currentDate.TranslateToDateTime()) <= 0 &&
                    endDate.TranslateToDateTime().CompareTo(currentDate.TranslateToDateTime()) >= 0;
            }
            else
            {
                return startDate.TranslateToDateTime(-1).CompareTo(currentDate.TranslateToDateTime()) <= 0 &&
                    endDate.TranslateToDateTime().CompareTo(currentDate.TranslateToDateTime()) >= 0;
            }
        }
    }
    [Serializable]
    public class DateInformation
    {
        public Month month;
        public int day;

        public DateTime TranslateToDateTime(int yearOffset = 0)
        {
            return new DateTime(DateTime.Now.Year + yearOffset, (int)month, day);
        }
    }
}
