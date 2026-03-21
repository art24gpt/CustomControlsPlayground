using System.Globalization;
using WinFormsPlayground.Models;

namespace WinFormsPlaygroundTests.ModelTests
{
    public class Tests
    {
        public class CalendarModelTests
        {
            /// <summary>
            /// Test that DaysInMonth returns the correct number of days for the current month.
            /// </summary>
            [Test]
            public void DaysInMonth_ForCurrentMonth_IsCorrect()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                int expected = DateTime.DaysInMonth(today.Year, today.Month);

                Assert.That(model.DaysInMonth, Is.EqualTo(expected));
            }

            /// <summary>
            /// Test that StartOffset returns correct weekday index for the first day of the month.
            /// </summary>
            [Test]
            public void StartOffset_ForCurrentMonth_IsCorrect()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;
                var firstDay = new DateTime(today.Year, today.Month, 1);
                int expected = ((int)firstDay.DayOfWeek + 6) % 7;

                model.CurrentMonth = firstDay;

                Assert.That(model.StartOffset, Is.EqualTo(expected));
            }

            /// <summary>
            /// Test that MonthName returns the correct string for the current month.
            /// </summary>
            [Test]
            public void MonthName_ForCurrentMonth_IsCorrect()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                string expected = model.CurrentMonth.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

                Assert.That(model.MonthName, Is.EqualTo(expected));
            }

            /// <summary>
            /// Test that IsToday returns true for today’s date.
            /// </summary>
            [Test]
            public void IsToday_WhenToday_ReturnsTrue()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                model.CurrentMonth = today;

                Assert.That(model.IsToday(today.Day), Is.True);
            }

            /// <summary>
            /// Test that IsToday returns false for a day that is not today.
            /// </summary>
            [Test]
            public void IsToday_WhenNotToday_ReturnsFalse()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                // Choose a day in the current month that is not today
                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                int nonTodayDay = today.Day == 1 ? 2 : 1;

                Assert.That(model.IsToday(nonTodayDay), Is.False);
            }

            /// <summary>
            /// Test that SelectDay sets SelectedDate correctly.
            /// </summary>
            [Test]
            public void SelectDay_SetsSelectedDateCorrectly()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                int dayToSelect = 15;

                model.SelectDay(dayToSelect);

                Assert.That(model.SelectedDate.HasValue, Is.True);
                Assert.That(model.SelectedDate.Value.Day, Is.EqualTo(dayToSelect));
                Assert.That(model.SelectedDate.Value.Month, Is.EqualTo(model.CurrentMonth.Month));
                Assert.That(model.SelectedDate.Value.Year, Is.EqualTo(model.CurrentMonth.Year));
            }

            /// <summary>
            /// Test that IsSelected returns true when the day matches the selected date.
            /// </summary>
            [Test]
            public void IsSelected_WhenSelectedDateMatches_ReturnsTrue()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                model.SelectDay(today.Day);

                Assert.That(model.IsSelected(today.Day), Is.True);
            }

            /// <summary>
            /// Test that IsSelected returns false when the day does not match the selected date.
            /// </summary>
            [Test]
            public void IsSelected_WhenSelectedDateDoesNotMatch_ReturnsFalse()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);

                int dayToSelect = today.Day + 1;
                if (dayToSelect > model.DaysInMonth)
                    dayToSelect = today.Day - 1;

                model.SelectDay(dayToSelect);

                Assert.That(model.IsSelected(today.Day), Is.False);
            }

            /// <summary>
            /// Test that NextMonth increments the month correctly.
            /// </summary>
            [Test]
            public void NextMonth_IncrementsMonth()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                var expected = model.CurrentMonth.AddMonths(1);

                model.NextMonth();

                Assert.That(model.CurrentMonth.Month, Is.EqualTo(expected.Month));
                Assert.That(model.CurrentMonth.Year, Is.EqualTo(expected.Year));
            }

            /// <summary>
            /// Test that PreviousMonth decrements the month correctly.
            /// </summary>
            [Test]
            public void PreviousMonth_DecrementsMonth()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;

                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                var expected = model.CurrentMonth.AddMonths(-1);

                model.PreviousMonth();

                Assert.That(model.CurrentMonth.Month, Is.EqualTo(expected.Month));
                Assert.That(model.CurrentMonth.Year, Is.EqualTo(expected.Year));
            }
        }
    }
}