using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class CollectableRelicDataBuilder
    {
        private string collectableRelicID;

        /// <summary>
        /// Unique string used to store and retrieve the relic data.
        /// Implicitly sets NameKey and DescriptionKey.
        /// </summary>
        public string CollectableRelicID
        {
            get { return collectableRelicID; }
            set
            {
                collectableRelicID = value;
                if (NameKey == null)
                {
                    NameKey = collectableRelicID + "_CollectableRelicData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = collectableRelicID + "_CollectableRelicData_DescriptionKey";
                }
            }
        }
        /// <summary>
        /// The IDs of all relic pools the relic should be inserted into.
        /// </summary>
        public List<string> RelicPoolIDs { get; set; }

        /// <summary>
        /// Name displayed for the relic.
        /// Note that setting this field will set the localization for all langauges.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Localization key for the relic's name.
        /// Automatically set by CollectableRelicID property
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Description displayed for the relic.
        /// Note that setting this field will set the localization for all langauges.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Localization key for the relic's description.
        /// Automatically set by CollectableRelicID property
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// ID of the clan the card is a part of. Leave null for clanless.
        /// Base game clan IDs should be retrieved via helper class "VanillaClanIDs".
        /// </summary>
        public string ClanID { get; set; }
        public List<RelicEffectDataBuilder> EffectBuilders { get; set; }
        public List<RelicEffectData> Effects { get; set; }
        public CollectableRarity Rarity { get; set; }
        public int UnlockLevel { get; set; }
        public string RelicActivatedKey { get; set; }
        public List<string> RelicLoreTooltipKeys { get; set; }
        public bool FromStoryEvent { get; set; }
        public bool IsBossGivenRelic { get; set; }
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + IconPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Custom asset path to load relic art from.
        /// </summary>
        public string IconPath { get; set; }
        /// <summary>
        /// Is this collectible relic a divine variant.
        /// </summary>
        public bool DivineVariant { get; set; }
        public RelicData.RelicLoreTooltipStyle RelicLoreTooltipStyle { get; set; }
        public bool IgnoreForNoRelicAchievement { get; set; }
        public ShinyShoe.DLC RequiredDLC { get; set; }

        public CollectableRelicDataBuilder()
        {
            Effects = new List<RelicEffectData>();
            EffectBuilders = new List<RelicEffectDataBuilder>();
            RelicPoolIDs = new List<string>();
            RelicLoreTooltipStyle = RelicData.RelicLoreTooltipStyle.Herzal;
            IgnoreForNoRelicAchievement = false;
            DivineVariant = false;
            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        public CollectableRelicData BuildAndRegister()
        {
            var relicData = Build();
            CustomCollectableRelicManager.RegisterCustomRelic(relicData, RelicPoolIDs);
            return relicData;
        }

        public CollectableRelicData Build()
        {
            if (CollectableRelicID == null)
            {
                throw new BuilderException("CollectableRelicID is required");
            }

            var relicData = ScriptableObject.CreateInstance<CollectableRelicData>();
            var guid = GUIDGenerator.GenerateDeterministicGUID(CollectableRelicID);
            AccessTools.Field(typeof(GameData), "id").SetValue(relicData, guid);
            relicData.name = CollectableRelicID;

            // RelicData.effects is not allocated initially
            var effects = new List<RelicEffectData>();
            effects.AddRange(Effects);
            foreach (var builder in EffectBuilders)
            {
                effects.Add(builder.Build());
            }

            // RelicData fields
            AccessTools.Field(typeof(RelicData), "descriptionKey").SetValue(relicData, DescriptionKey);
            AccessTools.Field(typeof(RelicData), "divineVariant").SetValue(relicData, DivineVariant);
            AccessTools.Field(typeof(RelicData), "effects").SetValue(relicData, effects);
            AccessTools.Field(typeof(RelicData), "nameKey").SetValue(relicData, NameKey);
            AccessTools.Field(typeof(RelicData), "relicActivatedKey").SetValue(relicData, RelicActivatedKey);
            AccessTools.Field(typeof(RelicData), "relicLoreTooltipKeys").SetValue(relicData, RelicLoreTooltipKeys);
            AccessTools.Field(typeof(RelicData), "relicLoreTooltipStyle").SetValue(relicData, RelicLoreTooltipStyle);
            if (IconPath != null)
            {
                Sprite iconSprite = CustomAssetManager.LoadSpriteFromPath(FullAssetPath);
                AccessTools.Field(typeof(RelicData), "icon").SetValue(relicData, iconSprite);
            }

            // CollectableRelicData fields
            AccessTools.Field(typeof(CollectableRelicData), "fromStoryEvent").SetValue(relicData, FromStoryEvent);
            AccessTools.Field(typeof(CollectableRelicData), "ignoreForNoRelicAchievement").SetValue(relicData, IgnoreForNoRelicAchievement);
            AccessTools.Field(typeof(CollectableRelicData), "isBossGivenRelic").SetValue(relicData, IsBossGivenRelic);
            AccessTools.Field(typeof(CollectableRelicData), "rarity").SetValue(relicData, Rarity);
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(relicData, RequiredDLC);
            AccessTools.Field(typeof(CollectableRelicData), "unlockLevel").SetValue(relicData, UnlockLevel);

            var linkedClass = ClanID == null ? null : CustomClassManager.GetClassDataByID(ClanID);
            AccessTools.Field(typeof(CollectableRelicData), "linkedClass").SetValue(relicData, linkedClass);

            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);
            BuilderUtils.ImportStandardLocalization(NameKey, Name);

            return relicData;
        }
    }
}
