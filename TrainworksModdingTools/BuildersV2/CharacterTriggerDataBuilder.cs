using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Trainworks.BuildersV2
{
    public class CharacterTriggerDataBuilder
    {
        private string triggerID;

        /// <summary>
        /// A unique ID for this trigger.
        /// Implicitly sets DescriptionKey if null.
        /// </summary>
        public string TriggerID
        {
            get { return triggerID; }
            set
            {
                triggerID = value;
                if (DescriptionKey == null)
                {
                    DescriptionKey = triggerID + "_CharacterTriggerData_DescriptionKey";
                }
            }
        }

        public CharacterTriggerData.Trigger Trigger { get; set; }
        /// <summary>
        /// Append to this list to add new card effects.
        /// </summary>
        public List<CardEffectDataBuilder> EffectBuilders { get; set; }
        /// <summary>
        /// List of pre-built card effects.
        /// </summary>
        public List<CardEffectData> Effects { get; set; }
        /// <summary>
        /// Description displayed for the relic.
        /// Note that setting this field will set the localization for all langauges.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Additional Text on Trigger
        /// Note that setting this field will set the localization for all langauges.
        /// If set then AdditionalTextOnTriggerKey must also be set.
        /// </summary>
        public string AdditionalTextOnTrigger { get; set; }
        /// <summary>
        /// Localization key for the text's localization.
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// Localization key for the text's localization.
        /// Note this isn't set automatically as its use is not common.
        /// </summary>
        public string AdditionalTextOnTriggerKey { get; set; }
        public bool DisplayEffectHintText { get; set; }
        public bool HideTriggerTooltip { get; set; }

        public CharacterTriggerDataBuilder()
        {
            EffectBuilders = new List<CardEffectDataBuilder>();
            Effects = new List<CardEffectData>();
            HideTriggerTooltip = false;
            DisplayEffectHintText = false;
            AdditionalTextOnTriggerKey = "";
        }

        /// <summary>
        /// Builds the CharacterTriggerData represented by this builders's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created CardTraitData</returns>
        public CharacterTriggerData Build()
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (TriggerID == null)
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Warning should provide a TriggerID.");
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Stacktrace: " + Environment.StackTrace);
            }

            CharacterTriggerData characterTriggerData = new CharacterTriggerData(Trigger, /* cardEffectData = */null);

            var effects = characterTriggerData.GetEffects();
            effects.Clear(); // Contains a null from CharacterTriggerData's constructor.
            effects.AddRange(Effects);
            foreach (var builder in EffectBuilders)
            {
                effects.Add(builder.Build());
            }

            AccessTools.Field(typeof(CharacterTriggerData), "additionalTextOnTriggerKey").SetValue(characterTriggerData, AdditionalTextOnTriggerKey);
            AccessTools.Field(typeof(CharacterTriggerData), "descriptionKey").SetValue(characterTriggerData, DescriptionKey);
            AccessTools.Field(typeof(CharacterTriggerData), "displayEffectHintText").SetValue(characterTriggerData, DisplayEffectHintText);
            AccessTools.Field(typeof(CharacterTriggerData), "hideTriggerTooltip").SetValue(characterTriggerData, HideTriggerTooltip);
            AccessTools.Field(typeof(CharacterTriggerData), "trigger").SetValue(characterTriggerData, Trigger);

            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);
            BuilderUtils.ImportStandardLocalization(AdditionalTextOnTriggerKey, AdditionalTextOnTrigger);

            return characterTriggerData;
        }
    }
}