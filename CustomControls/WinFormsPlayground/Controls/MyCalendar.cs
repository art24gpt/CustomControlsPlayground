using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsPlayground.Logic;
using WinFormsPlayground.Models;
using static WinFormsPlayground.Models.CalendarModel;

namespace WinFormsPlayground.Controls
{
    public class MyCalendar : Control
    {
        #region Calendar Fields and Resources
        private const int CalendarWidth = 220;
        private const int CalendarHeight = HeaderHeight + CellSize + 6 * CellSize;
        private const int CellSize = 30;
        private const int CellPadding = 2;
        private const int HeaderHeight = 20;
        private const int ArrowSize = 12;
        private const int DaysOfWeekCount = 7;
        private readonly PointF monthNamePosition = new(0, 0);

        private readonly string ArrowUpSymbol = "▲";
        private readonly string ArrowDownSymbol = "▼";

        private readonly CalendarModel model = new();
        private readonly CalendarLayoutEngine layout = new();

        private Rectangle prevMonthArrow;
        private Rectangle nextMonthArrow;

        private static readonly Brush TextBrush = Brushes.Black;
        private Brush? backgroundBrush;
        private readonly SolidBrush todayBackgroundBrush = new SolidBrush(Color.LightBlue);
        private readonly SolidBrush dayBackgroundBrush = new SolidBrush(Color.LightGray);
        private readonly SolidBrush anotherMonthDayBackgroundBrush = new SolidBrush(Color.LightSlateGray);
        private readonly Pen selectedPen = new Pen(Color.LightBlue, 2);
        private readonly Pen borderPen = new Pen(Color.Gray, 1);
        private readonly Font boldFont;
        private static readonly Color TextColorBlack = Color.Black;

        private static readonly TextFormatFlags TextFormatFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
        private static readonly ControlStyles CalendarControlStyles = ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer;

        /// <summary>
        /// Event raised when a day is clicked. Subscribers receive the clicked date.
        /// </summary>
        public event EventHandler<DateTime>? DayClicked;
        #endregion

        public MyCalendar()
        {
            SetStyle(CalendarControlStyles, true);

            this.Size = new Size(CalendarWidth, CalendarHeight);
            boldFont = new Font(this.Font, FontStyle.Bold);

            CalculateLayout();
        }

        #region Drawing
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            DrawHeader(g);
            DrawDayOfWeek(g);
            DrawDays(g);
        }

        /// <summary>
        /// Draws the calendar header, including the month name and navigation arrows.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawHeader(Graphics g)
        {
            g.DrawString(model.MonthName, boldFont, TextBrush, monthNamePosition);

            (prevMonthArrow, nextMonthArrow) = layout.CalculateMonthArrowRects(this.Width, HeaderHeight, ArrowSize, CellSize);

            DrawArrow(g, prevMonthArrow, ArrowUpSymbol);
            DrawArrow(g, nextMonthArrow, ArrowDownSymbol);
        }

        /// <summary>
        /// Draws a single arrow (previous or next month) at the specified rectangle.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        /// <param name="rect">The rectangle where the arrow should be drawn.</param>
        /// <param name="symbol">The arrow symbol to display.</param>
        private void DrawArrow(Graphics g, Rectangle rect, string symbol)
        {
            TextRenderer.DrawText(g, symbol, this.Font, rect, TextColorBlack, TextFormatFlags);
        }

        /// <summary>
        /// Draws the labels for the days of the week (Mon, Tue, ... Sun).
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawDayOfWeek(Graphics g)
        {
            for (int i = 0; i < DaysOfWeekCount; i++)
            {
                Rectangle rect = new Rectangle(i * CellSize, HeaderHeight, CellSize, CellSize);
                TextRenderer.DrawText(g, CalendarModel.DayOfWeekStrings[i], this.Font, rect, TextColorBlack, TextFormatFlags);
            }
        }

