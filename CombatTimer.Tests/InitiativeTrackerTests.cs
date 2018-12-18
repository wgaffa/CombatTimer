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
        public void ThirdInitiative()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            InitiativeRoll nextUp = null;
            for (int i = 0; i < 3; i++)
            {
                nextUp = tracker.Next();
            }

            Assert.AreEqual("Wizard #1", nextUp.Character.Name);
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

        [TestMethod]
        public void FighterSecondRound()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            List<InitiativeRoll> turnOrder = new List<InitiativeRoll>();
            for (int i = 0; i < 4; i++)
            {
                turnOrder.Add(tracker.Next());
            }
            tracker.Next();

            Assert.AreEqual("Fighter #1", tracker.CurrentInitiative.Character.Name);
        }

        [TestMethod]
        public void DelayActionTakenSetToDelay()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            InitiativeRoll initiative = tracker.Next();
            tracker.Delay();

            Assert.AreEqual(InitiativeStatus.Paused, initiative.Status);
        }

        [TestMethod]
        public void ResumeNewInititative()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            InitiativeRoll delayedAction = tracker.Next();

            tracker.Delay(); // Rogue turn
            tracker.Next(); // Wizard turn
            tracker.Next(); // Enemy turn

            tracker.Resume(delayedAction);

            Assert.AreEqual(10, delayedAction.RolledInitiative);
        }

        [TestMethod]
        public void ResumeAtTop()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            tracker.Next(); // Fighter

            InitiativeRoll delayedAction = tracker.Next(); // Rogue delay

            tracker.Delay(); // Wizard
            tracker.Next(); // Enemey
            tracker.Next(); // Fighter

            tracker.Resume(delayedAction);

            Assert.AreEqual(25, delayedAction.RolledInitiative);
        }

        [TestMethod]
        public void DelayUntilNextRound()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            tracker.Next(); // Fighter

            InitiativeRoll rogueDelay = tracker.Next();
            tracker.Delay(); // Wizard

            for (int i = 0; i < 2; i++) // E, Fig
            {
                tracker.Next();
            }

            InitiativeRoll initiative = tracker.Next();

            Assert.AreEqual("Rogue #1", initiative.Character.Name);
        }

        [TestMethod]
        public void DelayUntilNextRoundActionIsNone()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            tracker.Next(); // Fighter

            InitiativeRoll rogueDelay = tracker.Next();
            tracker.Delay(); // Wizard

            for (int i = 0; i < 2; i++) // E, Fig
            {
                tracker.Next();
            }

            InitiativeRoll initiative = tracker.Next();

            Assert.AreEqual(InitiativeStatus.Active, initiative.Status);
        }

        [TestMethod]
        public void OneCombatant()
        {
            Assert.ThrowsException<ArgumentException>(() => new InitiativeTracker(new List<InitiativeRoll>() { _initiativeRolls[0] }));
        }

        [TestMethod]
        public void ReadyActionTakenSetToReady()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            InitiativeRoll initiative = tracker.Next();
            tracker.Delay();

            Assert.AreEqual(InitiativeStatus.Paused, initiative.Status);
        }

        [TestMethod]
        public void ReadyNewInititative()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            InitiativeRoll delayedAction = tracker.Next();

            tracker.Delay(); // Rogue turn
            tracker.Next(); // Wizard turn
            tracker.Next(); // Enemy turn

            tracker.Resume(delayedAction);

            Assert.AreEqual(10, delayedAction.RolledInitiative);
        }

        [TestMethod]
        public void ReadyResumeAtTop()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            tracker.Next(); // Fighter

            InitiativeRoll delayedAction = tracker.Next(); // Rogue delay

            tracker.Delay(); // Wizard
            tracker.Next(); // Enemey
            tracker.Next(); // Fighter

            tracker.Resume(delayedAction);

            Assert.AreEqual(25, delayedAction.RolledInitiative);
        }

        [TestMethod]
        public void ReadyUntilNextRound()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            tracker.Next(); // Fighter

            InitiativeRoll rogueDelay = tracker.Next();
            tracker.Delay(); // Wizard

            for (int i = 0; i < 2; i++) // E, Fig
            {
                tracker.Next();
            }

            InitiativeRoll initiative = tracker.Next();

            Assert.AreEqual("Rogue #1", initiative.Character.Name);
        }

        [TestMethod]
        public void ReadyUntilNextRoundActionIsNone()
        {
            InitiativeTracker tracker = new InitiativeTracker(_initiativeRolls);

            tracker.Next(); // Fighter

            InitiativeRoll rogueDelay = tracker.Next();
            tracker.Delay(); // Wizard

            for (int i = 0; i < 2; i++) // E, Fig
            {
                tracker.Next();
            }

            InitiativeRoll initiative = tracker.Next();

            Assert.AreEqual(InitiativeStatus.Active, initiative.Status);
        }
    }
}
