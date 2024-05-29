using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.ManagersV2;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class CardUpgradeDataBuilder
    {
        private string upgradeID;

        /// <summary>
        /// UpgradeID, must be unique.
        /// Implicitly sets UpgradeTitleKey and UpgradeDescriptionKey
        /// </summary>
        public string UpgradeID
        {
            get { return upgradeID; }
            set
            {
                upgradeID = value;
                if (UpgradeTitleKey == null)
                {
                    UpgradeTitleKey = upgradeID + "_CardUpgradeData_UpgradeTitleKey";
                }
                if (UpgradeDescriptionKey == null)
                {
                    UpgradeDescriptionKey = upgradeID + "_CardUpgradeData_UpgradeDescriptionKey";
                }
            }
        }

        /// <summary>
        /// UpgradeTitle. This should be set if it is a Champion Upgrade.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string UpgradeTitle { get; set; }
        /// <summary>
        /// Upgrade Description.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string UpgradeDescription { get; set; }
        /// <summary>
        /// Note that setting this property will set the localization for all languages.
        /// This doesn't seem to be used by any cards in game. Some cards do have this field set, but there's no localization for the text.
        /// Note if you set this UpgradeNotificationKey must also be set.
        /// </summary>
        public string UpgradeNotification { get; set; }
        /// <summary>
        /// Upgrade Title Key for localization.
        /// No need to set this as its automatically set by UpgradeTitle
        /// </summary>
        public string UpgradeTitleKey { get; set; }
        /// <summary>
        /// Upgrade Description Key for localization.
        /// No need to set this as its automatically set by UpgradeTitle
        /// </summary>
        public string UpgradeDescriptionKey { get; set; }
        /// <summary>
        /// Upgrade Notification Key for localization.
        /// This doesn't seem to be used by any cards in game. Some cards do have this field set, but there's no localization for the text.
        /// Note this is not set automatically as its uncommon to use this.
        /// </summary>
        public string UpgradeNotificationKey { get; set; }
        /// <summary>
        /// Hide the Upgrade Icon on Card.
        /// This is mainly for permanent upgrades, such as Charged Echoes (Corruption)
        /// </summary>
        public bool HideUpgradeIconOnCard { get; set; }
        /// <summary>
        /// Highlight changes in the card when the upgrade is applied.
        /// Defaults to true. The use case for setting to false is if the Upgrade is a starting upgrade for a card.
        /// </summary>
        public bool UseUpgradeHighlightTextTags { get; set; }
        /// <summary>
        /// Bonus Damage for a Card or Unit. Note that this doesn't increase Heal amounts.
        /// </summary>
        public int BonusDamage { get; set; }
        /// <summary>
        /// Bonus HP for a unit.
        /// </summary>
        public int BonusHP { get; set; }
        /// <summary>
        /// Reduce ember to play card. Can be negative to increase Ember.
        /// </summary>
        public int CostReduction { get; set; }
        /// <summary>
        /// Reduce ember to play X Cost card. Can be negative.
        /// </summary>
        public int XCostReduction { get; set; }
        /// <summary>
        /// Bonus Healing
        /// </summary>
        public int BonusHeal { get; set; }
        /// <summary>
        /// Increases size of a unit. Can be negative to reduce size.
        /// </summary>
        public int BonusSize { get; set; }
        /// <summary>
        /// Convenience Builders for TraitDataUpgrade. Will be appended to the parameter.
        /// </summary>
        public List<CardTraitDataBuilder> TraitDataUpgradeBuilders { get; set; }
        /// <summary>
        /// Convenience Builders for TriggerUpgrade. Will be appended to the parameter.
        /// </summary>
        public List<CharacterTriggerDataBuilder> TriggerUpgradeBuilders { get; set; }
        /// <summary>
        /// Convenience Builders for CardTriggerUpgrade. Will be appended to the parameter.
        /// </summary>
        public List<CardTriggerEffectDataBuilder> CardTriggerUpgradeBuilders { get; set; }
        /// <summary>
        /// Convenience Builders for RoomModifierUpgrade. Will be appended to the parameter.
        /// </summary>
        public List<RoomModifierDataBuilder> RoomModifierUpgradeBuilders { get; set; }
        /// <summary>
        /// Convenience Builders for Filters. Will be appended to the parameter.
        /// </summary>
        public List<CardUpgradeMaskDataBuilder> FiltersBuilders { get; set; }
        /// <summary>
        /// Convenience Builders for UpgradesToRemove. Will be appended to the parameter.
        /// </summary>
        public List<CardUpgradeDataBuilder> UpgradesToRemoveBuilders { get; set; }
        /// <summary>
        /// Status Effects to apply to the unit.
        /// </summary>
        public List<StatusEffectStackData> StatusEffectUpgrades { get; set; }
        /// <summary>
        /// Card Traits to add.
        /// </summary>
        public List<CardTraitData> TraitDataUpgrades { get; set; }
        /// <summary>
        /// Traits to Remove when applied to a card.
        /// </summary>
        public List<string> RemoveTraitUpgrades { get; set; }
        /// <summary>
        /// When applied to a unit, additional Triggers to add.
        /// </summary>
        public List<CharacterTriggerData> TriggerUpgrades { get; set; }
        /// <summary>
        /// Card trigger upgrades to add to card.
        /// </summary>
        public List<CardTriggerEffectData> CardTriggerUpgrades { get; set; }
        /// <summary>
        /// Room Modifiers to add to a unit.
        /// </summary>
        public List<RoomModifierData> RoomModifierUpgrades { get; set; }
        /// <summary>
        /// Filters which can filter out cards to that the upgrade can be applied to.
        /// </summary>
        public List<CardUpgradeMaskData> Filters { get; set; }
        /// <summary>
        /// Upgrades to remove when this upgrade is applied.
        /// Not commonly used. The most common use case in the MT codebase is to remove a startingUpgrade from a card.
        /// The only use of this parameter is in the upgraded Spikedriver Colony and Improved Spikedriver
        /// to remove the self-duplication from the card.
        /// With primordium's superfood upgrade path, it removes the startingUpgrade from the card which
        /// effectively replaces the OnEaten effect to include the Stats and the Status Effects.
        /// Rarely, this is used to remove a previous champion upgrade. If your Champion Upgrade includes a CardTrait
        /// or CardTriggerUpgrade which are upgrades applied to the Card, then you will need to use this to remove
        /// said CardTrait or CardTriggerUpgrade since these aren't removed automatically. (See: LittleFade's Spikes Path).
        /// </summary>
        public List<CardUpgradeData> UpgradesToRemove { get; set; }
        /// <summary>
        /// Indicate that this CardUpgrade is a Synthesis by setting the CharacterData for it.
        /// </summary>
        public CharacterData SourceSynthesisUnit { get; set; }
        /// <summary>
        /// Is the Upgrade for a Unit Synthesis for a Character.
        /// </summary>
        public bool IsUnitSynthesisUpgrade { get => SourceSynthesisUnit != null; }
        /// <summary>
        /// Indicates that the upgrade can only be applied to a card/unit once.
        /// </summary>
        public bool IsUnique { get; set; }
        /// <summary>
        /// Specifies how much pact shards should be gained for duplicating the card.
        /// Common for the Left Divine Temple Upgrade (+5 Pact Shards).
        /// Uncommon for the Right Divine Temple Upgrade (+10 Pact Shards).
        /// Rare for Unit Essences (+15 Pact Shards).
        /// </summary>
        public CollectableRarity LinkedPactDuplicateRarity { get; set; }
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

        public CardUpgradeDataBuilder()
        {
            TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>();
            TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>();
            CardTriggerUpgradeBuilders = new List<CardTriggerEffectDataBuilder>();
            RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>();
            FiltersBuilders = new List<CardUpgradeMaskDataBuilder>();
            UpgradesToRemoveBuilders = new List<CardUpgradeDataBuilder>();

            StatusEffectUpgrades = new List<StatusEffectStackData>();
            TraitDataUpgrades = new List<CardTraitData>();
            RemoveTraitUpgrades = new List<string>();
            TriggerUpgrades = new List<CharacterTriggerData>();
            CardTriggerUpgrades = new List<CardTriggerEffectData>();
            RoomModifierUpgrades = new List<RoomModifierData>();
            Filters = new List<CardUpgradeMaskData>();
            UpgradesToRemove = new List<CardUpgradeData>();
            LinkedPactDuplicateRarity = CollectableRarity.Starter;
            UpgradeNotificationKey = "";
            UseUpgradeHighlightTextTags = true;

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }


        public CardUpgradeData BuildAndRegister()
        {
            var upgrade = Build(false);
            CustomUpgradeManager.RegisterCustomUpgrade(upgrade);
            return upgrade;
        }

        /// <summary>
        /// Builds and automatically registers the CardUpgradeData.
        /// </summary>
        /// <returns>CardUpgradeData</returns>
        public CardUpgradeData Build(bool register = true)
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (UpgradeID == null)
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Warning should provide a UpgradeID.");
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Stacktrace: " + Environment.StackTrace);
            }

            CardUpgradeData cardUpgradeData = ScriptableObject.CreateInstance<CardUpgradeData>();
            cardUpgradeData.name = upgradeID;
            var guid = GUIDGenerator.GenerateDeterministicGUID(UpgradeID);
            AccessTools.Field(typeof(GameData), "id").SetValue(cardUpgradeData, guid);
            AccessTools.Field(typeof(CardUpgradeData), "bonusDamage").SetValue(cardUpgradeData, BonusDamage);
            AccessTools.Field(typeof(CardUpgradeData), "bonusHeal").SetValue(cardUpgradeData, BonusHeal);
            AccessTools.Field(typeof(CardUpgradeData), "bonusHP").SetValue(cardUpgradeData, BonusHP);
            AccessTools.Field(typeof(CardUpgradeData), "bonusSize").SetValue(cardUpgradeData, BonusSize);
            AccessTools.Field(typeof(CardUpgradeData), "costReduction").SetValue(cardUpgradeData, CostReduction);
            AccessTools.Field(typeof(CardUpgradeData), "hideUpgradeIconOnCard").SetValue(cardUpgradeData, HideUpgradeIconOnCard);
            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(cardUpgradeData, IsUnitSynthesisUpgrade);
            AccessTools.Field(typeof(CardUpgradeData), "isUnique").SetValue(cardUpgradeData, IsUnique);
            AccessTools.Field(typeof(CardUpgradeData), "linkedPactDuplicateRarity").SetValue(cardUpgradeData, LinkedPactDuplicateRarity);
            AccessTools.Field(typeof(CardUpgradeData), "removeTraitUpgrades").SetValue(cardUpgradeData, RemoveTraitUpgrades);
            AccessTools.Field(typeof(CardUpgradeData), "sourceSynthesisUnit").SetValue(cardUpgradeData, SourceSynthesisUnit);
            AccessTools.Field(typeof(CardUpgradeData), "statusEffectUpgrades").SetValue(cardUpgradeData, StatusEffectUpgrades);
            AccessTools.Field(typeof(CardUpgradeData), "upgradeDescriptionKey").SetValue(cardUpgradeData, UpgradeDescriptionKey);
            AccessTools.Field(typeof(CardUpgradeData), "upgradeNotificationKey").SetValue(cardUpgradeData, UpgradeNotificationKey);
            AccessTools.Field(typeof(CardUpgradeData), "upgradeTitleKey").SetValue(cardUpgradeData, UpgradeTitleKey);
            AccessTools.Field(typeof(CardUpgradeData), "useUpgradeHighlightTextTags").SetValue(cardUpgradeData, UseUpgradeHighlightTextTags);
            AccessTools.Field(typeof(CardUpgradeData), "xCostReduction").SetValue(cardUpgradeData, XCostReduction);

            // Save allocations by using the allocated list from initialization.
            var cardTriggers = cardUpgradeData.GetCardTriggerUpgrades();
            cardTriggers.AddRange(CardTriggerUpgrades);
            foreach (var builder in CardTriggerUpgradeBuilders)
            {
                cardTriggers.Add(builder.Build());
            }

            var filters = cardUpgradeData.GetFilters();
            filters.AddRange(Filters);
            foreach (var builder in FiltersBuilders)
            {
                filters.Add(builder.Build());
            }

            var roomModifiers = cardUpgradeData.GetRoomModifierUpgrades();
            roomModifiers.AddRange(RoomModifierUpgrades);
            foreach (var builder in RoomModifierUpgradeBuilders)
            {
                roomModifiers.Add(builder.Build());
            }

            var triggers = cardUpgradeData.GetTriggerUpgrades();
            triggers.AddRange(TriggerUpgrades);
            foreach (var builder in TriggerUpgradeBuilders)
            {
                triggers.Add(builder.Build());
            }

            var upgradesToRemove = cardUpgradeData.GetUpgradesToRemove();
            upgradesToRemove.AddRange(UpgradesToRemove);
            foreach (var builder in UpgradesToRemoveBuilders)
            {
                upgradesToRemove.Add(builder.Build());
            }

            // List was not allocated, make a new list and add data.
            var traitDatas = new List<CardTraitData>();
            traitDatas.AddRange(TraitDataUpgrades);
            foreach (var builder in TraitDataUpgradeBuilders)
            {
                traitDatas.Add(builder.Build());
            }
            AccessTools.Field(typeof(CardUpgradeData), "traitDataUpgrades").SetValue(cardUpgradeData, traitDatas);


            if (AssetPath != null)
            {
                AccessTools.Field(typeof(CardUpgradeData), "upgradeIcon").SetValue(cardUpgradeData, CustomAssetManager.LoadSpriteFromPath(FullAssetPath));
            }

            BuilderUtils.ImportStandardLocalization(UpgradeDescriptionKey, UpgradeDescription);
            BuilderUtils.ImportStandardLocalization(UpgradeNotificationKey, UpgradeNotification);
            BuilderUtils.ImportStandardLocalization(UpgradeTitleKey, UpgradeTitle);

            if (register)
            {
                CustomUpgradeManager.RegisterCustomUpgrade(cardUpgradeData);
            }

            return cardUpgradeData;
        }
    }
}