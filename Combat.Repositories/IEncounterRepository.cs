using System;
using System.Collections.Generic;

namespace Combat.Repositories
{
    public interface IEncounterRepository
    {
        void Add(Encounter encounter);
        Encounter GetEncounter(string identifier);
        IEnumerable<Encounter> GetEncounters();
        IEnumerable<Encounter> GetEncounters(Func<Encounter, bool> predicate);
        void Remove(Encounter encounter);
    }
}