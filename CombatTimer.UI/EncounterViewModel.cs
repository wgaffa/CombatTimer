using Combat;
using Combat.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatTimer.UI
{
    class EncounterViewModel
    {
        public Encounter Encounter { get; private set; }
        public EncounterTimer EncounterTimer { get; private set; }

        public EncounterViewModel()
        {
            JsonEncounterRepository encounterRepository = new JsonEncounterRepository(File.ReadAllText("sample-encounter.json"));
            Encounter = encounterRepository.GetEncounter("Epic");

            List<InitiativeRoll> initiativeRolls = new List<InitiativeRoll>();
            foreach (Character character in Encounter.Characters)
            {
                initiativeRolls.Add(new InitiativeRoll(character));
            }

            EncounterTimer = new EncounterTimer(initiativeRolls);
            EncounterTimer.Next();
        }
    }
}
