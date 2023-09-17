using HarmonyLib;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class DraftRewardDataBuilder : IRewardDataBuilder
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
                    DescriptionKey = DraftRewardID + "DraftRewardData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Name of the reward data
        /// Note if this is set it will set the localization across all languages to this.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the reward data
        /// Note if this is set it will set the localization across all languages to this.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Name title key, this shouldn't need to be set as its set by DraftRewardID.
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Description title key, this shouldn't need to be set as its set by DraftRewardID.
        /// </summary>
        public string DescriptionKey { get; set; }
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
        public bool CanBeSkippedOverride { get; set; }
        public bool ForceContentUnlocked { get; set; }
        public int[] Costs { get; set; }
        public int Crystals { get; set; }
        public bool ShowRewardAnimationInEvent { get; set; }
        public string CollectSFXCueName { get; set; }
        public bool IsServiceMerchantReward { get; set; }
        public bool ShowCancelOverride { get; set; }
        public bool ShowRewardFlowInEvent { get; set; }
        public int MerchantServiceIndex { get; set; }
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + AssetPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Custom asset path to load from relative to the plugin's path
        /// </summary>
        public string AssetPath { get; set; }

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
            CustomRewardManager.RegisterCustomReward(data as GrantableRewardData);
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
            var guid = GUIDGenerator.GenerateDeterministicGUID(DraftRewardID);
            AccessTools.Field(typeof(GameData), "id").SetValue(rewardData, guid);
            rewardData.name = DraftRewardID;


            AccessTools.Field(typeof(RewardData), "costs").SetValue(rewardData, Costs);
            AccessTools.Field(typeof(RewardData), "crystals").SetValue(rewardData, Crystals);
            AccessTools.Field(typeof(RewardData), "ShowRewardAnimationInEvent").SetValue(rewardData, ShowRewardAnimationInEvent);
            AccessTools.Field(typeof(RewardData), "_collectSFXCueName").SetValue(rewardData, CollectSFXCueName);
            AccessTools.Field(typeof(RewardData), "_rewardDescriptionKey").SetValue(rewardData, DescriptionKey);
            AccessTools.Field(typeof(RewardData), "_rewardTitleKey").SetValue(rewardData, NameKey);
            AccessTools.Field(typeof(RewardData), "_showCancelOverride").SetValue(rewardData, ShowCancelOverride);
            AccessTools.Field(typeof(RewardData), "_showRewardFlowInEvent").SetValue(rewardData, ShowRewardFlowInEvent);
            if (AssetPath != null)
            {
                AccessTools.Field(typeof(RewardData), "_rewardSprite").SetValue(rewardData, CustomAssetManager.LoadSpriteFromPath(FullAssetPath));
            }

            AccessTools.Field(typeof(GrantableRewardData), "CanBeSkippedOverride").SetValue(rewardData, CanBeSkippedOverride);
            AccessTools.Field(typeof(GrantableRewardData), "ForceContentUnlocked").SetValue(rewardData, ForceContentUnlocked);
            AccessTools.Field(typeof(GrantableRewardData), "_isServiceMerchantReward").SetValue(rewardData, IsServiceMerchantReward);
            AccessTools.Field(typeof(GrantableRewardData), "_merchantServiceIndex").SetValue(rewardData, MerchantServiceIndex);

            AccessTools.Field(typeof(DraftRewardData), "classDataOverride").SetValue(rewardData, ClassDataOverride);
            AccessTools.Field(typeof(DraftRewardData), "classType").SetValue(rewardData, ClassType);
            AccessTools.Field(typeof(DraftRewardData), "classTypeOverride").SetValue(rewardData, ClassTypeOverride);
            AccessTools.Field(typeof(DraftRewardData), "draftOptionsCount").SetValue(rewardData, DraftOptionsCount);
            AccessTools.Field(typeof(DraftRewardData), "draftPool").SetValue(rewardData, DraftPool);
            AccessTools.Field(typeof(DraftRewardData), "grantSingleCard").SetValue(rewardData, GrantSingleCard);
            AccessTools.Field(typeof(DraftRewardData), "rarityFloorOverride").SetValue(rewardData, RarityFloorOverride);
            AccessTools.Field(typeof(DraftRewardData), "useRunRarityFloors").SetValue(rewardData, UseRunRarityFloors);

            BuilderUtils.ImportStandardLocalization(NameKey, Name);
            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);

            if (register)
            {
                CustomRewardManager.RegisterCustomReward(rewardData);
            }

            return rewardData;
        }
    }
}