        /// <summary>
        /// Draws all the day cells in the calendar grid, including:
        /// - days from the previous month filling the first week,
        /// - days of the current month,
        /// - days from the next month filling the last week.
        /// Uses the isCurrentMonth flag to determine the correct background color and selection logic.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawDays(Graphics g)
        {
            int totalCells = model.DayRects.Length;
            int prevMonthFill = model.StartOffset;
            int daysInMonth = model.DaysInMonth;
            int daysInPrevMonth = model.DaysInPreviousMonth;

            for (int i = 0; i < totalCells; i++)
            {
                int day;
                if (i < prevMonthFill)
                {
                    day = daysInPrevMonth - prevMonthFill + i + 1;
                }
                else if (i < prevMonthFill + daysInMonth)
                {
                    day = i - prevMonthFill + 1;
                }
                else
                {
                    day = i - (prevMonthFill + daysInMonth) + 1;
                }

                DrawDay(g, day, i, i >= prevMonthFill && i < prevMonthFill + daysInMonth);
            }
        }

        /// <summary>
        /// Draws a single day cell in the calendar grid, including its background, border, and day number.
        /// - Uses a different background for days outside the current month.
        /// - Highlights today if the day corresponds to the current date.
        /// - Highlights the selected day if it belongs to the current month.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        /// <param name="day">The day number to draw (1–31).</param>
        /// <param name="index">The zero-based index of the cell in the calendar grid.</param>
        /// <param name="isCurrentMonth">True if the day belongs to the current month, false if it is from the previous or next month.</param>
        private void DrawDay(Graphics g, int day, int index, bool isCurrentMonth)
        {
            Rectangle rect = model.DayRects[index];

            // Choose background
            if (!isCurrentMonth)
                backgroundBrush = anotherMonthDayBackgroundBrush;
            else if (model.IsToday(day))
                backgroundBrush = todayBackgroundBrush;
            else
                backgroundBrush = dayBackgroundBrush;

            g.FillRectangle(backgroundBrush, rect);

            // Fill colour
            Pen pen = (isCurrentMonth && model.IsSelected(day)) ? selectedPen : borderPen;
            g.DrawRectangle(pen, rect);

            // Draw day number
            TextRenderer.DrawText(
                g,
                CalendarModel.DayStrings[day - 1],
                this.Font,
                rect,
                TextColorBlack,
                TextFormatFlags
            );
        }
        #endregion

        /// <summary>
        /// Handles mouse click events for selecting a day or changing the month.
        /// </summary>
        /// <param name="e">MouseEventArgs containing mouse click data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (HandleMonthChange(e.Location))
                return;

            HandleDaySelection(e.Location);
        }

        /// <summary>
        /// Checks if the click was on a month navigation arrow and changes the month if so.
        /// </summary>
        /// <param name="location">The point where the mouse was clicked.</param>
        /// <returns>True if the month was changed; otherwise, false.</returns>
        private bool HandleMonthChange(Point location)
        {
            if (prevMonthArrow.Contains(location))
            {
                model.SetPreviousMonth();
                CalculateLayout();
                Invalidate();
                return true;
            }

            if (nextMonthArrow.Contains(location))
            {
                model.SetNextMonth();
                CalculateLayout();
                Invalidate();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a day cell was clicked and updates the selected day.
        /// If day has a value, it raises DayClicked event
        /// </summary>
        /// <param name="location">The point where the mouse was clicked.</param>
        private void HandleDaySelection(Point location)
        {
            int? day = layout.GetDayFromLocation(
                location,
                CellSize,
                HeaderHeight,
                model.StartOffset,
                model.DaysInMonth);

            if (day.HasValue)
            {
                model.SelectDay(day.Value);
                Invalidate();
                DayClicked?.Invoke(this, model.SelectedDate.Value);
            }
        }

        /// <summary>
        /// Calculates the rectangles for all day cells in the calendar grid.
        /// </summary>
        private void CalculateLayout()
        {
            model.DayRects = layout.CalculateDayRects(
                model.StartOffset,
                CellSize,
                CellPadding,
                HeaderHeight);
        }

        /// <summary>
        /// Recalculates the layout and invalidates the control when the calendar is resized.
        /// </summary>
        /// <param name="e">EventArgs associated with the resize event.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalculateLayout();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dayBackgroundBrush.Dispose();
                todayBackgroundBrush.Dispose();
                selectedPen.Dispose();
                borderPen.Dispose();
                boldFont.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}