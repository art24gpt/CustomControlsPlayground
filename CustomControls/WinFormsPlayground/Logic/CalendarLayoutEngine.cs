namespace WinFormsPlayground.Logic
{
    public class CalendarLayoutEngine
    {
        private const int DaysOfWeekCount = 7;

        /// <summary>
        /// Calculates the rectangles representing each day cell in the calendar grid.
        /// The rectangles are positioned based on the cell size, padding, header height, and the start offset of the month.
        /// </summary>
        /// <param name="daysInMonth">Number of days in the current month.</param>
        /// <param name="startOffset">Offset of the first day of the month in the week (0 = Monday, 6 = Sunday).</param>
        /// <param name="cellSize">Size (width and height) of each day cell.</param>
        /// <param name="cellPadding">Padding inside each day cell.</param>
        /// <param name="headerHeight">Height of the calendar header (used for vertical offset).</param>
        /// <returns>An array of rectangles representing the position and size of each day cell.</returns>
        public Rectangle[] CalculateDayRects(
            int daysInMonth,
            int startOffset,
            int cellSize,
            int cellPadding,
            int headerHeight)
        {
            Rectangle[] rects = new Rectangle[daysInMonth];

            for (int i = 0; i < daysInMonth; i++)
            {
                int index = i + startOffset;
                int row = index / DaysOfWeekCount;
                int col = index % DaysOfWeekCount;

                rects[i] = new Rectangle(
                    col * cellSize + cellPadding,
                    row * cellSize + headerHeight + cellSize + cellPadding,
                    cellSize - 2 * cellPadding,
                    cellSize - 2 * cellPadding
                );
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
        public (Rectangle prev, Rectangle next) CalculateMonthArrowRects(int controlWidth, int headerHeight, int arrowSize)
        {
            int arrowX = controlWidth - arrowSize - 2;
            int arrowY = (headerHeight - arrowSize) / 2;
            return (new Rectangle(arrowX, arrowY, arrowSize, arrowSize),
                    new Rectangle(arrowX, arrowY + arrowSize + 2, arrowSize, arrowSize));
        }

        /// <summary>
        /// Determines which day corresponds to a given point within the calendar grid.
        /// Returns null if the point is outside the day cells or outside the valid month range.
        /// </summary>
        /// <param name="location">The point (mouse position) to check.</param>
        /// <param name="cellSize">Size of each day cell.</param>
        /// <param name="headerHeight">Height of the calendar header.</param>
        /// <param name="startOffset">Offset of the first day of the month in the week (0 = Monday).</param>
        /// <param name="daysInMonth">Total number of days in the current month.</param>
        /// <returns>The day number corresponding to the point, or null if outside the grid.</returns>
        public int? GetDayFromLocation(
            Point location,
            int cellSize,
            int headerHeight,
            int startOffset,
            int daysInMonth)
        {
            // offset relative to top-left corner of the first day cell
            int x = location.X;
            int y = location.Y - headerHeight - cellSize;

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
