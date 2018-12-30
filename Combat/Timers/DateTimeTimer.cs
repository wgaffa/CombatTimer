using System;

namespace Combat.Timers
{
    public class DateTimeTimer : ITimer
    {
        private DateTime? _startTime;

        public void Start()
        {
            _startTime = DateTime.Now;
        }

        public TimeSpan Stop()
        {
            return DateTime.Now - _startTime.GetValueOrDefault(DateTime.Now);
        }
    }
}
