using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Trainworks.Managers;
using Trainworks.ManagersV2;
using UnityEngine;

namespace Trainworks.Builders
{
    public class CardUpgradeDataBuilder
    {
        private static HashSet<string> UsedUpgradeTitles = new HashSet<string>();
        /// <summary>
        /// Don't set directly; use UpgradeTitle instead.
        /// </summary>
        public string upgradeTitle;

        /// <summary>
        /// Overrides UpgradeTitleKey
        /// Implicitly sets UpgradeTitleKey, UpgradeDescriptionKey, and UpgradeNotificationKey if null
        /// </summary>
        public string UpgradeTitle
        {
            get { return this.upgradeTitle; }
            set
            {
                this.upgradeTitle = value;
                if (this.UpgradeTitleKey == null)
                {
                    this.UpgradeTitleKey = this.upgradeTitle + "_CardUpgradeData_UpgradeTitleKey";
                }
                if (this.UpgradeDescriptionKey == null)
                {
                    this.UpgradeDescriptionKey = this.upgradeTitle + "_CardUpgradeData_UpgradeDescriptionKey";
                }
                if (this.UpgradeNotificationKey == null)
                {
                    this.UpgradeNotificationKey = this.upgradeTitle + "_CardUpgradeData_UpgradeNotificationKey";
                }
            }
        }
        /// <summary>
        /// Overrides UpgradeDescriptionKey
        /// </summary>
        public string UpgradeDescription { get; set; }
        /// <summary>
        /// Overrides UpgradeNotificationKey
        /// </summary>
        public string UpgradeNotification { get; set; }
        public string UpgradeTitleKey { get; set; }
        public string UpgradeDescriptionKey { get; set; }
        public string UpgradeNotificationKey { get; set; }
        public string UpgradeIconPath { get; set; }
        public bool HideUpgradeIconOnCard { get; set; }
        public bool UseUpgradeHighlightTextTags { get; set; }
        public int BonusDamage { get; set; }
        public int BonusHP { get; set; }
        public int CostReduction { get; set; }
        public int XCostReduction { get; set; }
        public int BonusHeal { get; set; }
        public int BonusSize { get; set; }

        public List<CardTraitDataBuilder> TraitDataUpgradeBuilders { get; set; }
        public List<CharacterTriggerDataBuilder> TriggerUpgradeBuilders { get; set; }
        public List<CardTriggerEffectDataBuilder> CardTriggerUpgradeBuilders { get; set; }
        public List<RoomModifierDataBuilder> RoomModifierUpgradeBuilders { get; set; }
        public List<CardUpgradeMaskDataBuilder> FiltersBuilders { get; set; }
        public List<CardUpgradeDataBuilder> UpgradesToRemoveBuilders { get; set; }

        /// <summary>
        /// To add a status effect, no need for a builder. new StatusEffectStackData with properties statusId (string) and count (int) are sufficient.
        /// Get the string with -> statusEffectID = MTStatusEffectIDs.GetIDForType(statusEffectType);
        /// </summary>
        public List<StatusEffectStackData> StatusEffectUpgrades { get; set; }
        public List<CardTraitData> TraitDataUpgrades { get; set; }
        public List<string> RemoveTraitUpgrades { get; set; }
        public List<CharacterTriggerData> TriggerUpgrades { get; set; }
        public List<CardTriggerEffectData> CardTriggerUpgrades { get; set; }
        public List<RoomModifierData> RoomModifierUpgrades { get; set; }
        public List<CardUpgradeMaskData> Filters { get; set; }
        public List<CardUpgradeData> UpgradesToRemove { get; set; }

        public CharacterData SourceSynthesisUnit { get; set; }
        public bool IsUnitSynthesisUpgrade { get => SourceSynthesisUnit != null; }

        public string BaseAssetPath { get; set; }

