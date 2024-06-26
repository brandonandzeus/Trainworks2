﻿using HarmonyLib;
using Malee;
using System.Collections.Generic;

namespace Trainworks.Managers
{
    public class CustomMapNodePoolManager
    {
        /// <summary>
        /// Maps map node pool IDs to the RewardNodeData of reward nodes which can appear in them.
        /// Reward nodes which naturally appear in the pool in the base game will not appear in these lists.
        /// </summary>
        public static IDictionary<string, List<RewardNodeData>> CustomRewardNodeData { get; } = new Dictionary<string, List<RewardNodeData>>();
        public static IDictionary<string, List<MapNodeData>> CustomMapNodeData { get; } = new Dictionary<string, List<MapNodeData>>();

        /// <summary>
        /// Add the map node to the map node pools with given IDs.
        /// </summary>
        /// <param name="rewardNodeData">RewardNodeData to be added to the pools</param>
        /// <param name="mapNodePoolIDs">List of map node pool IDs to add the reward node to</param>
        public static void RegisterCustomRewardNode(RewardNodeData rewardNodeData, List<string> mapNodePoolIDs)
        {
            var mapNodeDatas = (List<MapNodeData>)AccessTools.Field(typeof(AllGameData), "mapNodeDatas").GetValue(ProviderManager.SaveManager.GetAllGameData());
            mapNodeDatas.Add(rewardNodeData);
            foreach (string mapNodePoolID in mapNodePoolIDs)
            {
                if (!CustomRewardNodeData.ContainsKey(mapNodePoolID))
                {
                    CustomRewardNodeData[mapNodePoolID] = new List<RewardNodeData>();
                }
                CustomRewardNodeData[mapNodePoolID].Add(rewardNodeData);
            }
        }

        public static void RegisterCustomMerchant(MerchantData merchantData, List<string> mapNodePoolIDs)
        {
            var mapNodeDatas = (List<MapNodeData>)AccessTools.Field(typeof(AllGameData), "mapNodeDatas").GetValue(ProviderManager.SaveManager.GetAllGameData());
            mapNodeDatas.Add(merchantData);
            foreach (string mapNodePoolID in mapNodePoolIDs)
            {
                if (!CustomMapNodeData.ContainsKey(mapNodePoolID))
                {
                    CustomMapNodeData[mapNodePoolID] = new List<MapNodeData>();
                }
                CustomMapNodeData[mapNodePoolID].Add(merchantData);
                AddCustomMapNodesToBucket(mapNodePoolID, merchantData);
            }
        }

        /// <summary>
        /// Gets a list of all reward nodes added to the given map node pool by mods,
        /// then adds them to the list passed in.
        /// Reward nodes which naturally appear in the pool will not be returned.
        /// </summary>
        /// <param name="mapNodePoolID">ID of the map node pool to get nodes for</param>
        /// <param name="mapNodes">List of map nodes to add the nodes to</param>
        /// <param name="classTypeOverride">Whether or not only nodes matching the main/subclass should be added</param>
        /// <returns>A list of reward nodes added to the map node pool with given ID by mods</returns>
        public static void AddRewardNodesForPool(string mapNodePoolID, List<MapNodeData> mapNodes, RunState.ClassType classTypeOverride)
        {
            if (CustomRewardNodeData.ContainsKey(mapNodePoolID))
            {
                var validMapNodes = new List<MapNodeData>();
                var possibleMapNodes = CustomRewardNodeData[mapNodePoolID];

                foreach (MapNodeData mapNodeData in possibleMapNodes)
                {
                    RewardNodeData rewardNodeData;
                    if ((rewardNodeData = (mapNodeData as RewardNodeData)) != null && rewardNodeData.RequiredClass != null)
                    {
                        if (classTypeOverride == RunState.ClassType.MainClass && ProviderManager.SaveManager.GetMainClass() != rewardNodeData.RequiredClass)
                        {
                            continue;
                        }
                        else if (classTypeOverride == RunState.ClassType.SubClass && ProviderManager.SaveManager.GetSubClass() != rewardNodeData.RequiredClass)
                        {
                            continue;
                        }
                    }
                    validMapNodes.Add(mapNodeData);
                }

                mapNodes.AddRange(validMapNodes);
            }
        }

        public static void AddCustomMapNodesToBucket(string mapNodePoolID, MapNodeData data)
        {
            var runData = ProviderManager.SaveManager.GetAllGameData().GetBalanceData().GetRunData(false);
            var mnbl = (ReorderableArray<MapNodeBucketList>)AccessTools.Field(typeof(RunData), "mapNodeBucketLists").GetValue(runData);

            foreach (var bucketList in mnbl[0].BucketList)
            {
                var id = (string)AccessTools.Field(typeof(MapNodeBucketContainer), "id").GetValue(bucketList);
                if (id == mapNodePoolID)
                {
                    var containerList = (ReorderableArray<MapNodeBucketData>)AccessTools.Field(typeof(MapNodeBucketContainer), "mapNodeBucketContainerList").GetValue(bucketList);
                    containerList[0].MapNodes.Add(data);
                }
            }
        }
    }
}
