﻿using System;
using System.Collections.Generic;
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
        /// <returns>The next Characters turn to act.</returns>
        public Character Next()
        {
            End();

            _currentCombatant = _initiativeTracker.Next().Character;
            _currentTurnStarted = DateTime.Now;
            return _currentCombatant;
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