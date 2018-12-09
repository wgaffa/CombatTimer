using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat
{
    public class Skirmish
    {
        private readonly InitiativeTracker _initiativeTracker;
        private Character _currentCombatant = null;
        private DateTime _currentTurnStarted;
        private List<RoundTimer> _roundTimers = new List<RoundTimer>();

        public Skirmish(IEnumerable<InitiativeRoll> initiatives)
        {
            _initiativeTracker = new InitiativeTracker(initiatives);
        }

        public IReadOnlyList<RoundTimer> Timers => _roundTimers.AsReadOnly();

        /// <summary>
        /// Starts the next turn.
        /// </summary>
        /// <returns>The next <see cref="Character"/> turn to act.</returns>
        public Character Next()
        {
            return NewTurn(() => _initiativeTracker.Next().Character);
        }

        /// <summary>
        /// Delay the turn and act later, then get the next characters turn.
        /// </summary>
        /// <returns>The next <see cref="Character"/> turn to act.</returns>
        public Character Delay()
        {
            return NewTurn(() => _initiativeTracker.Delay().Character);
        }

        /// <summary>
        /// Ready the turn and act later, then the next characters turn.
        /// </summary>
        /// <returns>The next <see cref="Character"/> turn to act.</returns>
        public Character Ready()
        {
            return NewTurn(() => _initiativeTracker.Ready().Character);
        }

        private Character NewTurn(Func<Character> func)
        {
            End();

            _currentCombatant = func();
            _currentTurnStarted = DateTime.Now;

            return _currentCombatant;
        }

        public Character Resume(Character characterToResume)
        {
            InitiativeRoll initiativeToResume = _initiativeTracker.Initiatives.FirstOrDefault(x => x.Character == characterToResume);

            if (initiativeToResume == null)
                throw new InvalidOperationException("character was not found in the initiative tracker");

            return NewTurn(() => _initiativeTracker.Resume(initiativeToResume).Character);
        }

        /// <summary>
        /// Ends the current turn.
        /// </summary>
        public void End()
        {
            bool firstRoundNotStarted = _currentCombatant == null;
            if (firstRoundNotStarted)
                return;

            TimeSpan turnTime = DateTime.Now - _currentTurnStarted;

            RoundTimer timer = _roundTimers.ElementAtOrDefault(_initiativeTracker.CurrentRound - 1);
            if (timer == null)
            {
                timer = new RoundTimer();
                _roundTimers.Add(timer);
            }

            timer.Add(_currentCombatant, turnTime);
        }
    }
}
