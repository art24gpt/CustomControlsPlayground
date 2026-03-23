using System.Globalization;
using WinFormsPlayground.Models;

namespace WinFormsPlaygroundTests.ModelTests
{
    public class CalendarModelTests
        {
            /// <summary>
            /// Tests that DaysInMonth returns the correct number of days for the current month.
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
            /// Tests that DaysInPreviousMonth returns the correct number of days for the previous month.
            /// </summary>
            [Test]
            public void DaysInPreviousMonth_IsCorrect()
            {
                var model = new CalendarModel();
                model.CurrentMonth = new DateTime(2026, 3, 1);
                int expected = DateTime.DaysInMonth(2026, 2);
                Assert.That(model.DaysInPreviousMonth, Is.EqualTo(expected));
            }

            /// <summary>
            /// Tests that StartOffset returns the correct weekday index for the first day of the month.
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
            /// Tests that MonthName returns the correct string for the current month.
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
            /// Tests that IsToday returns true when the day is today.
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
            /// Tests that IsToday returns false when the day is not today.
            /// </summary>
            [Test]
            public void IsToday_WhenNotToday_ReturnsFalse()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;
                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                int nonTodayDay = today.Day == 1 ? 2 : 1;
                Assert.That(model.IsToday(nonTodayDay), Is.False);
            }

            /// <summary>
            /// Tests that IsToday returns false for days from a previous month.
            /// </summary>
            [Test]
            public void IsToday_ForPreviousMonthDay_ReturnsFalse()
            {
                var model = new CalendarModel();
                model.CurrentMonth = DateTime.Today.AddMonths(-1);
                int day = DateTime.Today.Day;
                Assert.That(model.IsToday(day), Is.False);
            }

            /// <summary>
            /// Tests that SelectDay sets SelectedDate correctly.
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
            /// Tests that SelectDay throws an exception if the day is out of range.
            /// </summary>
            [Test]
            public void SelectDay_OutOfRange_ThrowsException()
            {
                var model = new CalendarModel();
                model.CurrentMonth = new DateTime(2026, 3, 1);
                Assert.Throws<ArgumentOutOfRangeException>(() => model.SelectDay(32));
            }

            /// <summary>
            /// Tests that IsSelected returns true when the day matches the selected date.
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
            /// Tests that IsSelected returns false when the day does not match the selected date.
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
            /// Tests that SetNextMonth increments the current month correctly.
            /// </summary>
            [Test]
            public void NextMonth_IncrementsMonth()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;
                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                var expected = model.CurrentMonth.AddMonths(1);
                model.SetNextMonth();
                Assert.That(model.CurrentMonth.Month, Is.EqualTo(expected.Month));
                Assert.That(model.CurrentMonth.Year, Is.EqualTo(expected.Year));
            }

            /// <summary>
            /// Tests that SetPreviousMonth decrements the month correctly across a year boundary.
            /// </summary>
            [Test]
            public void SetPreviousMonth_HandlesYearBoundary()
            {
                var model = new CalendarModel();
                model.CurrentMonth = new DateTime(2026, 1, 1);
                model.SetPreviousMonth();
                Assert.That(model.CurrentMonth.Month, Is.EqualTo(12));
                Assert.That(model.CurrentMonth.Year, Is.EqualTo(2025));
            }

            /// <summary>
            /// Tests that SetPreviousMonth decrements the current month correctly.
            /// </summary>
            [Test]
            public void PreviousMonth_DecrementsMonth()
            {
                var model = new CalendarModel();
                var today = DateTime.Today;
                model.CurrentMonth = new DateTime(today.Year, today.Month, 1);
                var expected = model.CurrentMonth.AddMonths(-1);
                model.SetPreviousMonth();
                Assert.That(model.CurrentMonth.Month, Is.EqualTo(expected.Month));
                Assert.That(model.CurrentMonth.Year, Is.EqualTo(expected.Year));
            }
        }
    }