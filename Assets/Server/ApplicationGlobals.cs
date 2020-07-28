using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;

namespace Assets.Server
{
    public static class ApplicationGlobals
    {
        public static string ApiUrl;
        public static string ExtendedApiUrl;
        public static string ApiToken;

        public static float CentreLat;
        public static float CentreLng;

        public static string DefectCreateDesignCode;
        public static string DefectCreateCollection;
        public static JObject DefectCreateAttributes;

        public static float DayNightCycleSeconds;

        public static bool DefectSpawn;
        public static float DefectSpawnMinTime;
        public static float DefectSpawnMaxTime;
        public static float DefectSpawnMinRange;
        public static float DefectSpawnMaxRange;

        public static bool JobFixAllowed;
        public static bool InspectionCompleteAllowed;

        public static bool ScoreSaveAllowed;
        public static string ScoreSaveDesignCode;
        public static string ScoreSaveCollectionCode;
        public static string ScoreSaveNameAttributeCode;
        public static string ScoreSaveScoreAttributeCode;

        public static void Init()
        {
            string configPath = Path.Combine(Application.persistentDataPath, "bottytown.config");

            // if we don't have a config then make a dummy
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
            }

            Config configJson = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));

            // set globals with fallbacks
            ApiUrl = configJson?.ApiUrl ?? Config.Default.ApiUrl;
            ExtendedApiUrl = configJson?.ExtendedApiUrl ?? Config.Default.ExtendedApiUrl;
            ApiToken = configJson?.ApiToken ?? Config.Default.ApiToken;

            CentreLat = configJson?.CentreLat ?? Config.Default.CentreLat;
            CentreLng = configJson?.CentreLng ?? Config.Default.CentreLng;

            DefectCreateDesignCode = configJson?.DefectCreateDesignCode ?? Config.Default.DefectCreateDesignCode;
            DefectCreateCollection = configJson?.DefectCreateCollection ?? Config.Default.DefectCreateCollection;
            DefectCreateAttributes = configJson?.DefectCreateAttributes ?? Config.Default.DefectCreateAttributes;

            DayNightCycleSeconds = Mathf.Max(5f, configJson?.DayNightCycleSeconds ?? Config.Default.DayNightCycleSeconds);

            DefectSpawn = configJson?.DefectSpawn ?? Config.Default.DefectSpawn;
            DefectSpawnMinTime = configJson?.DefectSpawnMinTime ?? Config.Default.DefectSpawnMinTime;
            DefectSpawnMaxTime = configJson?.DefectSpawnMaxTime ?? Config.Default.DefectSpawnMaxTime;
            DefectSpawnMinRange = configJson?.DefectSpawnMinRange ?? Config.Default.DefectSpawnMinRange;
            DefectSpawnMaxRange = configJson?.DefectSpawnMaxRange ?? Config.Default.DefectSpawnMaxRange;

            JobFixAllowed = configJson?.JobFixAllowed ?? Config.Default.JobFixAllowed;
            InspectionCompleteAllowed = configJson?.InspectionCompleteAllowed ?? Config.Default.InspectionCompleteAllowed;

            ScoreSaveAllowed = configJson?.ScoreSaveAllowed ?? Config.Default.ScoreSaveAllowed;
            ScoreSaveDesignCode = configJson?.ScoreSaveDesignCode ?? Config.Default.ScoreSaveDesignCode;
            ScoreSaveCollectionCode = configJson?.ScoreSaveCollectionCode ?? Config.Default.ScoreSaveCollectionCode;
            ScoreSaveNameAttributeCode = configJson?.ScoreSaveNameAttributeCode ?? Config.Default.ScoreSaveNameAttributeCode;
            ScoreSaveScoreAttributeCode = configJson?.ScoreSaveScoreAttributeCode ?? Config.Default.ScoreSaveScoreAttributeCode;
        }

        private class Config
        {
            public static Config Default = new Config
            {
                ApiToken = "<enter api token here, needs access to assets, jobs, defects, inspections>",
                ApiUrl = "https://api.labs.alloyapp.io",
                ExtendedApiUrl = "https://extended.api.labs.alloyapp.io",

                CentreLat = 52.28913f,
                CentreLng = -1.533704f,

                DefectCreateDesignCode = "designs_exampleDefects",
                DefectCreateCollection = "Live",
                DefectCreateAttributes = new JObject
                {
                    { "attributes_defectsDefectNumber", JToken.FromObject(8008135) },
                    { "attributes_exampleDefectsDangerous", JToken.FromObject(true) }
                },

                DayNightCycleSeconds = 60f,

                DefectSpawn = true,
                DefectSpawnMinTime = 15f,
                DefectSpawnMaxTime = 30f,
                DefectSpawnMinRange = 50f,
                DefectSpawnMaxRange = 300f,

                JobFixAllowed = true,
                InspectionCompleteAllowed = true,

                ScoreSaveAllowed = false,
                ScoreSaveDesignCode = "<design code of the custom design to save scores to>",
                ScoreSaveCollectionCode = "Live",
                ScoreSaveNameAttributeCode = "<attribute code of the custom attribute to save name to>",
                ScoreSaveScoreAttributeCode = "<attribute code of the custom attribute to save score to>",
            };

            public string ApiUrl;
            public string ExtendedApiUrl;
            public string ApiToken;

            public float CentreLat;
            public float CentreLng;

            public string DefectCreateDesignCode;
            public string DefectCreateCollection;
            public JObject DefectCreateAttributes;

            public float DayNightCycleSeconds;

            public bool DefectSpawn;
            public float DefectSpawnMinTime;
            public float DefectSpawnMaxTime;
            public float DefectSpawnMinRange;
            public float DefectSpawnMaxRange;

            public bool JobFixAllowed;
            public bool InspectionCompleteAllowed;
        
            public bool ScoreSaveAllowed;
            public string ScoreSaveDesignCode;
            public string ScoreSaveCollectionCode;
            public string ScoreSaveNameAttributeCode;
            public string ScoreSaveScoreAttributeCode;
        }
    }
}
