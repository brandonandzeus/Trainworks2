using BepInEx.Logging;
using System.Collections.Generic;
using Trainworks.Managers;
using Trainworks.Utilities;

namespace Trainworks.ManagersV2
{
    /// <summary>
    /// Handles registration and storage of custom mutator data.
    /// </summary>
    public static class CustomMutatorManager
    {
        /// <summary>
        /// Maps custom mutator IDs to their respective MutatorData.
        /// </summary>
        public static IDictionary<string, MutatorData> CustomMutatorData { get; } = new Dictionary<string, MutatorData>();

        /// <summary>
        /// Register a custom mutator with the manager, allowing it to show up in game.
        /// </summary>
        /// <param name="mutatorData">The custom mutator data to register</param>
        public static void RegisterCustomMutator(MutatorData mutatorData)
        {
            if (!CustomMutatorData.ContainsKey(mutatorData.GetID()))
            {
                CustomMutatorData.Add(mutatorData.GetID(), mutatorData);
                ProviderManager.SaveManager.GetAllGameData().GetAllMutatorData().Add(mutatorData);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate mutator data with name: " + mutatorData.name);
            }
        }

        /// <summary>
        /// Get the mutator data corresponding to the given ID
        /// </summary>
        /// <param name="mutatorID">ID of the mutator to get</param>
        /// <returns>The mutator data for the given ID</returns>
        public static MutatorData GetMutatorDataByID(string mutatorID)
        {
            // Search for custom mutator matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(mutatorID);
            if (CustomMutatorData.ContainsKey(guid))
            {
                return CustomMutatorData[guid];
            }

            // No custom mutator found; search for vanilla mutator matching ID
            var vanillaMutator = ProviderManager.SaveManager.GetAllGameData().FindMutatorData(mutatorID);
            if (vanillaMutator == null)
            {
                Trainworks.Log(LogLevel.Warning, "Couldn't find mutator: " + mutatorID + " - This will cause crashes.");
            }
            return vanillaMutator;
        }
    }
}