using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Server
{
    public static class ApplicationGlobals
    {
        // these fields are global flags for ease of use
        public static string FatalError;
        public static bool ApiTokenVerified = false;
        public static float ReportingTime = 3f;
        public static float? LastVrControllerPress;

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

        public static string TaskStatusTreatAsOpen;

        public static bool ScoreSaveAllowed;
        public static string ScoreSaveDesignCode;
        public static string ScoreSaveCollectionCode;
        public static string ScoreSaveNameAttributeCode;
        public static string ScoreSaveScoreAttributeCode;

        public static string PrefabDefault;
        public static IDictionary<string, PrefabMappingRecord> PrefabMapping;
        public static IDictionary<string, List<string>> PrefabReverseMapping; // maps dodi's to prefabs

        public static Color MeshColourDefault;
        public static IDictionary<string, Color> MeshColourMapping; // maps dodi's to colours

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

            InspectionCompleteAttributes = configJson?.InspectionCompleteAttributes ?? Config.Default.InspectionCompleteAttributes;

            DayNightCycleSeconds = Mathf.Max(5f, configJson?.DayNightCycleSeconds ?? Config.Default.DayNightCycleSeconds);

            DefectSpawn = configJson?.DefectSpawn ?? Config.Default.DefectSpawn;
            DefectSpawnMinTime = configJson?.DefectSpawnMinTime ?? Config.Default.DefectSpawnMinTime;
            DefectSpawnMaxTime = configJson?.DefectSpawnMaxTime ?? Config.Default.DefectSpawnMaxTime;
            DefectSpawnMinRange = configJson?.DefectSpawnMinRange ?? Config.Default.DefectSpawnMinRange;
            DefectSpawnMaxRange = configJson?.DefectSpawnMaxRange ?? Config.Default.DefectSpawnMaxRange;

            JobFixAllowed = configJson?.JobFixAllowed ?? Config.Default.JobFixAllowed;
            InspectionCompleteAllowed = configJson?.InspectionCompleteAllowed ?? Config.Default.InspectionCompleteAllowed;

            TaskStatusTreatAsOpen = configJson?.TaskStatusTreatAsOpen ?? Config.Default.TaskStatusTreatAsOpen;

            ScoreSaveAllowed = configJson?.ScoreSaveAllowed ?? Config.Default.ScoreSaveAllowed;
            ScoreSaveDesignCode = configJson?.ScoreSaveDesignCode ?? Config.Default.ScoreSaveDesignCode;
            ScoreSaveCollectionCode = configJson?.ScoreSaveCollectionCode ?? Config.Default.ScoreSaveCollectionCode;
            ScoreSaveNameAttributeCode = configJson?.ScoreSaveNameAttributeCode ?? Config.Default.ScoreSaveNameAttributeCode;
            ScoreSaveScoreAttributeCode = configJson?.ScoreSaveScoreAttributeCode ?? Config.Default.ScoreSaveScoreAttributeCode;

            PrefabMapping = configJson?.PrefabMapping ?? Config.Default.PrefabMapping;
            PrefabDefault = configJson?.PrefabDefault ?? Config.Default.PrefabDefault;

            // process reverse mapping for prefabs design -> prefab names
            PrefabReverseMapping = PrefabMapping.
                SelectMany(p => p.Value.DesignCodes).
                Distinct().
                ToDictionary(d => d, d => PrefabMapping.Where(p => p.Value.DesignCodes.Contains(d)).Select(p => p.Key).ToList());

            MeshColourDefault = configJson?.MeshColourDefault != null ? FloatArrayToColour(configJson?.MeshColourDefault) : FloatArrayToColour(Config.Default.MeshColourDefault);
            MeshColourMapping = configJson?.MeshColourMapping != null ? 
                configJson?.MeshColourMapping.ToDictionary(k => k.Key, v => FloatArrayToColour(v.Value)) : 
                Config.Default.MeshColourMapping.ToDictionary(k => k.Key, v => FloatArrayToColour(v.Value));
        }

        private static Color FloatArrayToColour(float[] arr)
        {
            if (arr == null)
            {
                throw new System.Exception("failed to convert float array to colour, array was null");
            }
            if (arr.Length < 3 || arr.Length > 4)
            {
                throw new System.Exception("failed to convert float array to colour, array was not of length 3 or 4");
            }
            return new Color(Mathf.Clamp(arr[0], 0f, 1f), Mathf.Clamp(arr[1], 0f, 1f), Mathf.Clamp(arr[2], 0f, 1f), arr.Length == 4 ? Mathf.Clamp(arr[3], 0f, 1f) : 1f);
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
                    { "attributes_tasksStatus", new JArray(new JValue("5bc5bdd281d088d177342c76")) },
                },

                InspectionCompleteAttributes = new JObject
                {
                    { "attributes_tasksStatus", new JArray(new JValue("5bc5bdd281d088d177342c76")) },
                },

                DayNightCycleSeconds = 60f,

                DefectSpawn = true,
                DefectSpawnMinTime = 15f,
                DefectSpawnMaxTime = 30f,
                DefectSpawnMinRange = 50f,
                DefectSpawnMaxRange = 300f,

                JobFixAllowed = true,
                InspectionCompleteAllowed = true,

                TaskStatusTreatAsOpen = "Open",

                ScoreSaveAllowed = false,
                ScoreSaveDesignCode = "<design code of the custom design to save scores to>",
                ScoreSaveCollectionCode = "Live",
                ScoreSaveNameAttributeCode = "<attribute code of the custom attribute to save name to>",
                ScoreSaveScoreAttributeCode = "<attribute code of the custom attribute to save score to>",

                PrefabDefault = "Item/SimpleTown/Prefabs/Props/grave_small_mesh",
                PrefabMapping = new Dictionary<string, PrefabMappingRecord>
                {
                    // add some available prefabs, expect users to add to any lists any custom designs
                    // they want a prefab to be represented by. we can handle MULTIPLE prefabs for a 
                    // single design code, we do this by randomising the prefab selected if the design
                    // exists as a child of more than 1 prefab

                    {
                        "Item/SimpleTown/Prefabs/Props/Aerial_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/billboard_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/bin_mesh",
                        new PrefabMappingRecord(new List<string> { "designs_wasteContainers" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/bush_large_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/bush_small_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/dish_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/dumpster_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Env_Planter",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/fence_long_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/fence_short_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/fence_short_spike",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/flag_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/flower_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/grass_square_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/grave_large_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/grave_medium_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/grave_small_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/hedge_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/hydrant_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/lamp_mesh",
                        new PrefabMappingRecord(new List<string>
                        {
                            "designs_streetLights",
                            "designs_otherStreetLights",
                            "designs_signIlluminations",
                            "designs_subwayLights"
                        })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/memorial_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/path_cross_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/path_straight_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/pipe_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Beachseat_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Beachseat_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Beachseat_03",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Roadsign_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Roadsign_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Roadsign_03",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_TirePile",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Tree_01",
                        new PrefabMappingRecord(2.0f, new List<string> { "designs_trees" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Tree_02",
                        new PrefabMappingRecord(2.0f, new List<string> { "designs_trees" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Umbrella_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Umbrella_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Prop_Umbrella_03",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Props_Buoy_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/Props_Buoy_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/traffic_light_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/trash_mesh",
                        new PrefabMappingRecord(new List<string> { "designs_wasteContainers" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/tree_large_mesh",
                        new PrefabMappingRecord(2.0f, new List<string> { "designs_trees" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/tree_medium_mesh",
                        new PrefabMappingRecord(2.0f, new List<string> { "designs_trees" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Props/tree_small_mesh",
                        new PrefabMappingRecord(2.0f, new List<string> { "designs_trees" })
                    },



                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Beach_Corner",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Beach_Short",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Beach_Straight",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Canal_Corner_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Canal_Corner_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Canal_End",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Canal_Pipe_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Canal_Pipe_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Canal_Straight",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Car_Bridge",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Car_Bridge_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Foot_Bridge",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Planter",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Road_Corner",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Road_Ramp",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Road_Ramp_Pillar",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Road_Ramp_Straight",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Rocks_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Rocks_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Rocks_03",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Seawall_Corner_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Seawall_Corner_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Seawall_Straight",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Seawall_Wall",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/Env_Water_Tile",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/fence_short_spike",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/path_driveway",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_bend_left_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_bend_right_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_corner_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_cornerLines_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_crossing_center_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_crossing_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_divider_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_LaneTransition_Left",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_LaneTransition_Right",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_Roundabout",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_square_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_straight_clear_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_straight_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/road_t_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Environment/roadLane_straight_Centered_mesh",
                        new PrefabMappingRecord()
                    },


                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_ApartmentLarge_Brown",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_ApartmentLarge_Orange",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_ApartmentLarge_Red",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_ApartmentSmall_Brown",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_ApartmentSmall_Orange",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_ApartmentSmall_Red",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_AutoRepair",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_BaberShop",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Cinema",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_CoffeeShop",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Garage_01",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Garage_02",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Garage_03",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Gym",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_01",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_02",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_03",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_04",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_05",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_06",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_07",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_08",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_09",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_010",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_011",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_012",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_013",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_014",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_015",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_Green",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_Orange",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_House_Red",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Mall",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeLarge_Blue",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeLarge_Brown",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeLarge_Grey",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeMedium_Blue",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeSmall_Blue",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeSmall_Brown",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeSmall_Grey",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeStepped_Blue",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeStepped_Brown",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_OfficeStepped_Grey",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Shop_01",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Shop_02",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Shop_03",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Shop_04",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Shop_05",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Store_Drug",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Store_Pawn",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_Store_Video",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_StoreCorner_Drug",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_StoreCorner_Pawn",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_StoreCorner_Video",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },
                    {
                        "Item/SimpleTown/Prefabs/Buildings/Building_StripClub",
                        new PrefabMappingRecord(new List<string> { "designs_nlpgPremises" })
                    },


                    {
                        "Item/SimpleTown/Prefabs/Vehicles/ambo_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/bus_blue",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/bus_brown",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/bus_grey",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/car_blue",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/car_green",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/car_red",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/cop_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/fire_truck_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/rubbishTruck_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/taxi_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/ute_empty_blue",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/ute_empty_red",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/ute_empty_yellow",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/ute_mesh_blue",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/ute_mesh_red",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/ute_mesh_yellow",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/van_mesh_blue",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/van_mesh_green",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/van_mesh_red",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/Vehicle_Boat_01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/Vehicle_Boat_02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleTown/Prefabs/Vehicles/Vehicle_Boat_03",
                        new PrefabMappingRecord()
                    },


                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Barrier01",
                        new PrefabMappingRecord(new List<string> { "designs_bollards" })
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Barrier02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Detour_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_DoNotEnter_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_GiveWay_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Manholecover",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_PowerPole",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_RoadClosed_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Roadcone01",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Roadcone02",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Roadwork_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_RoadWorkAhead_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Speed10_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Speed20_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Speed30_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Speed50_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Stop_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_StormWaterDrain",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Prop_Warning_Sign",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Road_Roadwork_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Road_straight_mesh",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Vehicle_DumpTruck_Blue",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Vehicle_DumpTruck_Red",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Vehicle_DumpTruck_Yellow",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Vehicle_MixerTruck_Cyan",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Vehicle_MixerTruck_Red",
                        new PrefabMappingRecord()
                    },
                    {
                        "Item/SimpleRoadwork/Prefabs/Vehicle_MixerTruck_Yellow",
                        new PrefabMappingRecord()
                    },

                },

                // defaults for polygons
                MeshColourDefault = new float[] { 0.254717f, 0.254717f, 0.254717f },
                MeshColourMapping = new Dictionary<string, float[]>
                {
                    { "designs_nlpgPremises", new float[] { 0.2202741f, 0.4245283f, 0.2484958f } }
                }
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

            public string TaskStatusTreatAsOpen;
        
            public bool ScoreSaveAllowed;
            public string ScoreSaveDesignCode;
            public string ScoreSaveCollectionCode;
            public string ScoreSaveNameAttributeCode;
            public string ScoreSaveScoreAttributeCode;

            public string PrefabDefault;
            public IDictionary<string, PrefabMappingRecord> PrefabMapping;

            public float[] MeshColourDefault;
            public IDictionary<string, float[]> MeshColourMapping;
        }

        public class PrefabMappingRecord
        {
            public float Scale;
            public IList<string> DesignCodes;

            public PrefabMappingRecord() : this(1.0f, new List<string>())
            {
            }

            public PrefabMappingRecord(float scale) : this(scale, new List<string>())
            {
            }

            public PrefabMappingRecord(IList<string> designCodes) : this(1.0f, designCodes)
            {
            }

            public PrefabMappingRecord(float scale, IList<string> designCodes)
            {
                Scale = scale;
                DesignCodes = designCodes;
            }
        }
    }
}
