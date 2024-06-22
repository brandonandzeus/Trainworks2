using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Trainworks.BuildersV2
{
    public class RewardNodeDataBuilder
    {
        private string rewardNodeID;

        /// <summary>
        /// Unique string used to store and retrieve the reward node data.
        /// Implicitly sets NameKey and DescriptionKey if null
        /// </summary>
        public string RewardNodeID
        {
            get { return rewardNodeID; }
            set
            {
                rewardNodeID = value;
                if (NameKey == null)
                {
                    NameKey = RewardNodeID + "_RewardNodeData_TooltipTitleKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardNodeID + "_RewardNodeData_TooltipBodyKey";
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
        /// This should not need to be manually set, as its set by RewardNodeID
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Localization key for the node's description.
        /// This should not need to be manually set, as its set by RewardNodeID
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
        public bool GrantImmediately { get; set; }
        public bool OverrideTooltipTitleBody { get; set; }
        /// <summary>
        /// Clan ID required for this reward.
        /// </summary>
        public string RequiredClassID { get; set; }
        public List<IRewardDataBuilder> RewardBuilders { get; set; }
        public List<RewardData> Rewards { get; set; }
        public List<MapNodeData> IgnoreIfNodesPresent { get; set; }
        public bool SkipCheckIfFullHealth { get; set; }
        public bool SkipCheckInBattleMode { get; set; }
        public bool UseFormattedOverrideTooltipTitle { get; set; }
        public int DlcHellforgedCrystalsCost { get; set; }
        public ShinyShoe.DLC RequiredDLC { get; set; }
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }

        public RewardNodeDataBuilder()
        {
            MapNodePoolIDs = new List<string>();
            Rewards = new List<RewardData>();
            RewardBuilders = new List<IRewardDataBuilder>();

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the RewardNodeData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered RewardNodeData</returns>
        public RewardNodeData BuildAndRegister()
        {
            var rewardNodeData = Build();
            CustomMapNodePoolManager.RegisterCustomRewardNode(rewardNodeData, MapNodePoolIDs);
            return rewardNodeData;
        }

        /// <summary>
        /// Builds the RewardNodeData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created RewardNodeData</returns>
        public RewardNodeData Build()
        {
            if (RewardNodeID == null)
            {
                throw new BuilderException("RewardNodeID is required");
            }

            RewardNodeData rewardNodeData = ScriptableObject.CreateInstance<RewardNodeData>();
            var guid = GUIDGenerator.GenerateDeterministicGUID(RewardNodeID);
            AccessTools.Field(typeof(GameData), "id").SetValue(rewardNodeData, guid);
            rewardNodeData.name = RewardNodeID;

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

            AccessTools.Field(typeof(MapNodeData), "ignoreIfNodesPresent").SetValue(rewardNodeData, IgnoreIfNodesPresent);
            AccessTools.Field(typeof(MapNodeData), "mapIcon").SetValue(rewardNodeData, CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + MapIconPath));
            AccessTools.Field(typeof(MapNodeData), "minimapIcon").SetValue(rewardNodeData, CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + MinimapIconPath));
            AccessTools.Field(typeof(MapNodeData), "nodeSelectedSfxCue").SetValue(rewardNodeData, NodeSelectedSfxCue);
            AccessTools.Field(typeof(MapNodeData), "mapIconPrefab").SetValue(rewardNodeData, MapIconPrefab);
            AccessTools.Field(typeof(MapNodeData), "skipCheckSettings").SetValue(rewardNodeData, skipCheckSettings);
            AccessTools.Field(typeof(MapNodeData), "tooltipBodyKey").SetValue(rewardNodeData, DescriptionKey);
            AccessTools.Field(typeof(MapNodeData), "tooltipTitleKey").SetValue(rewardNodeData, NameKey);
            AccessTools.Field(typeof(MapNodeData), "requiredDlc").SetValue(rewardNodeData, RequiredDLC);
            AccessTools.Field(typeof(RewardNodeData), "grantImmediately").SetValue(rewardNodeData, GrantImmediately);
            AccessTools.Field(typeof(RewardNodeData), "OverrideTooltipTitleBody").SetValue(rewardNodeData, OverrideTooltipTitleBody);
            AccessTools.Field(typeof(RewardNodeData), "UseFormattedOverrideTooltipTitle").SetValue(rewardNodeData, UseFormattedOverrideTooltipTitle);
            ClassData linkedClass = null;
            if (RequiredClassID != null)
            {
                linkedClass = CustomClassManager.GetClassDataByID(RequiredClassID);
            }
            AccessTools.Field(typeof(RewardNodeData), "requiredClass").SetValue(rewardNodeData, linkedClass);
            AccessTools.Field(typeof(RewardNodeData), "dlcHellforgedCrystalsCost").SetValue(rewardNodeData, DlcHellforgedCrystalsCost);

            // Field is not allocated at initialization.
            List<RewardData> rewards = new List<RewardData>();
            rewards.AddRange(Rewards);
            foreach (var builder in RewardBuilders)
            {
                rewards.Add(builder.Build());
            }
            AccessTools.Field(typeof(RewardNodeData), "rewards").SetValue(rewardNodeData, rewards);

            BuilderUtils.ImportStandardLocalization(NameKey, Name);
            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);

            return rewardNodeData;
        }

        private void MakeMapIconPrefab()
        {
            // These are too complicated to create from scratch, so by default we copy from an existing game banner and apply our sprites to it
            RewardNodeData copyBanner = (ProviderManager.SaveManager.GetAllGameData().FindMapNodeData(VanillaMapNodeIDs.RewardNodeUnitPackStygian) as RewardNodeData);
            MapIconPrefab = GameObject.Instantiate(copyBanner.GetMapIconPrefab());
            MapIconPrefab.transform.parent = null;
            MapIconPrefab.name = RewardNodeID;
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
