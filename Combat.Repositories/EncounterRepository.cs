using System;
using System.Collections.Generic;
using System.Linq;

namespace Combat.Repositories
{
    public class EncounterRepository
    {
        private HashSet<Encounter> _encounters = new HashSet<Encounter>();

        public EncounterRepository()
        {
        }

        public EncounterRepository(IEnumerable<Encounter> encounters)
        {
            _encounters = new HashSet<Encounter>(encounters);
        }

        public void Add(Encounter encounter)
        {
            if (_encounters.Contains(encounter))
                throw new ArgumentException("encounter allready exists", nameof(encounter));

            _encounters.Add(encounter);
        }

        public Encounter GetEncounter(string identifier)
        {
            return _encounters.Where(e => e.Identifier == identifier).SingleOrDefault();
        }

        public IEnumerable<Encounter> GetEncounters()
        {
            return _encounters.ToList();
        }

        public IEnumerable<Encounter> GetEncounters(Func<Encounter, bool> predicate)
        {
            return _encounters.Where(predicate);
        }

        public void Remove(Encounter encounter)
        {
            _encounters.Remove(encounter);
        }
    }
}
