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

        private List<InitiativeRoll> _initiatives;

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
            SortInitiatives();
        }

        public IReadOnlyList<InitiativeRoll> Initiatives => _initiatives.AsReadOnly();

        /// <summary>
        /// Start the next turn.
        /// </summary>
        /// <returns>The next <see cref="InitiativeRoll"/> to act.</returns>
        public virtual InitiativeRoll Next()
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

            if (nextInitiative.Status != InitiativeStatus.Active && nextInitiative.Status != InitiativeStatus.Complete)
                nextInitiative.Status = InitiativeStatus.Active;

            CurrentInitiative = nextInitiative;

            return nextInitiative;
        }

        public virtual InitiativeRoll Delay()
        {
            if (CurrentInitiative == null)
                throw new InvalidOperationException("skirmish not started, unable to delay");

            return PauseCurrentInitiative();
        }

        public virtual InitiativeRoll Resume(InitiativeRoll initiativeToResume)
        {
            initiativeToResume.Status = InitiativeStatus.Active;

            BumpInitiativeBeforeCurrent(initiativeToResume);

            CurrentInitiative = initiativeToResume;
            CurrentInitiative.Status = InitiativeStatus.Active;

            return initiativeToResume;
        }

        private InitiativeRoll PauseCurrentInitiative()
        {
            CurrentInitiative.Status = InitiativeStatus.Paused;

            InitiativeRoll nextInitiative = GetInitiativeOffset();

            CurrentInitiative = nextInitiative ?? BeginNewRound();

            return CurrentInitiative;
        }

        private InitiativeRoll GetInitiativeOffset(int offset = 1)
        {
            int index = _initiatives.IndexOf(CurrentInitiative);
            return _initiatives.ElementAtOrDefault(index + offset);
        }

        internal void BumpInitiativeBeforeCurrent(InitiativeRoll initiativeToResume)
        {
            int nextInitiative = CurrentInitiative.RolledInitiative;
            int previousInitiative = GetInitiativeOffset(-1)?.RolledInitiative ?? nextInitiative + 20;

            int initiativeBetween = (previousInitiative + nextInitiative) / 2;
            initiativeToResume.RolledInitiative = initiativeBetween;

            SortInitiatives();
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
                if (initiative.Status == InitiativeStatus.Complete)
                    initiative.Status = InitiativeStatus.Active;
            }

            CurrentRound++;
            return nextInitiative;
        }

        private void End()
        {
            Debug.Assert(CurrentInitiative != null);
            CurrentInitiative.Status = InitiativeStatus.Complete;
        }

        private InitiativeRoll BeginSkirmish()
        {
            SortInitiatives();
            CurrentInitiative = _initiatives.Max();
            return CurrentInitiative;
        }
    }
}
