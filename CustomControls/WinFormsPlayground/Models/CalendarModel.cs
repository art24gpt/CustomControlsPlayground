using System.Globalization;

namespace WinFormsPlayground.Models
{
    public class CalendarModel
    {
        public DateTime CurrentMonth { get; set; } = DateTime.Today;

        public DateTime FirstDay => new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);

        public int StartOffset => ((int)FirstDay.DayOfWeek + 6) % 7;

        public int DaysInMonth => DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);

        public DateTime? SelectedDate { get; set; }

        public Rectangle[] DayRects { get; set; } = Array.Empty<Rectangle>();

        public string MonthName => CurrentMonth.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        public static readonly string[] DayStrings =
        {
            "1","2","3","4","5","6","7","8","9","10",
            "11","12","13","14","15","16","17","18","19","20",
            "21","22","23","24","25","26","27","28","29","30","31"
        };

        public static readonly string[] DayOfWeekStrings =
            { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        /// <summary>
        /// Advances the current month by one.
        /// </summary>
        public void NextMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(1);
        }

        /// <summary>
        /// Moves the current month back by one.
        /// </summary>
        public void PreviousMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(-1);
        }

        /// <summary>
        /// Selects a specific day in the current month.
        /// </summary>
        /// <param name="day">The day number to select.</param>
        public void SelectDay(int day)
        {
            SelectedDate = new DateTime(CurrentMonth.Year, CurrentMonth.Month, day);
        }


        /// <summary>
        /// Checks if the specified day corresponds to today's date in the current month.
        /// </summary>
        /// <param name="day">The day number to check.</param>
        /// <returns>True if the day is today; otherwise, false.</returns>
        public bool IsToday(int day)
        {
            DateTime today = DateTime.Today;
            return today.Day == day &&
                   today.Month == CurrentMonth.Month &&
                   today.Year == CurrentMonth.Year;
        }

        /// <summary>
        /// Checks if the specified day is currently selected.
        /// </summary>
        /// <param name="day">The day number to check).</param>
        /// <returns>True if the day is selected; otherwise, false.</returns>
        public bool IsSelected(int day) => SelectedDate.HasValue &&
                                   SelectedDate.Value.Day == day &&
                                   SelectedDate.Value.Month == CurrentMonth.Month &&
                                   SelectedDate.Value.Year == CurrentMonth.Year;
    }
}