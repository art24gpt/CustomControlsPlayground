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
        /// Draws all the day cells for the current month.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        private void DrawDays(Graphics g)
        {
            for (int i = 0; i < model.DaysInMonth; i++)
            {
                DrawDay(g, i);
            }
        }

        /// <summary>
        /// Draws a single day cell, including its background, border, and day number.
        /// Highlights today and selected days.
        /// </summary>
        /// <param name="g">Graphics object used for drawing.</param>
        /// <param name="index">The zero-based index of the day in the current month.</param>
        private void DrawDay(Graphics g, int index)
        {
            int day = index + 1;
            Rectangle rect = model.DayRects[index];

            bool isToday = model.IsToday(day);
            bool isSelected = model.IsSelected(day);

            backgroundBrush = isToday ? todayBackgroundBrush : dayBackgroundBrush;
            g.FillRectangle(backgroundBrush, rect);

            Pen pen = isSelected ? selectedPen : borderPen;
            g.DrawRectangle(pen, rect);

            TextRenderer.DrawText(g, CalendarModel.DayStrings[day - 1], this.Font, rect, TextColorBlack, TextFormatFlags);
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
                model.PreviousMonth();
                CalculateLayout();
                Invalidate();
                return true;
            }

            if (nextMonthArrow.Contains(location))
            {
                model.NextMonth();
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
                model.DaysInMonth,
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