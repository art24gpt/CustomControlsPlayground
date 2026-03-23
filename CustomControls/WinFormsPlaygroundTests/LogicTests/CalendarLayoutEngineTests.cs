using System.Drawing;
using WinFormsPlayground.Logic;

namespace WinFormsPlaygroundTests.LogicTests
    {
    public class CalendarLayoutEngineTests
    {
        private const int CellSize = 30;
        private const int CellPadding = 2;
        private const int HeaderHeight = 20;
        private const int TotalCells = 42;

        /// <summary>
        /// Test that CalculateDayRects generates the correct number of rectangles.
        /// </summary>
        [Test]
        public void CalculateDayRects_GeneratesCorrectNumberOfRects()
        {
            var engine = new CalendarLayoutEngine();
            int startOffset = 0;

            var rects = engine.CalculateDayRects(startOffset, CellSize, CellPadding, HeaderHeight);

            Assert.That(rects.Length, Is.EqualTo(TotalCells));
        }

        /// <summary>
        /// Test that CalculateDayRects calculates the correct X and Y positions for each cell.
        /// </summary>
        [Test]
        public void CalculateDayRects_CalculatesCorrectPositions()
        {
            var engine = new CalendarLayoutEngine();
            int startOffset = 0;

            var rects = engine.CalculateDayRects(startOffset, CellSize, CellPadding, HeaderHeight);
            int yOffset = HeaderHeight + CellSize; // uwzględnia wiersz z nazwami dni tygodnia

            for (int i = 0; i < TotalCells; i++)
            {
                int row = i / 7;
                int col = i % 7;
                int expectedX = col * CellSize + CellPadding;
                int expectedY = row * CellSize + yOffset + CellPadding;

                Assert.That(rects[i].X, Is.EqualTo(expectedX));
                Assert.That(rects[i].Y, Is.EqualTo(expectedY));
                Assert.That(rects[i].Width, Is.EqualTo(CellSize - 2 * CellPadding));
                Assert.That(rects[i].Height, Is.EqualTo(CellSize - 2 * CellPadding));
            }
        }

        /// <summary>
        /// Test that CalculateMonthArrowRects returns rectangles with correct positions and sizes.
        /// </summary>
        [Test]
        public void CalculateMonthArrowRects_ReturnsCorrectPositions()
        {
            var engine = new CalendarLayoutEngine();
            int controlWidth = 220;
            int arrowSize = 12;

            var (prev, next) = engine.CalculateMonthArrowRects(controlWidth, HeaderHeight, arrowSize, CellSize);

            Assert.That(prev.X, Is.EqualTo(controlWidth - CellSize - CellSize));
            Assert.That(prev.Y, Is.EqualTo((HeaderHeight - arrowSize) / 2));
            Assert.That(prev.Width, Is.EqualTo(arrowSize));
            Assert.That(prev.Height, Is.EqualTo(arrowSize));

            Assert.That(next.X, Is.EqualTo(controlWidth - CellSize));
            Assert.That(next.Y, Is.EqualTo((HeaderHeight - arrowSize) / 2));
            Assert.That(next.Width, Is.EqualTo(arrowSize));
            Assert.That(next.Height, Is.EqualTo(arrowSize));
        }

        /// <summary>
        /// Test that GetDayFromLocation returns the correct day number for points inside the calendar grid.
        /// </summary>
        [Test]
        public void GetDayFromLocation_ReturnsCorrectDay()
        {
            var engine = new CalendarLayoutEngine();
            int startOffset = 0;
            int daysInMonth = 31;
            int yOffset = HeaderHeight + CellSize; // uwzględnia wiersz dni tygodnia

            var day1 = engine.GetDayFromLocation(new Point(5, yOffset + 5), CellSize, HeaderHeight, startOffset, daysInMonth);
            Assert.That(day1, Is.EqualTo(1));

            var day7 = engine.GetDayFromLocation(new Point(6 * CellSize + 1, yOffset + 1), CellSize, HeaderHeight, startOffset, daysInMonth);
            Assert.That(day7, Is.EqualTo(7));

            var day8 = engine.GetDayFromLocation(new Point(1, yOffset + CellSize + 1), CellSize, HeaderHeight, startOffset, daysInMonth);
            Assert.That(day8, Is.EqualTo(8));
        }

        /// <summary>
        /// Test that GetDayFromLocation returns null when clicking outside the calendar grid.
        /// </summary>
        [Test]
        public void GetDayFromLocation_ReturnsNullWhenOutside()
        {
            var engine = new CalendarLayoutEngine();
            int startOffset = 0;
            int daysInMonth = 31;
            int yOffset = HeaderHeight + CellSize;

            var day = engine.GetDayFromLocation(new Point(-5, yOffset + 5), CellSize, HeaderHeight, startOffset, daysInMonth);
            Assert.That(day, Is.Null);

            day = engine.GetDayFromLocation(new Point(10, yOffset + 6 * CellSize + 1), CellSize, HeaderHeight, startOffset, daysInMonth);
            Assert.That(day, Is.Null);

            day = engine.GetDayFromLocation(new Point(7 * CellSize + 1, yOffset + 1), CellSize, HeaderHeight, startOffset, daysInMonth);
            Assert.That(day, Is.Null);
        }
    }
}