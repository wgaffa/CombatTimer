using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combat.Repositories
{
    public class InMemoryEncounterRepository : IEncounterRepository
    {
        private HashSet<Encounter> _encounters = new HashSet<Encounter>();

        public InMemoryEncounterRepository()
        {
            Character adam = new Character("Adam", 2);
            Character bill = new Character("Bill", -2);
            Character orc = new Character("Orc");
            Character orcChieftain = new Character("Orc Chieftain", 1);
            Character goblinBoarRider = new Character("Goblin Boar Rider", 5);
            Character simpleThief = new Character("Simple Thief", 3);
            _encounters = new HashSet<Encounter>()
            {
                new Encounter("Epic", new List<Character>() { adam, bill, orc, orcChieftain })
                , new Encounter("Random", new List<Character>() { adam, bill, goblinBoarRider })
                , new Encounter("Skill encounter", new List<Character>() { adam, bill, simpleThief })
            };
        }

        public void Add(Encounter encounter)
        {
            _encounters.Add(encounter);
        }

        public Encounter GetEncounter(string identifier)
        {
            return _encounters.Where(e => e.Identifier == identifier).SingleOrDefault();
        }

        public IEnumerable<Encounter> GetEncounters()
        {
            return _encounters;
        }

        public IEnumerable<Encounter> GetEncounters(Func<Encounter, bool> predicate)
        {
            return _encounters.Where(e => predicate(e));
        }

        public void Remove(Encounter encounter)
        {
            _encounters.Remove(encounter);
        }
    }
}
