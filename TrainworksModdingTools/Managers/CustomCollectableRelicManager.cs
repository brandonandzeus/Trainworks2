using BepInEx.Logging;
using HarmonyLib;
using Malee;
using System;
using System.Collections.Generic;
using Trainworks.ConstantsV2;
using Trainworks.Utilities;

namespace Trainworks.Managers
{
    /// <summary>
    /// Handles registration and storage of custom relic data.
    /// </summary>
    public class CustomCollectableRelicManager
    {
        /// <summary>
        /// Custom RelicPools
        /// </summary>
        private static Dictionary<string, RelicPool> CustomRelicPools { get; } = new Dictionary<string, RelicPool>();

        /// <summary>
        /// Maps custom relic IDs to their respective RelicData.
        /// </summary>
        public static IDictionary<string, CollectableRelicData> CustomRelicData { get; } = new Dictionary<string, CollectableRelicData>();

        /// <summary>
        /// Cache to speed up finding Relic Pools.
        /// </summary>
        private static readonly Dictionary<string, RelicPool> VanillaRelicPoolCache = new Dictionary<string, RelicPool>();

        /// <summary>
        /// Add the relic to the RelicPool.
        /// </summary>
        /// <param name="data">CollectableRelicData to be added to the pool</param>
        /// <param name="poolID">Name of the RelicPool to add to</param>
        public static void AddRelicToPool(CollectableRelicData data, string poolID)
        {
            RelicPool relicPool = null;
            if (CustomRelicPools.ContainsKey(poolID))
            {
                relicPool = CustomRelicPools[poolID];
            }
            else
            {
                relicPool = GetVanillaRelicPool(poolID);
            }

            if (relicPool == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not find RelicPool wtih id " + poolID + " ignoring adding collectable relic: " + data.name + " to this pool.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
                return;
            }

            var list = (Malee.ReorderableArray<CollectableRelicData>) AccessTools.Field(typeof(RelicPool), "relicDataList").GetValue(relicPool);
            list.Add(data);
        }

        /// <summary>
        /// Register a custom relic with the manager, allowing it to show up in game.
        /// </summary>
        /// <param name="relicData">The custom relic data to register</param>
        /// <param name="relicPoolData">The pools to insert the custom relic data into</param>
        public static void RegisterCustomRelic(CollectableRelicData relicData, List<string> relicPoolData)
        {
            if (!CustomRelicData.ContainsKey(relicData.GetID()))
            {
                CustomRelicData.Add(relicData.GetID(), relicData);
                foreach (var id in relicPoolData)
                {
                    AddRelicToPool(relicData, id);
                }
                ProviderManager.SaveManager.GetAllGameData().GetAllCollectableRelicData().Add(relicData);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate relic data with name: " + relicData.name);
            }
        }

        /// <summary>
        /// Register a custom relic pool with the manager.
        /// </summary>
        /// <param name="pool"></param>
        public static void RegisterRelicPool(RelicPool pool)
        {
            if (!CustomRelicPools.ContainsKey(pool.name))
            {
                CustomRelicPools.Add(pool.name, pool);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate relic pool with name: " + pool.name);
            }
        }

        /// <summary>
        /// Get the relic data corresponding to the given ID
        /// </summary>
        /// <param name="relicID">ID of the relic to get</param>
        /// <returns>The relic data for the given ID</returns>
        public static CollectableRelicData GetRelicDataByID(string relicID)
        {
            // Search for custom relic matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(relicID);
            if (CustomRelicData.ContainsKey(guid))
            {
                return CustomRelicData[guid];
            }

            // No custom relic found; search for vanilla relic matching ID
            var vanillaRelic = ProviderManager.SaveManager.GetAllGameData().FindCollectableRelicData(relicID);
            if (vanillaRelic == null)
            {
                Trainworks.Log(LogLevel.Error, "Couldn't find relic: " + relicID + " - This will cause crashes.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            }
            return vanillaRelic;
        }

        public static RelicPool GetVanillaRelicPool(string id)
        {
            if (VanillaRelicPoolCache.ContainsKey(id))
            {
                return VanillaRelicPoolCache[id];
            }

            RelicPool pool = null;
            if (id == VanillaRelicPoolIDs.MegaRelicPool)
                pool = GetMegaPool();
            else if (id == VanillaRelicPoolIDs.BossPool)
                pool = GetBossPool();
            else if (id == VanillaRelicPoolIDs.BossPoolLevel6)
                pool = GetLevel6BossPool();
            else if (id == VanillaRelicPoolIDs.DivineRelicPoolA)
                pool = GetDivineArtifactPoolA();
            else if (id == VanillaRelicPoolIDs.DivineRelicPoolB)
                pool = GetDivineArtifactPoolB();
            else if (id == VanillaRelicPoolIDs.DivineRelicPoolC)
                pool = GetDivineArtifactPoolC();

            if (pool != null)
            {
                VanillaRelicPoolCache[id] = pool;
                return pool;
            }
            return null;
        }

        internal static RelicPool GetMegaPool()
        {
            var reward = ProviderManager.SaveManager.GetAllGameData().FindRewardData(VanillaRewardIDs.BlessingDraftReward) as RelicDraftRewardData;
            if (reward == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not get MegaPool Instance");
                return null;
            }
            return AccessTools.Field(typeof(RelicDraftRewardData), "draftPool").GetValue(reward) as RelicPool;
        }

        internal static RelicPool GetBossPool()
        {
            var reward = ProviderManager.SaveManager.GetAllGameData().FindRewardData(VanillaRewardIDs.BlessingDraftBoss) as RelicDraftRewardData;
            if (reward == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not get BossPool Instance");
                return null;
            }
            return AccessTools.Field(typeof(RelicDraftRewardData), "draftPool").GetValue(reward) as RelicPool;
        }

        internal static RelicPool GetLevel6BossPool()
        {
            var reward = ProviderManager.SaveManager.GetAllGameData().FindRewardData(VanillaRewardIDs.BlessingDraftBossLevel6) as RelicDraftRewardData;
            if (reward == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not get BossPoolLevel6 Instance");
                return null;
            }
            return AccessTools.Field(typeof(RelicDraftRewardData), "draftPool").GetValue(reward) as RelicPool;
        }

        internal static RelicPool GetDivineArtifactPoolA()
        {
            var reward = ProviderManager.SaveManager.GetAllGameData().FindRewardData(VanillaRewardIDs.ShardEvent_BlessingDraftReward_1);
            if (reward == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not get DivineArtifactPoolA Instance");
                return null;
            }
            return AccessTools.Field(typeof(RelicDraftRewardData), "draftPool").GetValue(reward) as RelicPool;
        }

        internal static RelicPool GetDivineArtifactPoolB()
        {
            var reward = ProviderManager.SaveManager.GetAllGameData().FindRewardData(VanillaRewardIDs.ShardEvent_BlessingDraftReward_2);
            if (reward == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not get DivineArtifactPoolB Instance");
                return null;
            }
            return AccessTools.Field(typeof(RelicDraftRewardData), "draftPool").GetValue(reward) as RelicPool;
        }

        internal static RelicPool GetDivineArtifactPoolC()
        {
            var reward = ProviderManager.SaveManager.GetAllGameData().FindRewardData(VanillaRewardIDs.ShardEvent_BlessingDraftReward_3);
            if (reward == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not get DivineArtifactPoolC Instance");
                return null;
            }
            return AccessTools.Field(typeof(RelicDraftRewardData), "draftPool").GetValue(reward) as RelicPool;
        }
    }
}
