using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Calendar = System.Windows.Controls.Calendar;

namespace wpf_mvvm_sample.CustomControls
{
    public partial class Ym : DatePicker
    {
        public Ym()
        {
            InitializeComponent();
        }

        private void DatePicker_Opened(object sender, RoutedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)sender;
            Popup popup = (Popup)datepicker.Template.FindName("PART_Popup", datepicker);
            Calendar cal = (Calendar)popup.Child;
            cal.DisplayModeChanged += Calender_DisplayModeChanged;
            cal.DisplayMode = CalendarMode.Decade;
        }

        private void Calender_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;
            if (calendar.DisplayMode == CalendarMode.Month)
            {
                calendar.SelectedDate = calendar.DisplayDate;
                YearPicker.IsDropDownOpen = false;
            }
        }

        private void YM_OnCalendarClosed(object sender, RoutedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)sender;
            Popup popup = (Popup)datepicker.Template.FindName("PART_Popup", datepicker);
            Calendar cal = (Calendar)popup.Child;
            cal.DisplayModeChanged += Calender_DisplayModeChanged;
            cal.DisplayMode = CalendarMode.Month;
        }
    }
}
