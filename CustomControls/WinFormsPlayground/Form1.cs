using WinFormsPlayground.Controls;

namespace WinFormsPlayground
{
    public partial class Form1 : Form
    {
        private MyCalendar calendar;
        private readonly string dayClickedTitle = "Selected Date";
        private readonly string dayClickedDateFormat = "yyyy-MM-dd";

        public Form1()
        {
            InitializeComponent();
            InitializeCalendar();

            this.MinimumSize = new Size(calendar.Width + 20, calendar.Height + 40);
            CenterCalendar();

            this.Resize += (s, e) => CenterCalendar();
        }

        /// <summary>
        /// Initializes the calendar control, adds it to the form, 
        /// subscribes to the DayClicked event.
        /// </summary>
        private void InitializeCalendar()
        {
            calendar = new MyCalendar();
            this.Controls.Add(calendar);
            calendar.DayClicked += Calendar_DayClicked;
        }

        /// <summary>
        /// Handles the DayClicked event from the calendar control.
        /// Displays a message box showing the selected date using the predefined format and title.
        /// </summary>
        /// <param name="sender">The calendar control that raised the event.</param>
        /// <param name="date">The selected date from the calendar.</param>
        private void Calendar_DayClicked(object? sender, DateTime date)
        {
            string message = $"Date: {date.ToString(dayClickedDateFormat)}";
            MessageBox.Show(message, dayClickedTitle);
        }

        /// <summary>
        /// Centers the calendar control within the form based on the current client size.
        /// </summary>
        private void CenterCalendar()
        {
            calendar.Location = new Point(
                (ClientSize.Width - calendar.Width) / 2,
                (ClientSize.Height - calendar.Height) / 2);
        }
    }
}
