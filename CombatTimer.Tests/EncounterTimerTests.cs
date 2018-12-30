using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Combat;
using Combat.Timers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CombatTimer.Tests
{
    [TestClass]
    public class CombatTests
    {
        private List<Character> _characters;
        private List<InitiativeRoll> _initiativeRolls;
        private List<InitiativeRoll> _initiativeRandomRolls;

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

            MockListRoller mockRandomRoller = new MockListRoller(new int[] { 7, 16, 15, 14 });
            _initiativeRandomRolls = new List<InitiativeRoll>()
            {
                new InitiativeRoll(_characters[0], mockRandomRoller)
                , new InitiativeRoll(_characters[1], mockRandomRoller)
                , new InitiativeRoll(_characters[2], mockRandomRoller)
                , new InitiativeRoll(_characters[3], mockRandomRoller)
            };
        }

        [TestMethod]
        public void OneTurn()
        {
            Mock<ITimer> mockTimer = new Mock<ITimer>();
            mockTimer.Setup(t => t.Stop()).Returns(TimeSpan.FromSeconds(1));

            EncounterTimer combat = new EncounterTimer(_initiativeRolls) { Timer = mockTimer.Object };

            combat.Next();
            combat.Next();

            Assert.AreEqual(1, combat.Timers[0].RoundTimes[_characters[0]].Seconds);
        }

        [TestMethod]
        public void RoundTwo()
        {
            Mock<ITimer> mockTimer = new Mock<ITimer>();
            mockTimer.SetupSequence(t => t.Stop())
                .Returns(TimeSpan.FromSeconds(1))
                .Returns(TimeSpan.FromSeconds(2))
                .Returns(TimeSpan.FromSeconds(3))
                .Returns(TimeSpan.FromSeconds(4));
            EncounterTimer combat = new EncounterTimer(_initiativeRolls);

            for (int i = 0; i <= 4; i++)
            {
                combat.Next();
            }

            combat.Timer = mockTimer.Object;
            for (int i = 0; i <= 4; i++)
            {
                combat.Next();
            }

            List<int> expected = new List<int>() { 1, 2, 3, 4 };
            List<int> actual = combat.Timers[1].RoundTimes.Select(x => x.Value.Seconds).OrderBy(x => x).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DelayAddingOnCurrentTurn()
        {
            Mock<ITimer> mockTimer = new Mock<ITimer>();
            mockTimer.Setup(t => t.Stop()).Returns(TimeSpan.FromSeconds(1));
            EncounterTimer combat = new EncounterTimer(_initiativeRolls) { Timer = mockTimer.Object };

            combat.Next(); // Fighter
            InitiativeRoll rogue = combat.Next(); // Rogue

            combat.Delay(); // Wiz
            combat.Next(); // Enemy

            combat.Resume(rogue);
            combat.End();

            Assert.AreEqual(2, combat.Timers[0].RoundTimes[rogue.Character].Seconds);
        }

        [TestMethod]
        public void DelayUntilNextTurn()
        {
            Mock<ITimer> mockTimer = new Mock<ITimer>();
            mockTimer.Setup(t => t.Stop()).Returns(TimeSpan.FromSeconds(1));
            EncounterTimer combat = new EncounterTimer(_initiativeRolls) { Timer = mockTimer.Object };

            combat.Next(); // Fighter
            combat.Next(); // Rogue
            InitiativeRoll wizard = combat.Next();
            combat.Delay(); // Enemy
            combat.Next(); // Fighter

            combat.Resume(wizard); // Wizard
            combat.End();

            List<int> wizardTimes = combat.Timers.Select(x => x.RoundTimes[wizard.Character].Seconds).ToList();
            List<int> expected = new List<int>() { 1, 1 };

            CollectionAssert.AreEqual(expected, wizardTimes);
        }

        [TestMethod]
        public void ConstructorValues()
        {
            List<int> expected = new List<int>() { 9, 22, 13, 16 };

            List<int> actual = _initiativeRandomRolls.Select(i => i.RolledInitiative).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
