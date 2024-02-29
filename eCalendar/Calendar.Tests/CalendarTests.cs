using eCalendar;

namespace eCalendar.Tests
{
    public class CalendarTests

    {
        [Theory]
        [InlineData("2009-05-01 14:57", 2, "2009-05-05 14:57")] //start at friday, finish at tuesday
        [InlineData("2009-05-04 8:00", 1, "2009-05-04 16:00")] //starts at monday
        [InlineData("2009-05-04 7:00", 1, "2009-05-04 16:00")]
        [InlineData("2009-05-04 16:00", 1, "2009-05-05 16:00")]
        [InlineData("2009-05-04 17:00", 1, "2009-05-05 16:00")]
        [InlineData("2009-05-04 8:00", 2, "2009-05-05 16:00")]
        [InlineData("2009-05-04 8:00", 2.5, "2009-05-06 12:00")]
        [InlineData("2004-05-24 15:07", 0.25, "2004-05-25 09:07")] //from initial requirements
        [InlineData("2004-05-24 04:00", 0.5, "2004-05-24 12:00")]
        [InlineData("2004-05-24 19:03", 44.723656, "2004-07-26 13:47")]
        [InlineData("2004-05-24 08:03", 12.782709, "2004-06-09 14:18")]
        [InlineData("2004-05-24 07:03", 8.276628, "2004-06-03 10:12")]
        [InlineData("2024-02-28 07:03", 3, "2024-03-01 16:00")]
        public void ShouldCountWorkingDaysCorrectly(string startDateStr, decimal days, string expectedDate)
        {
            //arrange
            var _calendar = new Calendar();
            DateTime.TryParse(startDateStr, out var startDate);

            //act
            var result = _calendar.GetFinishDate(startDate, days);

            //assert
            var expectedString = result.ToString("yyyy-MM-dd HH:mm");
            Assert.Equal(expectedDate, expectedString);
        }

        [Theory]
        [InlineData("2004-05-24 19:03", 44.723656, "2004-07-27 13:47")]
        [InlineData("2004-05-24 08:03", 12.782709, "2004-06-10 14:18")]
        [InlineData("2004-05-24 07:03", 8.276628, "2004-06-04 10:12")]
        public void ShouldCountHolidaysCorrectly(string startDateStr, decimal days, string expectedDate)
        {
            //arrange
            var _calendar = new Calendar();
            _calendar.AddHoliday(new DateOnly(2004, 5, 27));

            DateTime.TryParse(startDateStr, out var startDate);

            //act
            var result = _calendar.GetFinishDate(startDate, days);

            //assert
            var expectedString = result.ToString("yyyy-MM-dd HH:mm");
            Assert.Equal(expectedDate, expectedString);
        }

        [Theory]
        [InlineData("2004-05-16 19:03", 2, "2004-05-19 16:00")]
        [InlineData("2004-05-24 18:05", -5.5, "2004-05-14 12:00")]
        [InlineData("2004-05-24 18:03", -6.7470217, "2004-05-13 10:01")]
        public void ShouldCountRecurrentHolidaysCorrectly(string startDateStr, decimal days, string expectedDate)
        {
            //arrange
            var _calendar = new Calendar();
            _calendar.AddRecurringHoliday(5, 17);

            DateTime.TryParse(startDateStr, out var startDate);

            //act
            var result = _calendar.GetFinishDate(startDate, days);

            //assert
            var expectedString = result.ToString("yyyy-MM-dd HH:mm");
            Assert.Equal(expectedDate, expectedString);
        }


        [Theory]
        [InlineData("2009-05-01 14:57", -1, "2009-04-30 14:57")]
        [InlineData("2009-05-01 16:00", -1, "2009-05-01 08:00")]
        [InlineData("2009-05-01 08:00", -1, "2009-04-30 08:00")]
        [InlineData("2009-05-01 17:00", -1, "2009-05-01 08:00")]
        [InlineData("2009-05-01 07:00", -1, "2009-04-30 08:00")]
        [InlineData("2009-05-01 07:00", -0.25, "2009-04-30 14:00")]
        [InlineData("2024-03-01 07:00", -2, "2024-02-28 08:00")]
        public void ShouldCountWorkingDaysInBackDirectionCorrectly(string startDateStr, decimal days, string expectedDate)
        {
            //arrange
            var _calendar = new Calendar();
            _calendar.AddRecurringHoliday(5, 17);

            DateTime.TryParse(startDateStr, out var startDate);

            //act
            var result = _calendar.GetFinishDate(startDate, days);

            //assert
            var expectedString = result.ToString("yyyy-MM-dd HH:mm");
            Assert.Equal(expectedDate, expectedString);
        }


        [Theory]
        [InlineData("2009-05-01 6:57", 0.5, "2009-05-01 12:00")]
        [InlineData("2009-05-01 7:57", 0.5, "2009-05-01 12:57")]
        [InlineData("2009-05-01 7:00", 1, "2009-05-01 17:00")]
        [InlineData("2009-05-01 7:57", -0.1, "2009-04-30 16:57")]
        [InlineData("2009-05-01 17:01", -1, "2009-05-01 07:00")]
        public void ShouldCountWorkingDaysWithDifferentWorkingHours(string startDateStr, decimal days, string expectedDate)
        {
            //arrange
            var _calendar = new Calendar();
            _calendar.SetWorkingHours(7,0,17,0);

            DateTime.TryParse(startDateStr, out var startDate);

            //act
            var result = _calendar.GetFinishDate(startDate, days);

            //assert
            var expectedString = result.ToString("yyyy-MM-dd HH:mm");
            Assert.Equal(expectedDate, expectedString);
        }
    }
}