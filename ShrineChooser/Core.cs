using System.Collections.Generic;
using System.Reflection;
using Il2Cpp;
using Il2CppAssets.Scripts.Managers;
using Il2CppInterop.Runtime;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(ShrineChooser.Core), "ShrineChooser", "1.0.1", "Potatoe_Man", null)]
[assembly: MelonGame("Vedinad", "Megabonk Demo")]

namespace ShrineChooser
{
    public class Core : MelonMod
    {
        private static bool inRound = false;

        private static string roundStartObjectName = "Chest(Clone)";
        private static string roundEndObjectName = "B_ChestRigged";
        List<string> ShrineNames = new List<string> { "ChallengeShrine(Clone)", "MagnetShrine(Clone)", "MoaiShrine(Clone)", "ShadyGuy(Clone)", "BalanceShrine(Clone)" };
        public override void OnUpdate()
        {
            // 1. Detect round start
            if (GameObject.Find(roundStartObjectName) != null)
            {
                inRound = true;

                

                //MelonLogger.Msg(GameObject.Find("PauseUi").name);

                var counts = CountRootObjectsByName(ShrineNames);

                int index = ShrineWanted - 1;
                string key = ShrineNames[index];

                if (counts[key] >= WantedAmount)
                {
                    inRound = true;
                }

                else
                {
                    MelonLogger.Msg("FUCK");
                    MapController.RestartRun();
                    inRound = false;
                }
            }

            // 2. Detect round end
            if (GameObject.Find(roundEndObjectName) != null)
            {
                inRound = false;
            }
        }
        GameObject FindFirstWithComponent(Il2CppSystem.Type componentType)
        {
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                var result = SearchDescendantsForComponent(root.transform, componentType);
                if (result != null)
                    return result;
            }
            return null;
        }

        GameObject SearchDescendantsForComponent(Transform parent, Il2CppSystem.Type componentType)
        {
            if (parent.gameObject.GetComponent(componentType) != null)
                return parent.gameObject;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                GameObject found = SearchDescendantsForComponent(child, componentType);
                if (found != null)
                    return found;
            }

            return null;
        }

        public override void OnInitializeMelon()
        {
            Load();
            MelonLogger.Warning("MAKE SURE TO GO INTO USERLIBS.SHRINECHOOSER.SHRINECHOOSERCONFIG.CFG TO MAKE THIS WORK!!!");
            MelonLogger.Msg(WantedAmount);
        }

        public static string ConfigPath = Path.Combine(MelonEnvironment.UserDataDirectory, "ShrineChooser", "ShrineChooserConfig.cfg");

        public static int ShrineWanted = 5;
        public static int WantedAmount = 9;
        public static string LogPrefix = "[ShrineChooser]";

        public static void Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    Save(); // Create default config if missing
                    return;
                }

                foreach (var line in File.ReadAllLines(ConfigPath))
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("#") || string.IsNullOrWhiteSpace(trimmed))
                        continue;

                    var split = trimmed.Split('=', 2);
                    if (split.Length != 2)
                        continue;

                    string key = split[0].Trim();
                    string value = split[1].Trim();

                    switch (key)
                    {
                        case "ShrineWanted":
                            int.TryParse(value, out ShrineWanted);
                            break;
                        case "WantedAmount":
                            int.TryParse(value, out WantedAmount);
                            break;
                        case "LogPrefix":
                            LogPrefix = value;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"Error loading config: {ex}");
            }
        }
        GameObject FindThisOneThing(Transform parent)
        {
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);

                if (child.gameObject.name == "ChestOpeningWindow")
                {
                    LoggerInstance.Msg("🎯 Found ChestOpeningWindow!");
                    return child.gameObject;
                }

                GameObject found = FindThisOneThing(child);
                if (found != null)
                    return found;
            }

            return null;
        }
        public static Dictionary<string, int> CountRootObjectsByName(List<string> names)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            HashSet<string> nameSet = new HashSet<string>(names);

            // Initialize counts to zero
            foreach (var name in names)
                counts[name] = 0;

            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject obj in rootObjects)
            {

                if (nameSet.Contains(obj.name))
                    counts[obj.name]++;
            }


            return counts;
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));

                using (StreamWriter writer = new StreamWriter(ConfigPath))
                {
                    writer.WriteLine("# ShrineChooser Config");
                    writer.WriteLine("ShrineWanted=" + ShrineWanted);
                    writer.WriteLine("1 = Challenge Shrine, 2 = Magnet Shrine, 3 = Moai Shrine, 4 = Shady guy, 5 = Balance Shrine");
                    writer.WriteLine(" ");
                    writer.WriteLine("WantedAmount=" + WantedAmount);
                    writer.WriteLine("The amount of the Shrine you want.");
                    writer.WriteLine(" ");
                    writer.WriteLine("LogPrefix=" + LogPrefix);
                }
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"Error saving config: {ex}");
            }
        }


    }
}