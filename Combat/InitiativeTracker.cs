using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat
{
    public class InitiativeTracker
    {
        public int CurrentRound { get; private set; } = 1;
        public InitiativeRoll CurrentInitiative { get; private set; }

        private List<InitiativeRoll> _initiatives = new List<InitiativeRoll>();

        /// <summary>
        /// Initialize a new instance of the <see cref="InitiativeTracker"/> class.
        /// </summary>
        /// <param name="initiatives">A collection of initiative rolls.</param>
        public InitiativeTracker(IEnumerable<InitiativeRoll> initiatives)
        {
            if (initiatives == null)
                throw new ArgumentNullException(nameof(initiatives));

            if (initiatives.Count() < 2)
                throw new ArgumentException("atleast two combatants required", nameof(initiatives));

            _initiatives = initiatives.ToList();
        }

        public IReadOnlyList<InitiativeRoll> Initiatives => _initiatives.AsReadOnly();

        /// <summary>
        /// Start the next turn.
        /// </summary>
        /// <returns>The next <see cref="InitiativeRoll"/> to act.</returns>
        public InitiativeRoll Next()
        {
            if (CurrentInitiative == null)
                return BeginSkirmish();

            End();
            InitiativeRoll nextInitiative = GetInitiativeOffset();

            bool newRound = nextInitiative == null;
            if (newRound)
            {
                nextInitiative = BeginNewRound();
            }

            if (nextInitiative.ActionTaken != ActionTakenType.None && nextInitiative.ActionTaken != ActionTakenType.TurnComplete)
                nextInitiative.ActionTaken = ActionTakenType.None;

            CurrentInitiative = nextInitiative;

            return nextInitiative;
        }

        public InitiativeRoll Delay()
        {
            if (CurrentInitiative == null)
                throw new InvalidOperationException("skirmish not started, unable to delay");

            CurrentInitiative.ActionTaken = ActionTakenType.Delay;

            InitiativeRoll nextInitiative = GetInitiativeOffset();

            CurrentInitiative = nextInitiative ?? BeginNewRound();

            return CurrentInitiative;
        }

        public InitiativeRoll Ready()
        {
            if (CurrentInitiative == null)
                throw new InvalidOperationException("skirmish not started, unable to delay");

            CurrentInitiative.ActionTaken = ActionTakenType.Ready;

            InitiativeRoll nextInitiative = GetInitiativeOffset();

            CurrentInitiative = nextInitiative ?? BeginNewRound();

            return CurrentInitiative;
        }

        private InitiativeRoll GetInitiativeOffset(int offset = 1)
        {
            int index = _initiatives.IndexOf(CurrentInitiative);
            return _initiatives.ElementAtOrDefault(index + offset);
        }

        public InitiativeRoll BumpInitiativeBeforeCurrent(InitiativeRoll initiativeToResume)
        {
            initiativeToResume.ActionTaken = ActionTakenType.None;

            InitiativeRoll nextInitiative = CurrentInitiative;
            InitiativeRoll previousInitiative = GetInitiativeOffset(-1);

            int previousInitiativeRoll = previousInitiative?.RolledInitiative ?? nextInitiative.RolledInitiative + 20;
            int initiativeBetween = (previousInitiativeRoll + nextInitiative.RolledInitiative) / 2;
            initiativeToResume.RolledInitiative = initiativeBetween;

            CurrentInitiative = initiativeToResume;
            CurrentInitiative.ActionTaken = ActionTakenType.None;

            SortInitiatives();

            return CurrentInitiative;
        }

        private void SortInitiatives()
        {
            _initiatives = _initiatives
                .OrderByDescending(x => x.RolledInitiative)
                .ThenByDescending(x => x.Character.Initiative)
                .ToList();
        }

        private InitiativeRoll BeginNewRound()
        {
            InitiativeRoll nextInitiative = _initiatives.Max();
            foreach (InitiativeRoll initiative in _initiatives)
            {
                if (initiative.ActionTaken == ActionTakenType.TurnComplete)
                    initiative.ActionTaken = ActionTakenType.None;
            }

            CurrentRound++;
            return nextInitiative;
        }

        private void End()
        {
            Debug.Assert(CurrentInitiative != null);
            CurrentInitiative.ActionTaken = ActionTakenType.TurnComplete;
        }

        private InitiativeRoll BeginSkirmish()
        {
            SortInitiatives();
            CurrentInitiative = _initiatives.Max();
            return CurrentInitiative;
        }
    }
}
