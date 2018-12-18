using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat
{
    public class EncounterTimer : InitiativeTracker
    {
        private DateTime _currentTurnStarted;
        private List<RoundTimer> _roundTimers = new List<RoundTimer>();

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
            _currentTurnStarted = DateTime.Now;
            return base.Next();
        }

        /// <summary>
        /// Delay the turn and act later, then get the next characters turn.
        /// </summary>
        /// <returns>The next <see cref="InitiativeRoll"/> turn to act.</returns>
        public override InitiativeRoll Delay()
        {
            End();
            _currentTurnStarted = DateTime.Now;
            return base.Delay();
        }

        public override InitiativeRoll Resume(InitiativeRoll initiativeToResume)
        {
            _currentTurnStarted = DateTime.Now;
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

            TimeSpan turnTime = DateTime.Now - _currentTurnStarted;

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
