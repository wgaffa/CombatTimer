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
        private List<InitiativeRoll> _combatantsActed = new List<InitiativeRoll>();

        public InitiativeTracker(IEnumerable<InitiativeRoll> initiatives)
        {
            if (initiatives == null)
                throw new ArgumentNullException(nameof(initiatives));

            if (initiatives.Count() < 2)
                throw new ArgumentException("atleast two combatants required", nameof(initiatives));

            _initiatives = initiatives.ToList();
        }

        public IReadOnlyList<InitiativeRoll> Initiatives => _initiatives.AsReadOnly();

        public InitiativeRoll Next()
        {
            if (CurrentInitiative == null)
                return BeginSkirmish();

            _combatantsActed.Add(CurrentInitiative);
            IEnumerable<InitiativeRoll> nextInitiativeList = _initiatives
                .Except(_combatantsActed)
                .Where(x => x.RolledInitiative <= CurrentInitiative.RolledInitiative)
                .OrderByDescending(x => x.RolledInitiative)
                .ThenByDescending(x => x.Character.Initiative);

            InitiativeRoll nextInitiative = nextInitiativeList.FirstOrDefault();

            bool newRound = nextInitiative == null;
            if (newRound)
            {
                nextInitiative = _initiatives.Max();
                _combatantsActed.Clear();
                CurrentRound++;
            }

            CurrentInitiative = nextInitiative;

            return nextInitiative;
        }

        private InitiativeRoll BeginSkirmish()
        {
            CurrentInitiative = _initiatives.Max();
            return CurrentInitiative;
        }
    }
}
