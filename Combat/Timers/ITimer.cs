using System;

namespace Combat.Timers
{
    public interface ITimer
    {
        void Start();
        TimeSpan Stop();
    }
}
