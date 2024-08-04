using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using Trainworks.Utilities;

namespace Trainworks.ManagersV2
{
    public static class CustomEnhancerManager
    {
        /// <summary>
        /// Enhancer pool to add the EnhancerData of Enhancers which can appear.
        /// </summary>
        private static Dictionary<string, EnhancerPool> CustomEnhancerPools { get; } = new Dictionary<string, EnhancerPool>();

        /// <summary>
        /// Custom Enhancers.
        /// </summary>
        private static Dictionary<string, EnhancerData> CustomEnhancers = new Dictionary<string, EnhancerData>();

        /// <summary>
        /// Cache to speed up finding Enhancer Pools.
        /// </summary>
        private static Dictionary<string, EnhancerPool> VanillaEnhancerPoolCache = new Dictionary<string, EnhancerPool>();

        // Used to find Vanilla Enhancers. These aren't directly stored in AllGameData, they can be found in the MerchantData.
        private static readonly List<string> MerchantIDs = new List<string>
        {
            VanillaMapNodeIDs.UnitUpgradeMerchant,
            VanillaMapNodeIDs.SpellUpgradeMerchant,
            VanillaMapNodeIDs.DarkPactTempleMerchant
        };

        /// <summary>
        /// Cache for the ExcludedStatusEffects field in Stackstone.
        /// </summary>
        private static List<StatusEffectStackData> StackstoneExcludedStatusEffects = null;

        /// <summary>
        /// Add the enhancer to the pool.
        /// </summary>
        /// <param name="enhancerData">EnhancerData to be added to the pool</param>
        /// <param name="enhancerPoolID">Name of the Enhancer pool to add to</param>
        public static void AddEnhancerToPool(EnhancerData enhancerData, string enhancerPoolID)
        {
            EnhancerPool enhancerPool = null;
            if (CustomEnhancerPools.ContainsKey(enhancerPoolID))
            {
                enhancerPool = CustomEnhancerPools[enhancerPoolID];
            }
            else
            {
                enhancerPool = GetVanillaEnhancerPool(enhancerPoolID);
            }

            if (enhancerPool == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not find EnhancerPool wtih id " + enhancerPoolID + " ignoring adding enhancer: " + enhancerData.name + " to this pool.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
                return;
            }

            var enhancers = (Malee.ReorderableArray<EnhancerData>)AccessTools.Field(typeof(EnhancerPool), "relicDataList").GetValue(enhancerPool);
            enhancers.Add(enhancerData);
        }

        public static void RegisterEnhancer(EnhancerData enhancer, List<string> enhancerPoolIDs)
        {
            if (!CustomEnhancers.ContainsKey(enhancer.GetID()))
            {
                CustomEnhancers.Add(enhancer.GetID(), enhancer);
                foreach (var id in enhancerPoolIDs)
                {
                    AddEnhancerToPool(enhancer, id);
                }
                ProviderManager.SaveManager.GetAllGameData().GetAllEnhancerData().Add(enhancer);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate enhancer data with name: " + enhancer.name);
            }
        }

        public static void RegisterEnhancerPool(EnhancerPool pool)
        {
            if (!CustomEnhancerPools.ContainsKey(pool.name))
            {
                CustomEnhancerPools.Add(pool.name, pool);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate enhancer pool with name: " + pool.name);
            }
        }

        public static EnhancerData GetEnhancerByID(string enhancerID)
        {
            var guid = GUIDGenerator.GenerateDeterministicGUID(enhancerID);

            if (CustomEnhancers.ContainsKey(guid))
            {
                return CustomEnhancers[guid];
            }

            var vanillaEnhancer = ProviderManager.SaveManager.GetAllGameData().FindEnhancerData(enhancerID);
            if (vanillaEnhancer == null)
            {
                Trainworks.Log(LogLevel.Error, "Couldn't find enhancer: " + enhancerID + " - This will cause crashes.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            }

            return vanillaEnhancer;
        }

        private static EnhancerPool GetVanillaEnhancerPool(string enhancerID)
        {
            if (VanillaEnhancerPoolCache.ContainsKey(enhancerID))
            {
                return VanillaEnhancerPoolCache[enhancerID];
            }

            var allGameData = ProviderManager.SaveManager.GetAllGameData();
            EnhancerPool enhancerPool = null;

            if (enhancerID == VanillaEnhancerPoolIDs.DraftUpgradePool)
            {
                CollectableRelicData capriciousReflection = CustomCollectableRelicManager.GetRelicDataByID(VanillaRelicIDs.CapriciousReflection);
                var effect = capriciousReflection?.GetFirstRelicEffectData<RelicEffectAddStartingUpgradeToCardDrafts>();
                enhancerPool = effect?.GetParamEnhancerPool();
            }
            else if (enhancerID == VanillaEnhancerPoolIDs.WurmkinInfusedPool)
            {
                var classData = allGameData.FindClassData(VanillaClanIDs.Wurmkin);
                enhancerPool = classData.GetRandomDraftEnhancerPool();
            }
            else
            {
                foreach (var merchantID in MerchantIDs)
                {
                    var mapNode = allGameData.FindMapNodeData(merchantID);
                    if (mapNode is MerchantData merchant)
                    {
                        for (int i = 0; i < merchant.GetNumRewards(); i++)
                        {
                            RewardData reward = merchant.GetReward(i).RewardData;
                            if (reward is EnhancerPoolRewardData enhancerPoolReward)
                            {
                                var foundEnhancerPool = (EnhancerPool)AccessTools.Field(typeof(EnhancerPoolRewardData), "relicPool").GetValue(enhancerPoolReward);
                                if (foundEnhancerPool.name == enhancerID)
                                {
                                    enhancerPool = foundEnhancerPool;
                                    break;
                                }
                            }
                        }
                    }
                    if (enhancerPool != null)
                        break;
                }
            }

            if (enhancerPool != null) 
            {
                VanillaEnhancerPoolCache.Add(enhancerID, enhancerPool);
            }

            return enhancerPool;
        }

        /// <summary>
        /// Excludes a status effect from Doublestack. You generally want to do this if the status effect is nonstackable.
        /// </summary>
        /// <param name="statusId">Status Effect ID to exclude from Doublestack</param>
        public static void ExcludeStatusEffectFromDoublestack(string statusId)
        {
            if (StackstoneExcludedStatusEffects == null)
            {
                var upgrade = CustomUpgradeManager.GetCardUpgradeByID(VanillaCardUpgradeDataIDs.Stackstone);

                CardUpgradeMaskData nonstackable = null;
                foreach (var filter in upgrade.GetFilters())
                {
                    if (filter.GetName() == "ExcludeNonstackableStatusEffects")
                    {
                        nonstackable = filter;
                        break;
                    }
                }

                if (nonstackable == null)
                {
                    Trainworks.Log(LogLevel.Error, "Could not find ExcludeNonstackableStatusEffects in Stackstone.");
                    return;
                }

                StackstoneExcludedStatusEffects = (List<StatusEffectStackData>)AccessTools.Field(typeof(CardUpgradeMaskData), "excludedStatusEffects").GetValue(nonstackable);
            }

            StackstoneExcludedStatusEffects?.Add(new StatusEffectStackData { statusId = statusId, count = 0 });
        }
    }
}
