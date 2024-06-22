using HarmonyLib;
using System.Reflection;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class DraftRewardDataBuilder : GrantableRewardDataBuilderBase, IRewardDataBuilder
    {
        private string draftRewardID;

        /// <summary>
        /// Unique string used to store and retrieve the draft reward data.
        /// Implicitly sets NameKey and DescriptionKey if null
        /// </summary>
        public string DraftRewardID
        {
            get { return draftRewardID; }
            set
            {
                draftRewardID = value;
                if (NameKey == null)
                {
                    NameKey = DraftRewardID + "_DraftRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = DraftRewardID + "_DraftRewardData_DescriptionKey";
                }
            }
        }


        public ClassData ClassDataOverride { get; set; }
        public RunState.ClassType ClassType { get; set; }
        public bool ClassTypeOverride { get; set; }
        /// <summary>
        /// Number of cards the banner offers
        /// </summary>
        public uint DraftOptionsCount { get; set; }
        /// <summary>
        /// Card pool the banner pulls from
        /// </summary>
        public CardPool DraftPool { get; set; }
        public bool GrantSingleCard { get; set; }
        public CollectableRarity RarityFloorOverride { get; set; }
        public bool UseRunRarityFloors { get; set; }

        public DraftRewardDataBuilder()
        {
            Costs = System.Array.Empty<int>();
            DraftOptionsCount = 3;
            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        public RewardData BuildAndRegister()
        {
            RewardData data = Build(false);
            CustomRewardManager.RegisterCustomReward(data);
            return data;
        }

        /// <summary>
        /// Builds the RewardData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created RewardData</returns>
        public RewardData Build(bool register = true)
        {
            if (DraftRewardID == null)
            {
                throw new BuilderException("DraftRewardID is required");
            }

            DraftRewardData rewardData = ScriptableObject.CreateInstance<DraftRewardData>();
            rewardData.name = DraftRewardID;

            Construct(rewardData);

            AccessTools.Field(typeof(DraftRewardData), "classDataOverride").SetValue(rewardData, ClassDataOverride);
            AccessTools.Field(typeof(DraftRewardData), "classType").SetValue(rewardData, ClassType);
            AccessTools.Field(typeof(DraftRewardData), "classTypeOverride").SetValue(rewardData, ClassTypeOverride);
            AccessTools.Field(typeof(DraftRewardData), "draftOptionsCount").SetValue(rewardData, DraftOptionsCount);
            AccessTools.Field(typeof(DraftRewardData), "draftPool").SetValue(rewardData, DraftPool);
            AccessTools.Field(typeof(DraftRewardData), "grantSingleCard").SetValue(rewardData, GrantSingleCard);
            AccessTools.Field(typeof(DraftRewardData), "rarityFloorOverride").SetValue(rewardData, RarityFloorOverride);
            AccessTools.Field(typeof(DraftRewardData), "useRunRarityFloors").SetValue(rewardData, UseRunRarityFloors);

            if (register)
            {
                CustomRewardManager.RegisterCustomReward(rewardData);
            }

            return rewardData;
        }
    }
}
