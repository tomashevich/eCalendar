using eCalendar;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CalendarCalculatorUI
{
    public sealed partial class MainWindow : Window
    {
        DateTime startDateTime;
        DateTime recurringHoliday;
        DateTime holiday;

        double DaysNumberBoxValue = 1;
        Calendar _calendar;

        public MainWindow()
        {
            this.InitializeComponent();

            _calendar = new Calendar();
            _calendar.AddHoliday(new DateOnly(2024, 1, 1));
            _calendar.AddRecurringHoliday(5, 17);

            startDateTime = DateTime.Now;

            startTimePicker.SelectedTime = new TimeSpan(startDateTime.Hour, startDateTime.Minute, 0);
            StartDatePicker.SelectedDate = startDateTime;
            startDateText.Text = startDateTime.ToString("yyyy-MMM-dd HH:mm");
            holidaysText.Text = string.Join("  ", _calendar.GetHolidays().Select(d => d.ToString("yyyy-MMM-dd")).ToArray());
            recurrentHolidayslText.Text = string.Join("  ", _calendar.GetRecurringHolidays().Select(d => new DateOnly(2000, d.Month, d.Day).ToString("MMM-dd")).ToArray());
        }

        private void StartTimePicker_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            if (startTimePicker.SelectedTime != null)
            {
                startDateTime = new DateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day,
                                               args.NewTime.Value.Hours, args.NewTime.Value.Minutes, args.NewTime.Value.Seconds);
                startDateText.Text = startDateTime.ToString("yyyy-MMM-dd HH:mm");
            }

        }

        private void StartDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            if (StartDatePicker.SelectedDate != null)
            {
                startDateTime = new DateTime(args.NewDate.Value.Year, args.NewDate.Value.Month, args.NewDate.Value.Day,
                                                   startDateTime.Hour, startDateTime.Minute, startDateTime.Second);
                startDateText.Text = startDateTime.ToString("yyyy-MMM-dd HH:mm");
            }
        }

        private void RecurrentHolidayDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            recurringHoliday = new DateTime(args.NewDate.Value.Year, args.NewDate.Value.Month, args.NewDate.Value.Day);
        }

        private void HolidayDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            holiday = new DateTime(args.NewDate.Value.Year, args.NewDate.Value.Month, args.NewDate.Value.Day);
        }

        private void CalculateDateButton_Click(object sender, RoutedEventArgs e)
        {
            var result = _calendar.GetFinishDate(startDateTime, (decimal)DaysNumberBoxValue);
            resultText.Text = "Result: " + result.ToString("yyyy-MMM-dd HH:mm");
        }

        private void AddRecurrentHolidayButton_Click(object sender, RoutedEventArgs e)
        {
            _calendar.AddRecurringHoliday(recurringHoliday.Month, recurringHoliday.Day);

            var holidays = _calendar.GetRecurringHolidays();

            recurrentHolidayslText.Text = string.Join("  ", holidays.Select(d => new DateOnly(2000, d.Month, d.Day).ToString("MMM-dd")).ToArray());
        }

        private void AddHolidayDateButton_Click(object sender, RoutedEventArgs e)
        {
            _calendar.AddHoliday(new DateOnly(holiday.Year, holiday.Month, holiday.Day));

            var holidays = _calendar.GetHolidays();
            holidaysText.Text = string.Join("  ", holidays.Select(d => d.ToString("yyyy-MMM-dd")).ToArray());
        }
    }
}
