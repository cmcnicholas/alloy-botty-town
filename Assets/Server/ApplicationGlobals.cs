using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;

namespace Assets.Server
{
    public static class ApplicationGlobals
    {
        // these fields are global flags for ease of use
        public static string FatalError;
        public static bool ApiTokenVerified = false;
        public static float ReportingTime = 3f;

        // the rest are loaded from the config.json file
        public static string ApiUrl;
        public static string ExtendedApiUrl;
        public static string ApiToken;

        public static float CentreLat;
        public static float CentreLng;

        public static string DefectCreateDesignCode;
        public static string DefectCreateCollection;
        public static JObject DefectCreateAttributes;

        public static JObject JobFixAttributes;

        public static JObject InspectionCompleteAttributes;

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

        public static string GetConfigFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "bottytown.config");
        }

        public static void Init()
        {
            // if we don't have a config then make a dummy
            if (!File.Exists(GetConfigFilePath()))
            {
                File.WriteAllText(GetConfigFilePath(), JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
            }

            Config configJson = JsonConvert.DeserializeObject<Config>(File.ReadAllText(GetConfigFilePath()));

            // set globals with fallbacks
            ApiUrl = configJson?.ApiUrl ?? Config.Default.ApiUrl;
            ExtendedApiUrl = configJson?.ExtendedApiUrl ?? Config.Default.ExtendedApiUrl;
            ApiToken = configJson?.ApiToken ?? Config.Default.ApiToken;

            CentreLat = configJson?.CentreLat ?? Config.Default.CentreLat;
            CentreLng = configJson?.CentreLng ?? Config.Default.CentreLng;

            DefectCreateDesignCode = configJson?.DefectCreateDesignCode ?? Config.Default.DefectCreateDesignCode;
            DefectCreateCollection = configJson?.DefectCreateCollection ?? Config.Default.DefectCreateCollection;
            DefectCreateAttributes = configJson?.DefectCreateAttributes ?? Config.Default.DefectCreateAttributes;

            JobFixAttributes = configJson?.JobFixAttributes ?? Config.Default.JobFixAttributes;

            InspectionCompleteAllowed = configJson?.InspectionCompleteAllowed ?? Config.Default.InspectionCompleteAllowed;

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
                    { "attributes_defectsDefectNumber", new JValue(8008135) },
                    { "attributes_exampleDefectsDangerous", new JValue(true) }
                },

                JobFixAttributes = new JObject
                {
                    { "attributes_tasksStatus", new JArray(new JValue("5bc5bdd281d088d177342c72")) },
                },

                InspectionCompleteAttributes = new JObject
                {
                    { "attributes_tasksStatus", new JArray(new JValue("5bc5bdd281d088d177342c72")) },
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

            public JObject JobFixAttributes;

            public JObject InspectionCompleteAttributes;

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
