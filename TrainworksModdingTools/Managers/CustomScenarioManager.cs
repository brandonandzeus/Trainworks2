using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Utilities;

namespace Trainworks.Managers
{
    /// <summary>
    /// Handles Custom Scenarios, Sins, and Trials.
    /// </summary>
    public static class CustomScenarioManager
    {
        /// <summary>
        /// Mpas custom scenario IDs to their respective ScenarioData.
        /// </summary>
        public static Dictionary<String, ScenarioData> CustomScenarioData = new Dictionary<String, ScenarioData>();
        /// <summary>
        /// Maps custom sins IDs to their respective SinsData.
        /// </summary>
        public static IDictionary<String, SinsData> CustomSinsData = new Dictionary<String, SinsData>();
        /// <summary>
        /// Maps custom trial IDs to their respective TrialData.
        /// </summary>
        public static IDictionary<String, TrialData> CustomTrialData = new Dictionary<String, TrialData>();

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
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate scenario with name: " + scenario.name);
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

        /// <summary>
        /// Register a custom SinsData with the manager.
        /// </summary>
        /// <param name="sinsData">The custom SinsData to register</param>
        public static void RegisterCustomSin(SinsData sinsData)
        {
            if (!CustomSinsData.ContainsKey(sinsData.GetID()))
            {
                CustomSinsData.Add(sinsData.GetID(), sinsData);
                ProviderManager.SaveManager.GetAllGameData().GetAllSinsDatas().Add(sinsData);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate SinsData with name: " + sinsData.name);
            }
        }

        /// <summary>
        /// Get the SinsData corresponding to the given ID
        /// </summary>
        /// <param name="sinsID">ID of the SinsData to get</param>
        /// <returns>The SinsData for the given ID</returns>
        public static SinsData GetSinsDataByID(string sinsID)
        {
            // Search for custom sins matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(sinsID);
            if (CustomSinsData.ContainsKey(guid))
            {
                return CustomSinsData[guid];
            }

            // No custom sins found; search for vanilla sins matching ID
            var vanillaSins = ProviderManager.SaveManager.GetAllGameData().FindEnemySinsData(sinsID);
            if (vanillaSins == null)
            {
                Trainworks.Log(LogLevel.Warning, "Couldn't find SinsData: " + sinsID + " - This will cause crashes.");
            }
            return vanillaSins;
        }

        /// <summary>
        /// Register a custom TrialData with the manager.
        /// </summary>
        /// <param name="trialData">The custom TrialData to register</param>
        public static void RegisterCustomTrial(TrialData trialData)
        {
            if (!CustomTrialData.ContainsKey(trialData.GetID()))
            {
                CustomTrialData.Add(trialData.GetID(), trialData);
                AllGameData allGameData = ProviderManager.SaveManager.GetAllGameData();
                List<TrialData> trials;
                trials = (List<TrialData>)AccessTools.Field(typeof(AllGameData), "trialDatas").GetValue(allGameData);
                trials.Add(trialData);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate TrialData with name: " + trialData.name);
            }
        }

        /// <summary>
        /// Get the TrialData corresponding to the given ID
        /// </summary>
        /// <param name="trialID">ID of the TrialData to get</param>
        /// <returns>The TrialData for the given ID</returns>
        public static TrialData GetTrialDataByID(string trialID)
        {
            // Search for custom trial matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(trialID);
            if (CustomTrialData.ContainsKey(guid))
            {
                return CustomTrialData[guid];
            }

            // No custom trial found; search for vanilla trial matching ID
            var vanillaTrial = ProviderManager.SaveManager.GetAllGameData().FindTrialData(trialID);
            if (vanillaTrial == null)
            {
                Trainworks.Log(LogLevel.Warning, "Couldn't find TrialData: " + trialID + " - This will cause crashes.");
            }
            return vanillaTrial;
        }
    }
}
