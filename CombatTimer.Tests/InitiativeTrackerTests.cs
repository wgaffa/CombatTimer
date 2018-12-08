using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Combat;
using System.Collections.Generic;

namespace CombatTimer.Tests
{
    [TestClass]
    public class InitiativeTrackerTests
    {
        private List<InitiativeRoll> _initiativeRolls = new List<InitiativeRoll>();
        private List<Character> _characters = new List<Character>();

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
        public void EmptyCombatants()
        {
            Assert.ThrowsException<ArgumentException>(() => new InitiativeTracker(new List<InitiativeRoll>()));
        }

        [TestMethod]
        public void NullCollect()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InitiativeTracker(null));
        }

        [TestMethod]
        public void NextInitiative()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            InitiativeRoll nextUp = tracker.Next();

            Assert.AreEqual("Fighter #1", nextUp.Character.Name);
        }

        [TestMethod]
        public void NextThreeInitiative()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            InitiativeRoll nextUp = null;
            for (int i = 0; i < 3; i++)
            {
                nextUp = tracker.Next();
            }

            Assert.AreEqual("Enemy #1", nextUp.Character.Name);
        }

        [TestMethod]
        public void AllActionsDoneNewRound()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            for (int i = 0; i < 5; i++)
            {
                tracker.Next();
            }

            Assert.AreEqual(2, tracker.CurrentRound);
        }
    }
}
