using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Utilities;

namespace Trainworks.Managers
{
    public static class CustomScenarioManager
    {
        public static Dictionary<String, ScenarioData> CustomScenarioData = new Dictionary<String, ScenarioData>();

        public static void RegisterCustomScenario(ScenarioData scenario, int distance)
        {
            if (!CustomScenarioData.ContainsKey(scenario.GetID()))
            {
                CustomScenarioData.Add(scenario.GetID(), scenario);

                // Add scenario to AllGameData.
                AllGameData allGameData = ProviderManager.SaveManager.GetAllGameData();
                List<ScenarioData> scenarios;
                scenarios = (List<ScenarioData>) AccessTools.Field(typeof(AllGameData), "scenarioDatas").GetValue(allGameData);
                scenarios.Add(scenario);

                // Add scenario to appear at distance.
                BalanceData balanceData = allGameData.GetBalanceData();
                RunData runData = balanceData.GetRunData(false);
                NodeDistanceData distanceData = runData.GetDistanceData(distance);
                List<ScenarioData> distanceScenarios;
                distanceScenarios = (List<ScenarioData>)AccessTools.Field(typeof(NodeDistanceData), "battleDatas").GetValue(distanceData);
                distanceScenarios.Add(scenario);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate sinsdata with name: " + scenario.name);
            }
        }

        public static ScenarioData GetScenarioDataByID(String id)
        {
            // Search for custom sins matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(id);
            if (CustomScenarioData.ContainsKey(guid))
            {
                return CustomScenarioData[guid];
            }

            // No custom sins found; search for vanilla scenarios matching ID
            var vanillaScenario = ProviderManager.SaveManager.GetAllGameData().FindScenarioData(id);
            if (vanillaScenario == null)
            {
                Trainworks.Log(LogLevel.Warning, "Couldn't find scenario: " + id + " - This will cause crashes.");
            }
            return vanillaScenario;
        }
    }
}
