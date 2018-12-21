using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Combat.Repositories
{
    public class JsonService
    {
        public EncounterRepository EncounterRepository { get; private set; } = new EncounterRepository();

        public JsonService(IEnumerable<Encounter> encounters)
        {
            EncounterRepository = new EncounterRepository(encounters);
        }

        public JsonService(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            StringBuilder jsonBuilder = new StringBuilder();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                jsonBuilder.Append(line);
            }

            EncounterRepository = new EncounterRepository(DeserializeEncounters(jsonBuilder.ToString()));
        }

        public JsonService(string json)
        {
            EncounterRepository = new EncounterRepository(DeserializeEncounters(json));
        }

        private IEnumerable<Encounter> DeserializeEncounters(string json)
        {
            List<Encounter> encounters = JsonConvert.DeserializeObject<List<Encounter>>(json,
                            GetJsonSettings());
            return encounters;
        }

        public JsonService()
        {
        }
        
        public void Save(TextWriter writer)
        {
            string json = JsonConvert.SerializeObject(EncounterRepository.GetEncounters()
                , GetJsonSettings());
            writer.Write(json);
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
    }
}
