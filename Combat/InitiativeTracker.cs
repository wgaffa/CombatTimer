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
        public int CurrentInitiative { get; private set; } = 1;

        private List<InitiativeRoll> _initiatives = new List<InitiativeRoll>();

        public InitiativeTracker(IEnumerable<InitiativeRoll> initiatives)
        {
            if (initiatives == null)
                throw new ArgumentNullException(nameof(initiatives));

            if (initiatives.Count() < 2)
                throw new ArgumentException("atleast two combatants required", nameof(initiatives));

            _initiatives = initiatives.ToList();

            CurrentInitiative = _initiatives.Max().RolledInitiative + 1;
        }

        public IReadOnlyList<InitiativeRoll> Initiatives => _initiatives.AsReadOnly();

        public InitiativeRoll Next()
        {
            InitiativeRoll nextInititiative = _initiatives
                .Where(x => x.RolledInitiative < CurrentInitiative)
                .OrderByDescending(x => x.RolledInitiative)
                .ThenByDescending(x => x.Character.Initiative)
                .FirstOrDefault();

            bool newRound = nextInititiative == null;
            if (newRound)
            {
                nextInititiative = _initiatives.Max();
                CurrentRound++;
            }

            CurrentInitiative = nextInititiative.RolledInitiative;

            return nextInititiative;
        }
    }
}
