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
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJsonWithDuplicate());

            Assert.AreEqual(1, encounterRepository.GetEncounters().Count());
        }

        private string GetSimplelJsonWithDuplicate()
        {
            return @"[{ ""identifier"":""Epic"",""characters"":[{""name"":""Adam"",""initiative"":2},{""name"":""Bill"",""initiative"":-2}]},{""identifier"":""Epic"",""characters"":[{""name"":""Adam"",""initiative"":2},{""name"":""Bill"",""initiative"":-2}]}]";
        }

        private string GetSimplelJson()
        {
            return @"[{""identifier"":""Epic"",""characters"":[{""name"":""Adam"",""initiative"":2},{""name"":""Bill"",""initiative"":-2},{""name"":""Orc""},{""name"":""Orc chieftain""}]},{""identifier"":""Random"",""characters"":[{""name"":""Adam"",""initiative"":2},{""name"":""Bill"",""initiative"":-2},{""name"":""Goblin Boar Rider""}]},{""identifier"":""Skill encounter"",""characters"":[{""name"":""Adam"",""initiative"":2},{""name"":""Bill"",""initiative"":-2},{""name"":""Simple Thief""}]}]";
        }

        [TestMethod]
        public void Constructor_GivenJson_ShouldPopulate()
        {
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

            List<Encounter> expected = CreateEncounters().OrderBy(e => e.Identifier).ToList();

            List<Encounter> actual = encounterRepository.GetEncounters().OrderBy(e => e.Identifier).ToList();

            Assert.AreEqual(3, actual.Count);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Add_GivenNoDuplicate_ShouldPopuplate()
        {
            Encounter newEncounter = new Encounter("New encounter");
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

            encounterRepository.Add(newEncounter);

            Assert.AreEqual(4, encounterRepository.GetEncounters().Count());
        }

        [TestMethod]
        public void Add_GivenDuplicate_ShouldThrow()
        {
            Encounter addEncounter = new Encounter("Random");
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

            Assert.ThrowsException<ArgumentException>(() => encounterRepository.Add(addEncounter));
        }

        [TestMethod]
        public void GetEncounter_GivenExistingIdentifier_ShouldReturnEncounter()
        {
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

            Encounter expected = new Encounter("Random");
            Encounter actual = encounterRepository.GetEncounter("Random");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetEncounter_GivenNonExistingIdentifier_ShouldReturnNull()
        {
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

            Encounter encounter = encounterRepository.GetEncounter("Fake id");

            Assert.IsNull(encounter);
        }

        [TestMethod]
        public void GetEncounters_ShouldReturnCollection()
        {
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

            Assert.AreEqual(3, encounterRepository.GetEncounters().Count());
        }

        [TestMethod]
        public void GetEncounters_GivenPredicate_ShouldReturnCollection()
        {
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

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
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

            encounterRepository.Remove(new Encounter("Skill encounter"));

            Assert.AreEqual(2, encounterRepository.GetEncounters().Count());
        }

        [TestMethod]
        public void Remove_GivenNonExistingIdentifier_ShouldIgnore()
        {
            EncounterRepository encounterRepository = new EncounterRepository(GetSimplelJson());

            encounterRepository.Remove(new Encounter("Non existing"));

            Assert.AreEqual(3, encounterRepository.GetEncounters().Count());
        }

        private static IEnumerable<Encounter> CreateEncounters()
        {
            List<Character> epicCharacters = new List<Character>()
            {
                new Character("Adam"),
                new Character("Bill"),
                new Character("Orc"),
                new Character("Orc chieftain")
            };

            List<Character> randomCharacters = new List<Character>()
            {
                new Character("Adam"),
                new Character("Bill"),
                new Character("Goblin Boar Rider")
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
