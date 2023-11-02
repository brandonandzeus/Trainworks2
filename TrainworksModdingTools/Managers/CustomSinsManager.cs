using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Utilities;

namespace Trainworks.Managers
{
    /// <summary>
    /// Handles registration and storage of custom SinsData.
    /// </summary>
    public static class CustomSinsManager
    {
        /// <summary>
        /// Maps custom sins IDs to their respective SinsData.
        /// </summary>
        public static IDictionary<string, SinsData> CustomSinsData { get; } = new Dictionary<string, SinsData>();

        /// <summary>
        /// Register a custom SinsDAta with the manager.
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
    }
}
