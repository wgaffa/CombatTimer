using System;
using System.Collections.Generic;
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
            InitiativeRoll nextInitiative = _initiatives
                .Where(x => x.ActionTaken != ActionTakenType.TurnComplete && x.RolledInitiative <= CurrentInitiative.RolledInitiative)
                .OrderByDescending(x => x.RolledInitiative)
                .ThenByDescending(x => x.Character.Initiative)
                .FirstOrDefault();

            bool newRound = nextInitiative == null;
            if (newRound)
            {
                nextInitiative = BeginNewRound();
            }

            CurrentInitiative = nextInitiative;

            return nextInitiative;
        }

        private InitiativeRoll BeginNewRound()
        {
            InitiativeRoll nextInitiative = _initiatives.Max();
            foreach (InitiativeRoll initiative in _initiatives)
            {
                initiative.ActionTaken = ActionTakenType.None;
            }

            CurrentRound++;
            return nextInitiative;
        }

        private void End()
        {
            CurrentInitiative.ActionTaken = ActionTakenType.TurnComplete;
        }

        private InitiativeRoll BeginSkirmish()
        {
            CurrentInitiative = _initiatives.Max();
            return CurrentInitiative;
        }
    }
}