        public CardUpgradeDataBuilder()
        {
            this.UpgradeNotificationKey = null;
            this.UseUpgradeHighlightTextTags = true;

            this.TraitDataUpgradeBuilders = new List<CardTraitDataBuilder>();
            this.TriggerUpgradeBuilders = new List<CharacterTriggerDataBuilder>();
            this.CardTriggerUpgradeBuilders = new List<CardTriggerEffectDataBuilder>();
            this.RoomModifierUpgradeBuilders = new List<RoomModifierDataBuilder>();
            this.FiltersBuilders = new List<CardUpgradeMaskDataBuilder>();
            this.UpgradesToRemoveBuilders = new List<CardUpgradeDataBuilder>();

            this.StatusEffectUpgrades = new List<StatusEffectStackData>();
            this.TraitDataUpgrades = new List<CardTraitData>();
            this.RemoveTraitUpgrades = new List<string>();
            this.TriggerUpgrades = new List<CharacterTriggerData>();
            this.CardTriggerUpgrades = new List<CardTriggerEffectData>();
            this.RoomModifierUpgrades = new List<RoomModifierData>();
            this.Filters = new List<CardUpgradeMaskData>();
            this.UpgradesToRemove = new List<CardUpgradeData>();

            var assembly = Assembly.GetCallingAssembly();
            this.BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        public CardUpgradeData Build()
        {
            CardUpgradeData cardUpgradeData = ScriptableObject.CreateInstance<CardUpgradeData>();

            foreach (var builder in this.TraitDataUpgradeBuilders)
            {
                this.TraitDataUpgrades.Add(builder.Build());
            }
            foreach (var builder in this.TriggerUpgradeBuilders)
            {
                this.TriggerUpgrades.Add(builder.Build());
            }
            foreach (var builder in this.CardTriggerUpgradeBuilders)
            {
                this.CardTriggerUpgrades.Add(builder.Build());
            }
            foreach (var builder in this.RoomModifierUpgradeBuilders)
            {
                this.RoomModifierUpgrades.Add(builder.Build());
            }
            foreach (var builder in this.FiltersBuilders)
            {
                this.Filters.Add(builder.Build());
            }
            foreach (var builder in this.UpgradesToRemoveBuilders)
            {
                this.UpgradesToRemove.Add(builder.Build());
            }


            var UpgradeID = UpgradeTitleKey;
            if (UpgradeID == null)
            {
                StackFrame frame = new StackFrame(1);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                var name = method.Name;
                var baseUpgradeID = string.Format("{0}.{1}", type, name);
                
                UpgradeID = baseUpgradeID;
                int seen = 0;
                while (UsedUpgradeTitles.Contains(UpgradeID))
                {
                    seen++;
                    UpgradeID = string.Format("{0}.{1}", baseUpgradeID, seen);
                }

                Trainworks.Log(LogLevel.Error, "UpgradeTitleKey not set. Setting the name/id of this CardUpgrade to " + UpgradeID + ". If this CardUpgrade is meant to be permanent upgrade on a card, please set UpgradeTitleKey for this upgrade otherwise Run History Data will be incorrect.");
            }

            AccessTools.Field(typeof(CardUpgradeData), "bonusDamage").SetValue(cardUpgradeData, this.BonusDamage);
            AccessTools.Field(typeof(CardUpgradeData), "bonusHeal").SetValue(cardUpgradeData, this.BonusHeal);
            AccessTools.Field(typeof(CardUpgradeData), "bonusHP").SetValue(cardUpgradeData, this.BonusHP);
            AccessTools.Field(typeof(CardUpgradeData), "bonusSize").SetValue(cardUpgradeData, this.BonusSize);
            AccessTools.Field(typeof(CardUpgradeData), "cardTriggerUpgrades").SetValue(cardUpgradeData, this.CardTriggerUpgrades);
            AccessTools.Field(typeof(CardUpgradeData), "costReduction").SetValue(cardUpgradeData, this.CostReduction);
            AccessTools.Field(typeof(CardUpgradeData), "filters").SetValue(cardUpgradeData, this.Filters);
            AccessTools.Field(typeof(CardUpgradeData), "hideUpgradeIconOnCard").SetValue(cardUpgradeData, this.HideUpgradeIconOnCard);
            AccessTools.Field(typeof(CardUpgradeData), "removeTraitUpgrades").SetValue(cardUpgradeData, this.RemoveTraitUpgrades);
            AccessTools.Field(typeof(CardUpgradeData), "roomModifierUpgrades").SetValue(cardUpgradeData, this.RoomModifierUpgrades);
            AccessTools.Field(typeof(CardUpgradeData), "statusEffectUpgrades").SetValue(cardUpgradeData, this.StatusEffectUpgrades);
            AccessTools.Field(typeof(CardUpgradeData), "traitDataUpgrades").SetValue(cardUpgradeData, this.TraitDataUpgrades);
            AccessTools.Field(typeof(CardUpgradeData), "triggerUpgrades").SetValue(cardUpgradeData, this.TriggerUpgrades);
            BuilderUtils.ImportStandardLocalization(this.UpgradeDescriptionKey, this.UpgradeDescription);
            AccessTools.Field(typeof(CardUpgradeData), "upgradeDescriptionKey").SetValue(cardUpgradeData, this.UpgradeDescriptionKey);
            if (this.UpgradeIconPath != null && this.UpgradeIconPath != "")
                AccessTools.Field(typeof(CardUpgradeData), "upgradeIcon").SetValue(cardUpgradeData, CustomAssetManager.LoadSpriteFromPath(this.BaseAssetPath + "/" + this.UpgradeIconPath));
            BuilderUtils.ImportStandardLocalization(this.UpgradeNotificationKey, this.UpgradeNotification);
            AccessTools.Field(typeof(CardUpgradeData), "upgradeNotificationKey").SetValue(cardUpgradeData, this.UpgradeNotificationKey);
            AccessTools.Field(typeof(CardUpgradeData), "upgradesToRemove").SetValue(cardUpgradeData, this.UpgradesToRemove);
            BuilderUtils.ImportStandardLocalization(this.UpgradeTitleKey, this.UpgradeTitle);
            AccessTools.Field(typeof(CardUpgradeData), "upgradeTitleKey").SetValue(cardUpgradeData, this.UpgradeTitleKey);
            AccessTools.Field(typeof(CardUpgradeData), "useUpgradeHighlightTextTags").SetValue(cardUpgradeData, this.UseUpgradeHighlightTextTags);
            AccessTools.Field(typeof(CardUpgradeData), "xCostReduction").SetValue(cardUpgradeData, this.XCostReduction);

            AccessTools.Field(typeof(CardUpgradeData), "isUnitSynthesisUpgrade").SetValue(cardUpgradeData, IsUnitSynthesisUpgrade);
            AccessTools.Field(typeof(CardUpgradeData), "sourceSynthesisUnit").SetValue(cardUpgradeData, SourceSynthesisUnit);

            cardUpgradeData.name = UpgradeTitleKey ?? UpgradeID;
            Traverse.Create(cardUpgradeData).Field("id").SetValue(UpgradeTitleKey ?? UpgradeID);

            CustomUpgradeManager.RegisterOldVersionCustomUpgrade(cardUpgradeData);

            return cardUpgradeData;
        }
    }
}
