using Combat.Timers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Combat
{
    public class EncounterTimer : InitiativeTracker
    {
        private DateTime _currentTurnStarted;
        private List<RoundTimer> _roundTimers = new List<RoundTimer>();
        private ITimer _timer;

        public ITimer Timer
        {
            get
            {
                if (_timer == null)
                    _timer = new DateTimeTimer();

                return _timer;
            }
            set
            { _timer = value; }
        }

        public EncounterTimer(IEnumerable<InitiativeRoll> initiatives)
            : base(initiatives)
        {
        }

        public IReadOnlyList<RoundTimer> Timers => _roundTimers.AsReadOnly();

        /// <summary>
        /// Starts the next turn.
        /// </summary>
        /// <returns>The next <see cref="InitiativeRoll"/> turn to act.</returns>
        public override InitiativeRoll Next()
        {
            End();
            Timer.Start();
            return base.Next();
        }

        /// <summary>
        /// Delay the turn and act later, then get the next characters turn.
        /// </summary>
        /// <returns>The next <see cref="InitiativeRoll"/> turn to act.</returns>
        public override InitiativeRoll Delay()
        {
            End();
            Timer.Start();
            return base.Delay();
        }

        public override InitiativeRoll Resume(InitiativeRoll initiativeToResume)
        {
            Timer.Start();
            return base.Resume(initiativeToResume);
        }

        /// <summary>
        /// Ends the current turn.
        /// </summary>
        public void End()
        {
            bool firstRoundNotStarted = CurrentInitiative == null;
            if (firstRoundNotStarted)
                return;

            TimeSpan turnTime = Timer.Stop();

            RoundTimer timer = _roundTimers.ElementAtOrDefault(CurrentRound - 1);
            if (timer == null)
            {
                timer = new RoundTimer();
                _roundTimers.Add(timer);
            }

            timer.Add(CurrentInitiative.Character, turnTime);
        }
    }
}
