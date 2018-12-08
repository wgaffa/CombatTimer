using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Combat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CombatTimer.Tests
{
    [TestClass]
    public class CombatTests
    {
        private List<Character> _characters;
        private List<InitiativeRoll> _initiativeRolls;

        [TestInitialize]
        public void TestSetup()
        {
            _characters = new List<Character>()
            {
                new Character("Fighter #1", 2)
                , new Character("Rogue #1", 6)
                , new Character("Enemy #1", -2)
                , new Character("Wizard #1", 2)
            };

            _initiativeRolls = new List<InitiativeRoll>()
            {
                new InitiativeRoll(_characters[0], 15)
                , new InitiativeRoll(_characters[1], 14)
                , new InitiativeRoll(_characters[2], 7)
                , new InitiativeRoll(_characters[3], 14)
            };
        }

        [TestMethod]
        public void OneTurn()
        {
            Skirmish combat = new Skirmish(_initiativeRolls);

            combat.Next();
            Thread.Sleep(1000);
            combat.Next();

            Assert.AreEqual(1, combat.Timers[0].RoundTimes[_characters[0]].Seconds);
        }

        [TestMethod]
        public void RoundTwo()
        {
            Skirmish combat = new Skirmish(_initiativeRolls);

            for (int i = 0; i < 4; i++)
            {
                combat.Next();
            }

            int threshold = 1;
            for (int i = 0; i < 4; i++)
            {
                combat.Next();
                int quickThinkers = threshold < i ? -1 : 1;
                Thread.Sleep((i + quickThinkers) * 1000);
            }

            combat.Next();

            List<int> expected = new List<int>() { 1, 1, 2, 2 };
            List<int> actual = combat.Timers[1].RoundTimes.Select(x => x.Value.Seconds).OrderBy(x => x).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
