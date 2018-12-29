using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Combat.Repositories
{
    public class JsonEncounterRepository
    {
        private HashSet<Encounter> _encounters = new HashSet<Encounter>();
        
        public JsonEncounterRepository(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("cannot be null or contain only whitespace", nameof(json));

            _encounters = new HashSet<Encounter>(DeserializeEncounters(json));
        }

        public JsonEncounterRepository(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));

            _encounters = new HashSet<Encounter>(DeserializeEncounters(File.ReadAllText(fileInfo.FullName)));
        }
        
        private IEnumerable<Encounter> DeserializeEncounters(string json)
        {
            List<Encounter> encounters = JsonConvert.DeserializeObject<List<Encounter>>(json,
                            GetJsonSettings());
            return encounters;
        }

        private JsonSerializerSettings GetJsonSettings()
        {
            DefaultContractResolver camelCaseResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = camelCaseResolver,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            return jsonSettings;
        }

        private static string ReadAllLines(TextReader reader)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                jsonBuilder.Append(line);
            }

            return jsonBuilder.ToString();
        }

        public void Add(Encounter encounter)
        {
            if (_encounters.Contains(encounter))
                throw new ArgumentException("encounter allready exists", nameof(encounter));

            _encounters.Add(encounter);
        }

        public Encounter GetEncounter(string identifier)
        {
            return _encounters.Where(e => e.Identifier == identifier).SingleOrDefault();
        }

        public IEnumerable<Encounter> GetEncounters()
        {
            return _encounters.ToList();
        }

        public IEnumerable<Encounter> GetEncounters(Func<Encounter, bool> predicate)
        {
            return _encounters.Where(predicate);
        }

        public void Remove(Encounter encounter)
        {
            _encounters.Remove(encounter);
        }
    }
}
