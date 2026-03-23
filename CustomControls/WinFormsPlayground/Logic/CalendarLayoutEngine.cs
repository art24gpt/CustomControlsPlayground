namespace WinFormsPlayground.Logic
{
    public class CalendarLayoutEngine
    {
        private const int DaysOfWeekCount = 7;
        private const int TotalCells = 42;

        /// <summary>
        /// Calculates the rectangles representing each cell in the calendar grid.
        /// This includes all visible cells, not only the days of the current month, 
        /// so that cells can also represent trailing days from the previous month
        /// or leading days from the next month.
        /// The rectangles are positioned based on the cell size, padding, header height, 
        /// and the start offset of the current month.
        /// </summary>
        /// <param name="startOffset">Offset of the first day of the month in the week (0 = Monday, 6 = Sunday).</param>
        /// <param name="cellSize">Size (width and height) of each day cell.</param>
        /// <param name="cellPadding">Padding inside each day cell.</param>
        /// <param name="headerHeight">Height of the calendar header (used for vertical offset).</param>
        /// <returns>An array of rectangles representing the position and size of each cell in the calendar grid.</returns>
        public Rectangle[] CalculateDayRects(
            int startOffset,
            int cellSize,
            int cellPadding,
            int headerHeight)
        {
            Rectangle[] rects = new Rectangle[TotalCells];
            int yOffset = headerHeight + cellSize;

            for (int i = 0; i < TotalCells; i++)
            {
                int row = i / DaysOfWeekCount;
                int col = i % DaysOfWeekCount;

                rects[i] = new Rectangle(
                    col * cellSize + cellPadding,
                    row * cellSize + yOffset + cellPadding,
                    cellSize - 2 * cellPadding,
                    cellSize - 2 * cellPadding);
            }

            return rects;
        }

        /// <summary>
        /// Calculates the rectangles for the previous and next month arrow buttons in the calendar header.
        /// </summary>
        /// <param name="controlWidth">Width of the calendar control.</param>
        /// <param name="headerHeight">Height of the header section.</param>
        /// <param name="arrowSize">Size of the arrow buttons.</param>
        /// <returns>A tuple containing rectangles for the previous and next month arrows.</returns>
        public (Rectangle prev, Rectangle next) CalculateMonthArrowRects(int controlWidth, int headerHeight, int arrowSize, int cellSize)
        {
            int arrowX = controlWidth - cellSize;
            int arrowY = (headerHeight - arrowSize) / 2;
            return (new Rectangle(arrowX - cellSize, arrowY, arrowSize, arrowSize),
                    new Rectangle(arrowX, arrowY, arrowSize, arrowSize));
        }

        /// <summary>
        /// Determines which day of the current month corresponds to a given point within the calendar grid.
        /// Returns null if the point is outside the grid or if it corresponds to a day not in the current month.
        /// </summary>
        /// <param name="location">The point (mouse position) to check.</param>
        /// <param name="cellSize">Size of each day cell.</param>
        /// <param name="headerHeight">Height of the calendar header.</param>
        /// <param name="startOffset">Offset of the first day of the month in the week (0 = Monday).</param>
        /// <param name="daysInMonth">Total number of days in the current month.</param>
        /// <returns>The day number in the current month corresponding to the point, or null if outside the grid or not in the current month.</returns>
        public int? GetDayFromLocation(
            Point location,
            int cellSize,
            int headerHeight,
            int startOffset,
            int daysInMonth)
        {
            // offset relative to top-left corner of the first day cell
            int yOffset = headerHeight + cellSize;
            int x = location.X;
            int y = location.Y - yOffset;

            // reject points left of grid or above first row
            if (x < 0 || y < 0)
                return null;

            int col = x / cellSize;
            int row = y / cellSize;

            // reject points beyond 7 columns
            if (col >= DaysOfWeekCount)
                return null;

            int index = row * DaysOfWeekCount + col - startOffset;

            // reject points beyond number of days in month
            if (index < 0 || index >= daysInMonth)
                return null;

            return index + 1;
        }
    }
}
