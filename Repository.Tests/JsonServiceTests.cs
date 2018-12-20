using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Combat.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repository.Tests
{
    [TestClass]
    public class JsonServiceTests
    {
        [TestMethod]
        public void ReadString()
        {
            string json = GetSimpleJson();

            JsonService jsonService = JsonService.ReadFromString(json);

            string actual = jsonService.EncounterRepository.GetEncounters().SelectMany(e => e.Characters).First().Name;
            Assert.AreEqual("Adam", actual);
        }

        [TestMethod]
        public void GetJson()
        {
            string expected = string.Concat(GetSimpleJson().Where(c => !char.IsWhiteSpace(c)));

            List<Encounter> encounters = new List<Encounter>()
            {
                new Encounter("Epic", new List<Character>()
                {
                    new Character("Adam", 2),
                    new Character("Bill", -2)
                })
            };

            string actual = new JsonService(encounters).GetJson();

            Assert.AreEqual(expected, actual);
        }

        private static string GetSimpleJson()
        {
            return @"
                [
                    { 
                        ""identifier"": ""Epic"",
                        ""characters"": [
                            {
                                ""name"": ""Adam"",
                                ""initiative"": 2
                            },
                            {
                                ""name"": ""Bill"",
                                ""initiative"": -2
                            }
                        ]
                    }
                ]
            ";
        }
    }
}
