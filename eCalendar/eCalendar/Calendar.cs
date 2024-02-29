namespace eCalendar
{
    public class Calendar
    {
        private List<(int Month, int Day)> _recurringHolidays;
        private List<DateOnly> _holidays;
        private List<DayOfWeek> _workingDays;
        private int _startWorkingHour;
        private int _startWorkingMinute;
        private int _endWorkingHour;
        private int _endWorkingMinute;
        public Calendar()
        {
            SetWorkingDays(new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
            SetWorkingHours(8, 0, 16, 0);
            _recurringHolidays = new List<(int Month, int Day)>();
            _holidays = new List<DateOnly>();
        }

        public void SetWorkingDays(List<DayOfWeek> workingDays)
        {
            _workingDays = workingDays;
        }

        public void SetWorkingHours(int startHours, int startMinutes, int endHour, int endMinutes)
        {
            _startWorkingHour = startHours;
            _startWorkingMinute = startMinutes;
            _endWorkingHour = endHour;
            _endWorkingMinute = endMinutes;
        } 
        
        public void AddRecurringHoliday(int month, int day)
        {
            _recurringHolidays.Add(new(month, day));
        }

        public void AddHoliday(DateOnly holiday)
        {
            _holidays.Add(holiday);
        }

        public List<DateOnly> GetHolidays()
        {
            return _holidays;
        }

        public List<(int Month, int Day)> GetRecurringHolidays()
        {
            return _recurringHolidays;
        }

        public (int StartHour, int StartMinute, int EndHour, int EndMinute) GetWorkingHours()
        {
            return new (_startWorkingHour, _startWorkingMinute, _endWorkingHour, _endWorkingMinute);
        }

        public DateTime GetFinishDate(DateTime start, decimal addWorkingDays)
        {
            var workingDayTicks = GetWorkingDayDurationTicks();
            var totalWorkAmountTicks = (long)(workingDayTicks * addWorkingDays);

            if (totalWorkAmountTicks > 0)
            {
                //go trough timespan unill finish date met
                var amountOfWorkLeftTicks = totalWorkAmountTicks;
                var currentDate = start;
                do
                {
                    if (IsWeekEnd(currentDate) || IsHoliday(currentDate) || IsRecurringHoliday(currentDate))
                    {
                    }
                    else
                    {
                        TimeOnly currentTime, endOfWorkingDayTime, startOfWorkingDayTime;
                        InitWorkingDay(currentDate, out currentTime, out endOfWorkingDayTime, out startOfWorkingDayTime);

                        if (currentTime < endOfWorkingDayTime)
                        {
                            if (currentTime < startOfWorkingDayTime)
                            {
                                var ticksToStartOfWorkingDay = startOfWorkingDayTime.Ticks - currentTime.Ticks;
                                //move to the start of the working day
                                currentDate = currentDate.AddTicks(ticksToStartOfWorkingDay);
                                currentTime = new TimeOnly(currentDate.Hour, currentDate.Minute, 0);
                            }
                            if ((startOfWorkingDayTime <= currentTime)) // current time inside working hours
                            {
                                var currentDayWorkingTicksLeft = endOfWorkingDayTime.Ticks - currentTime.Ticks;

                                if (amountOfWorkLeftTicks > currentDayWorkingTicksLeft)
                                {
                                    //subtract from total
                                    amountOfWorkLeftTicks = amountOfWorkLeftTicks - currentDayWorkingTicksLeft;
                                    currentDate = currentDate.AddTicks(currentDayWorkingTicksLeft);
                                }
                                else
                                {
                                    // find the finish time and set the finish date
                                    currentDate = currentDate.AddTicks(amountOfWorkLeftTicks);
                                    break;
                                }
                            }
                        }
                    }

                    // increase day count on current date
                    currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, _startWorkingHour, _startWorkingMinute, 0);
                    currentDate = currentDate.AddDays(1);

                } while (amountOfWorkLeftTicks > 0);

                return currentDate;
            }
            else //reverse 
            {
                var amountOfWorkLeftTicks = totalWorkAmountTicks;
                var currentDate = start;
                do
                {
                    if (IsWeekEnd(currentDate) || IsHoliday(currentDate) || IsRecurringHoliday(currentDate))
                    {
                    }
                    else
                    {
                        TimeOnly currentTime, endOfWorkingDayTime, startOfWorkingDayTime;
                        InitWorkingDay(currentDate, out currentTime, out endOfWorkingDayTime, out startOfWorkingDayTime);

                        if (currentTime > startOfWorkingDayTime)
                        {
                            if (currentTime > endOfWorkingDayTime)
                            {
                                var ticksToEndOfWorkingDay = currentTime.Ticks - endOfWorkingDayTime.Ticks;
                                //move to the end of the working day
                                currentDate = currentDate.AddTicks(-ticksToEndOfWorkingDay);
                                currentTime = new TimeOnly(currentDate.Hour, currentDate.Minute, 0);
                            }
                            if ((startOfWorkingDayTime <= currentTime)) // current time inside working hours
                            {
                                var currentDayWorkingTicksLeft = currentTime.Ticks - startOfWorkingDayTime.Ticks;

                                if (-amountOfWorkLeftTicks > currentDayWorkingTicksLeft)
                                {
                                    //subtract from total
                                    amountOfWorkLeftTicks = amountOfWorkLeftTicks + currentDayWorkingTicksLeft;
                                    currentDate = currentDate.AddTicks(-currentDayWorkingTicksLeft);
                                }
                                else
                                {
                                    // find the finish time and set the finish date
                                    currentDate = currentDate.AddTicks(amountOfWorkLeftTicks);
                                    break;
                                }
                            }
                        }
                    }

                    // increase day count on current date
                    currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, _endWorkingHour, _endWorkingMinute, 0);
                    currentDate = currentDate.AddDays(-1);

                } while (amountOfWorkLeftTicks < 0);

                return currentDate;
            }
        }

        private void InitWorkingDay(DateTime currentDate, out TimeOnly currentTime, out TimeOnly endOfWorkingDayTime, out TimeOnly startOfWorkingDayTime)
        {
            currentTime = new TimeOnly(currentDate.Hour, currentDate.Minute, 0);
            endOfWorkingDayTime = new TimeOnly(_endWorkingHour, _endWorkingMinute, 0);
            startOfWorkingDayTime = new TimeOnly(_startWorkingHour, _startWorkingMinute, 0);
        }

        private bool IsHoliday(DateTime currentDate)
        {
            return _holidays.Contains(new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day));
        }

        private bool IsRecurringHoliday(DateTime currentDate)
        {
            return _recurringHolidays.Contains(new(currentDate.Month, currentDate.Day));
        }

        private bool IsWeekEnd(DateTime currentDate)
        {
            return !_workingDays.Contains(currentDate.DayOfWeek);
        }

        private long GetWorkingDayDurationTicks()
        {
            var endDate = new TimeOnly(_endWorkingHour, _endWorkingMinute, 0);
            var startDate = new TimeOnly(_startWorkingHour, _startWorkingMinute, 0);
            return endDate.Ticks - startDate.Ticks;
        }
    }
}