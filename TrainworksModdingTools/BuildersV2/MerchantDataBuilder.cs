using HarmonyLib;
using Malee;
using ShinyShoe;
using System;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Trainworks.BuildersV2
{
    public class MerchantDataBuilder
    {
        private string merchantID;

        /// <summary>
        /// Unique string used to store and retrieve the reward node data.
        /// Implicitly sets NameKey and DescriptionKey if null
        /// </summary>
        public string MerchantID
        {
            get { return merchantID; }
            set
            {
                merchantID = value;
                if (NameKey == null)
                {
                    NameKey = MerchantID + "_MerchantData_TooltipTitleKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = MerchantID + "_MerchantData_TooltipBodyKey";
                }
            }
        }

        /// <summary>
        /// Name for the node.
        /// Note if set, it will set the localization for all languages.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description for the node.
        /// Note if set, it will set the localization for all languages.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Localization key for the node's name.
        /// This should not need to be manually set, as its set by MerchantID
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Localization key for the node's description.
        /// This should not need to be manually set, as its set by MerchantID
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// Sprite used when the node is selected by the controller.
        /// </summary>
        public string ControllerSelectedOutline { get; set; }
        /// <summary>
        /// Sprite used when the node is on the same path but has not been visited.
        /// </summary>
        public string EnabledSpritePath { get; set; }
        /// <summary>
        /// Sprite used when the node is on the same path and has been visited.
        /// </summary>
        public string EnabledVisitedSpritePath { get; set; }
        /// <summary>
        /// Sprite used when the node is on a different path.
        /// </summary>
        public string DisabledSpritePath { get; set; }
        /// <summary>
        /// Sprite used when the node cannot be visited because it already has been.
        /// </summary>
        public string DisabledVisitedSpritePath { get; set; }
        /// <summary>
        /// Sprite used when the node is on a path in a future zone, which is still frozen.
        /// </summary>
        public string FrozenSpritePath { get; set; }
        /// <summary>
        /// Sprite used for the mouseover glow effect. Currently unused.
        /// </summary>
        public string GlowSpritePath { get; set; }
        public string MapIconPath { get; set; }
        public string MinimapIconPath { get; set; }
        /// <summary>
        /// Clickable game object representing the node
        /// </summary>
        public MapNodeIcon MapIconPrefab { get; set; }
        public string NodeSelectedSfxCue { get; set; }
        /// <summary>
        /// The IDs of all map node pools the reward node should be inserted into.
        /// </summary>
        public List<string> MapNodePoolIDs { get; set; }
        public List<MapNodeData> IgnoreIfNodesPresent { get; set; }
        public bool SkipCheckIfFullHealth { get; set; }
        public bool SkipCheckInBattleMode { get; set; }
        public DLC RequiredDLC { get; set; }
        /// <summary>
        /// Determines the Merchant Interface
        /// Gold: For Standard Merchants (MerchantCharacter required)
        /// Crystals: For the Divine Temple interface (Rewards length must be == 3).
        /// </summary>
        public MerchantData.Currency Currency { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public GameObject MerchantScreenContentPrefab { get; set; }
        /// <summary>
        /// Merchant Character, reuse a prexisting MerchantCharacter.
        /// </summary>
        public MerchantCharacterData MerchantCharacter { get; set; }
        /// <summary>
        /// Convienence Builder for MerchantCharacter if set overrides MerchantCharacter
        /// </summary>
        public MerchantCharacterDataBuilder MerchantCharacterBuilder { get; set; }
        /// <summary>
        /// Merchant Goods up for sale. (Reuse prexisitng content).
        /// Note this is not needed for the CardPurge, Reroll, or Duplicate Items. (set AppendStandardRewards to true)
        /// </summary>
        public List<MerchantRewardData> Rewards { get; set; }
        /// <summary>
        /// Convienence Builder for Rewards if set is appended to Rewards.
        /// </summary>
        public List<MerchantRewardDataBuilder> RewardBuilders { get; set; }
        /// <summary>
        /// Appends the Card Purge, Reroll, and Duplicate (w/Mutator) Shop items to the end of the goods offered.
        /// Default true.
        /// </summary>
        public bool AppendStandardRewards { get; set; }
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }

        public MerchantDataBuilder()
        {
            MapNodePoolIDs = new List<string>();
            Rewards = new List<MerchantRewardData>();
            RewardBuilders = new List<MerchantRewardDataBuilder>();
            AppendStandardRewards = true;

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the RewardNodeData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered RewardNodeData</returns>
        public MerchantData BuildAndRegister()
        {
            var merchantData = Build();
            CustomMapNodePoolManager.RegisterCustomMerchant(merchantData, MapNodePoolIDs);
            return merchantData;
        }

        /// <summary>
        /// Builds the MerchantData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created RewardNodeData</returns>
        public MerchantData Build()
        {
            if (MerchantID == null)
            {
                throw new BuilderException("MerchantID is required");
            }

            var merchantData = ScriptableObject.CreateInstance<MerchantData>();
            var guid = GUIDGenerator.GenerateDeterministicGUID(MerchantID);
            AccessTools.Field(typeof(GameData), "id").SetValue(merchantData, guid);
            merchantData.name = MerchantID;

            if (MapIconPrefab == null)
            {
                MakeMapIconPrefab();
            }
            MapNodeData.SkipCheckSettings skipCheckSettings = MapNodeData.SkipCheckSettings.Always;
            if (SkipCheckIfFullHealth)
            {
                skipCheckSettings |= MapNodeData.SkipCheckSettings.IfFullHealth;
            }
            if (SkipCheckInBattleMode)
            {
                skipCheckSettings |= MapNodeData.SkipCheckSettings.InBattleMode;
            }

            var characterData = MerchantCharacter;
            if (MerchantCharacterBuilder != null)
            {
                characterData = MerchantCharacterBuilder.Build();
            }

            MerchantData copyMerchant = (ProviderManager.SaveManager.GetAllGameData().FindMapNodeData(VanillaMapNodeIDs.SpellUpgradeMerchant) as MerchantData);

            Type type = typeof(MerchantData).GetNestedType("MerchantRewardDataList", BindingFlags.NonPublic);
            var rewards = (ReorderableArray<MerchantRewardData>)Activator.CreateInstance(type, true);
            rewards.CopyFrom(Rewards);
            foreach (var rewardBuilder in RewardBuilders)
            {
                rewards.Add(rewardBuilder.Build());
            }
            if (AppendStandardRewards && Currency == MerchantData.Currency.Gold)
            {
                // Card Purge
                rewards.Add(copyMerchant.GetReward(3));
                // Reroll
                rewards.Add(copyMerchant.GetReward(4));
                // Duplication (requires the mutator).
                rewards.Add(copyMerchant.GetReward(5));
            }

            var merchantScreenContentPrefab = MerchantScreenContentPrefab;
            if (merchantScreenContentPrefab != null)
            {
                merchantScreenContentPrefab = copyMerchant.GetContentPrefab();
            }

            AccessTools.Field(typeof(MapNodeData), "ignoreIfNodesPresent").SetValue(merchantData, IgnoreIfNodesPresent);
            AccessTools.Field(typeof(MapNodeData), "mapIcon").SetValue(merchantData, CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + MapIconPath));
            AccessTools.Field(typeof(MapNodeData), "minimapIcon").SetValue(merchantData, CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + MinimapIconPath));
            AccessTools.Field(typeof(MapNodeData), "nodeSelectedSfxCue").SetValue(merchantData, NodeSelectedSfxCue);
            AccessTools.Field(typeof(MapNodeData), "mapIconPrefab").SetValue(merchantData, MapIconPrefab);
            AccessTools.Field(typeof(MapNodeData), "skipCheckSettings").SetValue(merchantData, skipCheckSettings);
            AccessTools.Field(typeof(MapNodeData), "tooltipBodyKey").SetValue(merchantData, DescriptionKey);
            AccessTools.Field(typeof(MapNodeData), "tooltipTitleKey").SetValue(merchantData, NameKey);
            AccessTools.Field(typeof(MapNodeData), "requiredDlc").SetValue(merchantData, RequiredDLC);

            AccessTools.Field(typeof(MerchantData), "currency").SetValue(merchantData, Currency);
            AccessTools.Field(typeof(MerchantData), "merchantScreenContentPrefab").SetValue(merchantData, merchantScreenContentPrefab);
            AccessTools.Field(typeof(MerchantData), "characterData").SetValue(merchantData, characterData);
            AccessTools.Field(typeof(MerchantData), "rewards").SetValue(merchantData, rewards);

            BuilderUtils.ImportStandardLocalization(NameKey, Name);
            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);

            return merchantData;
        }

        private void MakeMapIconPrefab()
        {
            // These are too complicated to create from scratch, so by default we copy from an existing game banner and apply our sprites to it
            MerchantData copy = (ProviderManager.SaveManager.GetAllGameData().FindMapNodeData(VanillaMapNodeIDs.UnitUpgradeMerchant) as MerchantData);
            MapIconPrefab = GameObject.Instantiate(copy.GetMapIconPrefab());
            MapIconPrefab.transform.parent = null;
            MapIconPrefab.name = MerchantID;
            GameObject.DontDestroyOnLoad(MapIconPrefab);
            var images = MapIconPrefab.GetComponentsInChildren<Image>(true);
            List<string> spritePaths = new List<string>
                { // This is the order they're listed on the prefab
                    ControllerSelectedOutline,
                    EnabledSpritePath,
                    EnabledVisitedSpritePath,
                    DisabledVisitedSpritePath,
                    DisabledSpritePath,
                    FrozenSpritePath
                };
            for (int i = 0; i < images.Length; i++)
            { // This method of modifying the image's sprite has the unfortunate side-effect of removing the white mouse-over outline
                var sprite = CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + spritePaths[i]);
                if (sprite != null)
                {
                    images[i].sprite = sprite;
                    images[i].material = null;
                }
            }
        }
    }
}
