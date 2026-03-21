using System.Drawing;
using WinFormsPlayground.Logic;

namespace WinFormsPlaygroundTests.LogicTests
{
    public class CalendarLayoutEngineTests
    {
        /// <summary>
        /// Test that CalculateDayRects generates the correct number of rectangles for the month.
        /// </summary>
        [Test]
        public void CalculateDayRects_GeneratesCorrectNumberOfRects()
        {
            var engine = new CalendarLayoutEngine();
            int daysInMonth = 31;
            int startOffset = 2;
            int cellSize = 30;
            int cellPadding = 2;
            int headerHeight = 20;

            var rects = engine.CalculateDayRects(daysInMonth, startOffset, cellSize, cellPadding, headerHeight);

            // Ensure the array length matches the number of days in the month
            Assert.That(rects.Length, Is.EqualTo(daysInMonth));
        }

        /// <summary>
        /// Test that CalculateDayRects computes correct X, Y, width, and height for each day cell.
        /// </summary>
        [Test]
        public void CalculateDayRects_CalculatesCorrectPositions()
        {
            var engine = new CalendarLayoutEngine();
            int daysInMonth = 7;
            int startOffset = 0;
            int cellSize = 30;
            int cellPadding = 2;
            int headerHeight = 20;

            var rects = engine.CalculateDayRects(daysInMonth, startOffset, cellSize, cellPadding, headerHeight);

            for (int i = 0; i < daysInMonth; i++)
            {
                int expectedX = i * cellSize + cellPadding;
                int expectedY = headerHeight + cellSize + cellPadding;

                // Verify rectangle positions and sizes
                Assert.That(rects[i].X, Is.EqualTo(expectedX));
                Assert.That(rects[i].Y, Is.EqualTo(expectedY));
                Assert.That(rects[i].Width, Is.EqualTo(cellSize - 2 * cellPadding));
                Assert.That(rects[i].Height, Is.EqualTo(cellSize - 2 * cellPadding));
            }
        }

        /// <summary>
        /// Test that CalculateMonthArrowRects returns correct positions for previous and next month arrows.
        /// </summary>
        [Test]
        public void CalculateMonthArrowRects_ReturnsCorrectPositions()
        {
            var engine = new CalendarLayoutEngine();
            int controlWidth = 220;
            int headerHeight = 20;
            int arrowSize = 12;

            var (prev, next) = engine.CalculateMonthArrowRects(controlWidth, headerHeight, arrowSize);

            int expectedX = controlWidth - arrowSize - 2;
            int expectedPrevY = (headerHeight - arrowSize) / 2;
            int expectedNextY = expectedPrevY + arrowSize + 2;

            // Check previous arrow rectangle
            Assert.That(prev.X, Is.EqualTo(expectedX));
            Assert.That(prev.Y, Is.EqualTo(expectedPrevY));
            Assert.That(prev.Width, Is.EqualTo(arrowSize));
            Assert.That(prev.Height, Is.EqualTo(arrowSize));

            // Check next arrow rectangle
            Assert.That(next.X, Is.EqualTo(expectedX));
            Assert.That(next.Y, Is.EqualTo(expectedNextY));
            Assert.That(next.Width, Is.EqualTo(arrowSize));
            Assert.That(next.Height, Is.EqualTo(arrowSize));
        }

        /// <summary>
        /// Test that GetDayFromLocation returns the correct day number for valid coordinates.
        /// </summary>
        [Test]
        public void GetDayFromLocation_ReturnsCorrectDay()
        {
            var engine = new CalendarLayoutEngine();
            int cellSize = 30;
            int headerHeight = 20;
            int startOffset = 0;
            int daysInMonth = 31;

            // Test first day in top-left corner
            var day1 = engine.GetDayFromLocation(new Point(5, headerHeight + cellSize + 5), cellSize, headerHeight, startOffset, daysInMonth);
            Assert.That(day1, Is.EqualTo(1));

            // Test seventh day in the same row (column 6)
            var day7 = engine.GetDayFromLocation(new Point(6 * cellSize + 1, headerHeight + cellSize + 1), cellSize, headerHeight, startOffset, daysInMonth);
            Assert.That(day7, Is.EqualTo(7));

            // Test eighth day in the next row
            var day8 = engine.GetDayFromLocation(new Point(0, headerHeight + cellSize + cellSize + 1), cellSize, headerHeight, startOffset, daysInMonth);
            Assert.That(day8, Is.EqualTo(8));
        }

        /// <summary>
        /// Test that GetDayFromLocation returns null when coordinates are outside of the calendar grid.
        /// </summary>
        [Test]
        public void GetDayFromLocation_ReturnsNullWhenOutside()
        {
            var engine = new CalendarLayoutEngine();
            int cellSize = 30;
            int headerHeight = 20;
            int startOffset = 0;
            int daysInMonth = 31;

            // Point left of the calendar
            var day = engine.GetDayFromLocation(new Point(-5, headerHeight + cellSize + 5), cellSize, headerHeight, startOffset, daysInMonth);
            Assert.That(day, Is.Null);

            // Point below the calendar
            day = engine.GetDayFromLocation(new Point(10, headerHeight + cellSize + 6 * cellSize + 1), cellSize, headerHeight, startOffset, daysInMonth);
            Assert.That(day, Is.Null);

            // Point to the right of the calendar
            day = engine.GetDayFromLocation(new Point(7 * cellSize + 1, headerHeight + cellSize + 1), cellSize, headerHeight, startOffset, daysInMonth);
            Assert.That(day, Is.Null);
        }
    }
}
