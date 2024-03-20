using HarmonyLib;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.Utilities;

namespace Trainworks.BuildersV2
{
    public abstract class RewardDataBuilderBase
    {
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
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + AssetPath;
        /// <summary>
        /// Set automatically. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }

        private string assetPath;
        /// <summary>
        /// Custom asset path to load from relative to the plugin's path
        /// </summary>
        public string AssetPath
        {
            get { return assetPath; }
            set
            {
                assetPath = value;
                var assembly = Assembly.GetCallingAssembly();
                BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
            }
        }

        public string CollectSFXCueName { get; set; }
        public int[] Costs { get; set; }
        public int Crystals { get; set; }
        public bool ShowRewardFlowInEvent { get; set; }
        public bool ShowRewardAnimationInEvent { get; set; }
        public bool ShowCancelOverride { get; set; }

        public void Construct(RewardData rewardData)
        {
            if (rewardData.name == null)
            {
                throw new BuilderException("No name set for RewardData");
            }

            var guid = GUIDGenerator.GenerateDeterministicGUID(rewardData.name);
            AccessTools.Field(typeof(GameData), "id").SetValue(rewardData, guid);

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

            BuilderUtils.ImportStandardLocalization(NameKey, Name);
            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);
        }
    }
}
