using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Combat.Repositories
{
    public class JsonService
    {
        public EncounterRepository EncounterRepository { get; private set; } = new EncounterRepository();

        public JsonService(IEnumerable<Encounter> encounters)
        {
            EncounterRepository = new EncounterRepository(encounters);
        }

        public JsonService()
        {
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(EncounterRepository.GetEncounters()
                , GetJsonSettings());
        }

        public void SaveToFile(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                new JsonSerializer().Serialize(jsonWriter, EncounterRepository.GetEncounters());
            }
        }

        public static JsonService ReadFromString(string jsonString)
        {
            JsonSerializerSettings jsonSettings = GetJsonSettings();

            List<Encounter> encounters = JsonConvert.DeserializeObject<List<Encounter>>(jsonString,
                jsonSettings);

            return new JsonService(encounters);
        }

        private static JsonSerializerSettings GetJsonSettings()
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

        public static JsonService ReadFromFile(string path)
        {
            return ReadFromString(File.ReadAllText(path));
        }
    }
}
