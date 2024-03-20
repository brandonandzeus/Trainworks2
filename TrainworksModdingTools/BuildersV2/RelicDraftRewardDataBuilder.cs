using HarmonyLib;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class RelicDraftRewardDataBuilder : GrantableRewardDataBuilderBase, IRewardDataBuilder
    {
        private string rewardID;

        /// <summary>
        /// Unique string used to store and retrieve the draft reward data.
        /// Implicitly sets NameKey and DescriptionKey if null
        /// </summary>
        public string RewardID
        {
            get { return RewardID; }
            set
            {
                rewardID = value;
                if (NameKey == null)
                {
                    NameKey = RewardID + "_RelicDraftRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardID + "_RelicDraftRewardData_DescriptionKey";
                }
            }
        }


        public RunState.ClassType ClassType { get; set; }
       
        /// <summary>
        /// Number of cards the banner offers
        /// </summary>
        public uint DraftOptionsCount { get; set; }
        /// <summary>
        /// Relic pool to draft relics from.
        /// </summary>
        public RelicPool DraftPool { get; set; }
        public bool RandomizeOrder { get; set; }

        public RelicDraftRewardDataBuilder()
        {
            DraftOptionsCount = 3;
            RandomizeOrder = true;
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
            if (RewardID == null)
            {
                throw new BuilderException("RewardID is required");
            }

            DraftRewardData rewardData = ScriptableObject.CreateInstance<DraftRewardData>();
            rewardData.name = RewardID;

            Construct(rewardData);

            AccessTools.Field(typeof(DraftRewardData), "classType").SetValue(rewardData, ClassType);
            AccessTools.Field(typeof(DraftRewardData), "draftOptionsCount").SetValue(rewardData, DraftOptionsCount);
            AccessTools.Field(typeof(DraftRewardData), "draftPool").SetValue(rewardData, DraftPool);
            AccessTools.Field(typeof(DraftRewardData), "randomizeOrder").SetValue(rewardData, RandomizeOrder);

            if (register)
            {
                CustomRewardManager.RegisterCustomReward(rewardData);
            }

            return rewardData;
        }
    }
}
