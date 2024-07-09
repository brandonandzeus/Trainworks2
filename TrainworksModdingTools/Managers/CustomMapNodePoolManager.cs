using HarmonyLib;
using Malee;
using System.Collections.Generic;
using Trainworks.ConstantsV2;

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
        internal static RandomMapDataContainer RandomChosenMainClassUnit;
        internal static RandomMapDataContainer RandomChosenSubClassUnit;

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
                AddCustomMapNodesToBucket(mapNodePoolID, rewardNodeData);
            }
        }

        /// <summary>
        /// Registers a custom merchant
        /// </summary>
        /// <param name="merchantData">MerchantData</param>
        /// <param name="mapNodePoolIDs">Possible map nodes to add the merchant to.</param>
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

        public static void AddCustomMapNodesToBucket(string mapNodePoolID, MapNodeData data)
        {
            // Banner Pools have to be handled separately since they are setup differently.
            if (mapNodePoolID == VanillaMapNodePoolIDs.RandomChosenMainClassUnit)
            {
                AddRewardNodeToMainClassBanner(data);
            }
            else if (mapNodePoolID == VanillaMapNodePoolIDs.RandomChosenSubClassUnit)
            {
                AddRewardNodeToSubClassBanner(data);
            }
            else
            {
                var bucketList = FindMapNodeBucketList(mapNodePoolID);
                if (bucketList == null)
                {
                    Trainworks.Log("Could not find MapNodeBucketContainer with id " + mapNodePoolID + " ignoring.");
                    return;
                }
                var containerList = (ReorderableArray<MapNodeBucketData>)AccessTools.Field(typeof(MapNodeBucketContainer), "mapNodeBucketContainerList").GetValue(bucketList);
                containerList[0].MapNodes.Add(data);
            }
        }

        private static MapNodeBucketContainer FindMapNodeBucketList(string id)
        {
            var runData = ProviderManager.SaveManager.GetAllGameData().GetBalanceData().GetRunData(false);
            var mnbl = (ReorderableArray<MapNodeBucketList>)AccessTools.Field(typeof(RunData), "mapNodeBucketLists").GetValue(runData);

            foreach (var bucketList in mnbl[0].BucketList)
            {
                var bucketId = (string)AccessTools.Field(typeof(MapNodeBucketContainer), "id").GetValue(bucketList);
                if (bucketId == id)
                {
                    return bucketList;
                }
            }
            return null;
        }

        internal static void AddRewardNodeToMainClassBanner(MapNodeData data)
        {
            if (RandomChosenMainClassUnit == null)
            {
                var mnbl = FindMapNodeBucketList(VanillaMapNodePoolIDs.LimboBannerMain);
                var containerList = (ReorderableArray<MapNodeBucketData>)AccessTools.Field(typeof(MapNodeBucketContainer), "mapNodeBucketContainerList").GetValue(mnbl);
                RandomChosenMainClassUnit = containerList[0].MapNodes[0] as RandomMapDataContainer;
            }

            var dataList = (ReorderableArray<MapNodeData>)AccessTools.Field(typeof(RandomMapDataContainer), "mapNodeDataList").GetValue(RandomChosenMainClassUnit);
            dataList.Add(data);
        }

        internal static void AddRewardNodeToSubClassBanner(MapNodeData data)
        {
            if (RandomChosenSubClassUnit == null)
            {
                var mnbl = FindMapNodeBucketList(VanillaMapNodePoolIDs.LimboBannerSub);
                var containerList = (ReorderableArray<MapNodeBucketData>)AccessTools.Field(typeof(MapNodeBucketContainer), "mapNodeBucketContainerList").GetValue(mnbl);
                RandomChosenSubClassUnit = containerList[0].MapNodes[0] as RandomMapDataContainer;
            }

            var dataList = (ReorderableArray<MapNodeData>)AccessTools.Field(typeof(RandomMapDataContainer), "mapNodeDataList").GetValue(RandomChosenSubClassUnit);
            dataList.Add(data);
        }
    }
}
