using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using Trainworks.ManagersV2;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    // Nothing changes except the Registration. CustomEnhancerPoolManager is broken, and the fact its not added to the save data.
    public class EnhancerDataBuilder
    {
        private string enhancerID;
        /// <summary>
        /// Internal ID for use by Unity. Must be unique.
        /// Sets NameKey and DescriptionKey.
        /// </summary>
        public string EnhancerID
        {
            get { return enhancerID; }
            set
            {
                enhancerID = value;
                if (NameKey == null)
                {
                    NameKey = enhancerID + "_EnhancerData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = enhancerID + "_EnhancerData_DescriptionKey";
                }
            }
        }
        /// <summary>
        /// Name displayed for the enhancer.
        /// Note if set, will set the localization for all languages.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Localization key for the enhancer's name.
        /// This should not need to be set manually.
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Description displayed for the enhancer.
        /// Note if set, will set the localization for all languages.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Localization key for the enhancer's description.
        /// This should not need to be set manually.
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// ID of the clan the card is a part of. Leave null for clanless.
        /// Base game clan IDs should be retrieved via helper class "VanillaClanIDs".
        /// </summary>
        public string ClanID { get; set; }
        /// <summary>
        /// Holds the upgrade to be applied to the chosen card after the enhancer is purchased.
        /// </summary>
        public CardUpgradeData Upgrade { get; set; }
        /// <summary>
        /// Convenience builder for Upgrade.
        /// If set overrides Upgrade.
        /// </summary>
        public CardUpgradeDataBuilder UpgradeBuilder { get; set; }
        public CollectableRarity Rarity { get; set; }
        /// <summary>
        /// Determines which types of cards it upgrades.
        /// </summary>
        public CardType CardType { get; set; }
        /// <summary>
        /// The IDs of all enhancer pools the enhancer should be inserted into.
        /// </summary>
        public List<string> EnhancerPoolIDs { get; set; }
        /// <summary>
        /// Unlock level.
        /// </summary>
        public int UnlockLevel { get; set; }
        /// <summary>
        /// Number of Cards shown in Upgrade Screen.
        /// </summary>
        public int NumCardsShwonInUpgradeScreen { get; set; }
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + IconPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Custom asset path to load art from. 76x76 image.
        /// </summary>
        public string IconPath { get; set; }
        /// <summary>
        /// ADVANCED. Override Enhancer Relic Effect.
        /// Necessary for Special Enhancers like GenericDuplicator.
        /// Note this can be set to RelicEffectCardDuplicator.
        /// Don't set to anything else unless you know what you are doing.
        /// </summary>
        public RelicEffectDataBuilder OverrideEffectBuilder { get; set; }

        public EnhancerDataBuilder()
        {
            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
            EnhancerPoolIDs = new List<string>();
            NumCardsShwonInUpgradeScreen = -1;
        }

        /// <summary>
        /// Builds the EnhancerData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered EnhancerData</returns>
        public EnhancerData BuildAndRegister()
        {
            var enhancerData = Build();
            CustomEnhancerManager.RegisterEnhancer(enhancerData, EnhancerPoolIDs);
            return enhancerData;
        }

        /// <summary>
        /// Builds the EnhancerData represented by this builder's parameters.
        /// </summary>
        /// <returns>The newly created EnhancerData</returns>
        public EnhancerData Build()
        {
            if (EnhancerID == null)
            {
                throw new BuilderException("EnhancerID is required");
            }

            var enhancerData = ScriptableObject.CreateInstance<EnhancerData>();

            var guid = GUIDGenerator.GenerateDeterministicGUID(EnhancerID);
            AccessTools.Field(typeof(GameData), "id").SetValue(enhancerData, guid);
            enhancerData.name = enhancerID;

            // Upgrades are contained within a relic effect - this is mandatory or the game will crash.
            // TODO check if using RelicEffectClassName is ok.
            CardUpgradeData upgrade = Upgrade;
            if (UpgradeBuilder != null)
            {
                upgrade = UpgradeBuilder.Build();
            }

            var relicEffect = new RelicEffectDataBuilder
            {
                RelicEffectClassType = typeof(RelicEffectCardUpgrade),
                ParamCardUpgradeData = upgrade,
                ParamCardType = CardType,
                ParamCharacterSubtype = VanillaSubtypeIDs.None,
            };

            if (OverrideEffectBuilder != null)
            {
                relicEffect = OverrideEffectBuilder;
            }

            List<RelicEffectData> Effects = new List<RelicEffectData>
            {
                relicEffect.Build()
            };
            AccessTools.Field(typeof(RelicData), "effects").SetValue(enhancerData, Effects);

            var linkedClass = ClanID == null ? null : CustomClassManager.GetClassDataByID(ClanID);
            AccessTools.Field(typeof(RelicData), "descriptionKey").SetValue(enhancerData, DescriptionKey);
            AccessTools.Field(typeof(RelicData), "nameKey").SetValue(enhancerData, NameKey);
            AccessTools.Field(typeof(EnhancerData), "linkedClass").SetValue(enhancerData, linkedClass);
            AccessTools.Field(typeof(EnhancerData), "rarity").SetValue(enhancerData, Rarity);
            AccessTools.Field(typeof(EnhancerData), "unlockLevel").SetValue(enhancerData, UnlockLevel);
            AccessTools.Field(typeof(EnhancerData), "numCardsToShowInUpgradeScreen").SetValue(enhancerData, NumCardsShwonInUpgradeScreen);

            if (IconPath != null)
            {
                Sprite iconSprite = CustomAssetManager.LoadSpriteFromPath(FullAssetPath);
                AccessTools.Field(typeof(RelicData), "icon").SetValue(enhancerData, iconSprite);
            }

            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);
            BuilderUtils.ImportStandardLocalization(NameKey, Name);

            return enhancerData;
        }
    }
}
