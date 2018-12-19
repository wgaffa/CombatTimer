using System;
using System.Collections.Generic;
using Combat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CombatTimer.Tests
{
    [TestClass]
    public class EncounterTests
    {
        [TestMethod]
        public void NullCollection()
        {
            Encounter encounter = new Encounter("Test", null);

            Assert.AreEqual(0, encounter.Characters.Count);
        }

        [TestMethod]
        public void WhiteSpaceIdentifier()
        {
            Assert.ThrowsException<ArgumentException>(() => new Encounter("\t"));
        }

        [TestMethod]
        public void EmptyIdentifier()
        {
            Assert.ThrowsException<ArgumentException>(() => new Encounter(string.Empty));
        }

        [TestMethod]
        public void NullIdentifier()
        {
            Assert.ThrowsException<ArgumentException>(() => new Encounter(null));
        }

        [TestMethod]
        public void IdenticalCharacters()
        {
            List<Character> characters = new List<Character>()
            {
                new Character("Adam")
                , new Character("Adam")
                , new Character("Adam", 3)
            };

            Encounter encounter = new Encounter("Test", characters);

            Assert.AreEqual(2, encounter.Characters.Count);
        }
    }
}
