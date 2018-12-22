using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Combat.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repository.Tests
{
    [TestClass]
    public class EncounterRepositoryTests
    {
        [TestMethod]
        public void Constructor_GivenIdenticalIdentifier_ShouldIgnore()
        {
            List<Encounter> encounters = new List<Encounter>()
            {
                new Encounter("Epic fight"),
                new Encounter("Epic fight")
            };

            EncounterRepository encounterRepository = new EncounterRepository(encounters);

            Assert.AreEqual(1, encounterRepository.GetEncounters().Count());
        }

        [TestMethod]
        public void Constructor_GivenEncounters_ShouldPopulate()
        {
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            List<Encounter> expected = CreateEncounters().OrderBy(e => e.Identifier).ToList();

            List<Encounter> actual = encounterRepository.GetEncounters().OrderBy(e => e.Identifier).ToList();

            Assert.AreEqual(3, actual.Count);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Add_GivenNoDuplicate_ShouldPopuplate()
        {
            Encounter newEncounter = new Encounter("New encounter");
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            encounterRepository.Add(newEncounter);

            Assert.AreEqual(4, encounterRepository.GetEncounters().Count());
        }

        [TestMethod]
        public void Add_GivenDuplicate_ShouldThrow()
        {
            Encounter addEncounter = new Encounter("Random");
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            Assert.ThrowsException<ArgumentException>(() => encounterRepository.Add(addEncounter));
        }

        [TestMethod]
        public void GetEncounter_GivenExistingIdentifier_ShouldReturnEncounter()
        {
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            Encounter expected = new Encounter("Random");
            Encounter actual = encounterRepository.GetEncounter("Random");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetEncounter_GivenNonExistingIdentifier_ShouldReturnNull()
        {
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            Encounter encounter = encounterRepository.GetEncounter("Fake id");

            Assert.IsNull(encounter);
        }

        [TestMethod]
        public void GetEncounters_ShouldReturnCollection()
        {
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            Assert.AreEqual(3, encounterRepository.GetEncounters().Count());
        }

        [TestMethod]
        public void GetEncounters_GivenPredicate_ShouldReturnCollection()
        {
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            List<Encounter> encounters = encounterRepository.GetEncounters(e => e.Characters.Any(c => c.Name.Contains("Orc"))).ToList();

            List<Encounter> expected = new List<Encounter>()
            {
                new Encounter("Epic")
            };

            CollectionAssert.AreEqual(expected, encounters);
        }

        [TestMethod]
        public void Remove_GivenIdentifier()
        {
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            encounterRepository.Remove(new Encounter("Skill encounter"));

            Assert.AreEqual(2, encounterRepository.GetEncounters().Count());
        }

        [TestMethod]
        public void Remove_GivenNonExistingIdentifier_ShouldIgnore()
        {
            EncounterRepository encounterRepository = new EncounterRepository(CreateEncounters());

            encounterRepository.Remove(new Encounter("Non existing"));

            Assert.AreEqual(3, encounterRepository.GetEncounters().Count());
        }

        private static IEnumerable<Encounter> CreateEncounters()
        {
            List<Character> epicCharacters = new List<Character>()
            {
                new Character("Adam"),
                new Character("Bill"),
                new Character("Orc grunt"),
                new Character("Orc chieftain")
            };

            List<Character> randomCharacters = new List<Character>()
            {
                new Character("Adam"),
                new Character("Bill"),
                new Character("Goblin boar rider")
            };

            List<Character> skillCharacters = new List<Character>()
            {
                new Character("Adam"),
                new Character("Bill"),
                new Character("Simple Thief")
            };

            List<Encounter> encounters = new List<Encounter>()
            {
                new Encounter("Epic", epicCharacters),
                new Encounter("Random", randomCharacters),
                new Encounter("Skill encounter", skillCharacters)
            };

            return encounters;
        }
    }
}
