using HarmonyLib;
using System;
using System.Collections.Generic;
using Trainworks.ConstantsV2;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class CardEffectDataBuilder
    {
        /// <summary>
        /// Type of the effect class to instantiate.
        /// This should be a class inheriting from CardEffectBase.
        /// </summary>
        public Type EffectStateType { get; set; }
        /// <summary>
        /// Card Effect class to instantiate.
        /// Note that this isn't a simple string name of the class it is the class name plus the Assembly info (if necessary).
        /// </summary>
        public string EffectStateName => BuilderUtils.GetEffectClassName(EffectStateType);
        /// <summary>
        /// Int parameter.
        /// </summary>
        public int ParamInt { get; set; }
        /// <summary>
        /// Int parameter.
        /// </summary>
        public int AdditionalParamInt { get; set; }
        /// <summary>
        /// Int parameter
        /// </summary>
        public int ParamMaxInt { get; set; }
        /// <summary>
        /// Int parameter.
        /// </summary>
        public int ParamMinInt { get; set; }
        /// <summary>
        /// Float parameter.
        /// </summary>
        public float ParamMultiplier { get; set; }
        /// <summary>
        /// Boolean parameter.
        /// </summary>
        public bool ParamBool { get; set; }
        /// <summary>
        /// String parameter.
        /// </summary>
        public string ParamStr { get; set; }
        /// <summary>
        /// Subtype parameter.
        /// </summary>
        public string ParamSubtype { get; set; }
        /// <summary>
        /// CardUpgradeMaskData parameter.
        /// </summary>
        public CardUpgradeMaskData ParamCardFilter { get; set; }
        /// <summary>
        /// CardPool parameter.
        /// </summary>
        public CardPool ParamCardPool { get; set; }
        /// <summary>
        /// CardUpgradeData parameter.
        /// </summary>
        public CardUpgradeData ParamCardUpgradeData { get; set; }
        /// <summary>
        /// CardUpgradeData parameter.
        /// If this is set overrides ParamCardUpgradeData
        /// </summary>
        public CardUpgradeDataBuilder ParamCardUpgradeDataBuilder { get; set; }
        /// <summary>
        /// CharacterData parameter.
        /// </summary>
        public CharacterData ParamCharacterData { get; set; }
        /// <summary>
        /// Convenience Builder for CharacterData parameter. If set overrides ParamCharacterData.
        /// </summary>
        public CharacterDataBuilder ParamCharacterDataBuilder { get; set; }
        /// <summary>
        /// CharacterData parameter.
        /// </summary>
        public CharacterData ParamAdditionalCharacterData { get; set; }
        /// <summary>
        /// CharacterDataPool parameter.
        /// </summary>
        public List<CharacterData> ParamCharacterDataPool { get; set; }
        /// <summary>
        /// Convenience Builder for CharacterDataPool parameter. This will be append to CharacterDataPool when built.
        /// </summary>
        public List<CharacterDataBuilder> ParamCharacterDataPoolBuilder { get; set; }
        /// <summary>
        /// RoomData parameter.
        /// </summary>
        public RoomData ParamRoomData { get; set; }
        /// <summary>
        /// Status effect stack data parameter.
        /// </summary>
        public List<StatusEffectStackData> ParamStatusEffects { get; set; }
        /// <summary>
        /// Vector3 parameter.
        /// </summary>
        public Vector3 ParamTimingDelays { get; set; }
        /// <summary>
        /// Trigger parameter.
        /// </summary>
        public CharacterTriggerData.Trigger ParamTrigger { get; set; }
        /// <summary>
        /// Which statusID to use to multiply the effect by.
        /// </summary>
        public string StatusEffectStackMultiplier { get; set; }
        /// <summary>
        /// Tooltips displayed when hovering over any game entity this effect is applied to.
        /// </summary>
        public List<AdditionalTooltipData> AdditionalTooltips { get; set; }
        public CharacterUI.Anim AnimToPlay { get; set; }
        public VfxAtLoc AppliedToSelfVFX { get; set; }
        public VfxAtLoc AppliedVFX { get; set; }
        public bool UseIntRange { get; set; }
        public bool UseStatusEffectStackMultiplier { get; set; }
        public bool CopyModifiersFromSource { get; set; }
        public bool FilterBasedOnMainSubClass { get; set; }
        public bool HideTooltip { get; set; }
        public bool IgnoreTemporaryModifiersFromSource { get; set; }
        public bool ShouldTest { get; set; }
        public CardEffectData.CardSelectionMode TargetCardSelectionMode { get; set; }
        public CardType TargetCardType { get; set; }
        public string TargetCharacterSubtype { get; set; }
        public bool TargetIgnoreBosses { get; set; }
        public bool TargetIgnorePyre { get; set; }
        public TargetMode TargetMode { get; set; }
        public CardEffectData.HealthFilter TargetModeHealthFilter { get; set; }
        public string[] TargetModeStatusEffectsFilter { get; set; }
        public Team.Type TargetTeamType { get; set; }

        public CardEffectDataBuilder()
        {
            TargetTeamType = Team.Type.Heroes;
            ShouldTest = true;
            ParamMultiplier = 1f;
            ParamStatusEffects = new List<StatusEffectStackData>();
            ParamCharacterDataPool = new List<CharacterData>();
            ParamCharacterDataPoolBuilder = new List<CharacterDataBuilder>();
            ParamTimingDelays = Vector3.zero;
            AdditionalTooltips = new List<AdditionalTooltipData>();
            TargetModeStatusEffectsFilter = Array.Empty<string>();
            ParamSubtype = VanillaSubtypeIDs.None;
            TargetCharacterSubtype = VanillaSubtypeIDs.None;
        }

        /// <summary>
        /// Builds the CardEffectData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created CardEffectData</returns>
        public CardEffectData Build()
        {
            if (EffectStateType == null)
            {
                throw new BuilderException("EffectStateType is required");
            }

            // Doesn't inherit from ScriptableObject
            CardEffectData cardEffectData = new CardEffectData();
            AccessTools.Field(typeof(CardEffectData), "additionalParamInt").SetValue(cardEffectData, AdditionalParamInt);
            AccessTools.Field(typeof(CardEffectData), "additionalTooltips").SetValue(cardEffectData, AdditionalTooltips.ToArray());
            AccessTools.Field(typeof(CardEffectData), "animToPlay").SetValue(cardEffectData, AnimToPlay);
            AccessTools.Field(typeof(CardEffectData), "appliedToSelfVFX").SetValue(cardEffectData, AppliedToSelfVFX);
            AccessTools.Field(typeof(CardEffectData), "appliedVFX").SetValue(cardEffectData, AppliedVFX);
            AccessTools.Field(typeof(CardEffectData), "copyModifiersFromSource").SetValue(cardEffectData, CopyModifiersFromSource);
            AccessTools.Field(typeof(CardEffectData), "effectStateName").SetValue(cardEffectData, EffectStateName);
            AccessTools.Field(typeof(CardEffectData), "filterBasedOnMainSubClass").SetValue(cardEffectData, FilterBasedOnMainSubClass);
            AccessTools.Field(typeof(CardEffectData), "hideTooltip").SetValue(cardEffectData, HideTooltip);
            AccessTools.Field(typeof(CardEffectData), "ignoreTemporaryModifiersFromSource").SetValue(cardEffectData, IgnoreTemporaryModifiersFromSource);
            AccessTools.Field(typeof(CardEffectData), "paramAdditionalCharacterData").SetValue(cardEffectData, ParamAdditionalCharacterData);
            AccessTools.Field(typeof(CardEffectData), "paramBool").SetValue(cardEffectData, ParamBool);
            AccessTools.Field(typeof(CardEffectData), "paramCardFilter").SetValue(cardEffectData, ParamCardFilter);
            AccessTools.Field(typeof(CardEffectData), "paramCardPool").SetValue(cardEffectData, ParamCardPool);
            AccessTools.Field(typeof(CardEffectData), "paramInt").SetValue(cardEffectData, ParamInt);
            AccessTools.Field(typeof(CardEffectData), "paramMaxInt").SetValue(cardEffectData, ParamMaxInt);
            AccessTools.Field(typeof(CardEffectData), "paramMinInt").SetValue(cardEffectData, ParamMinInt);
            AccessTools.Field(typeof(CardEffectData), "paramMultiplier").SetValue(cardEffectData, ParamMultiplier);
            AccessTools.Field(typeof(CardEffectData), "paramRoomData").SetValue(cardEffectData, ParamRoomData);
            AccessTools.Field(typeof(CardEffectData), "paramStatusEffects").SetValue(cardEffectData, ParamStatusEffects.ToArray());
            AccessTools.Field(typeof(CardEffectData), "paramStr").SetValue(cardEffectData, ParamStr);
            AccessTools.Field(typeof(CardEffectData), "paramSubtype").SetValue(cardEffectData, ParamSubtype);
            AccessTools.Field(typeof(CardEffectData), "paramTimingDelays").SetValue(cardEffectData, ParamTimingDelays);
            AccessTools.Field(typeof(CardEffectData), "paramTrigger").SetValue(cardEffectData, ParamTrigger);
            AccessTools.Field(typeof(CardEffectData), "shouldTest").SetValue(cardEffectData, ShouldTest);
            AccessTools.Field(typeof(CardEffectData), "statusEffectStackMultiplier").SetValue(cardEffectData, StatusEffectStackMultiplier);
            AccessTools.Field(typeof(CardEffectData), "targetCardSelectionMode").SetValue(cardEffectData, TargetCardSelectionMode);
            AccessTools.Field(typeof(CardEffectData), "targetCardType").SetValue(cardEffectData, TargetCardType);
            AccessTools.Field(typeof(CardEffectData), "targetCharacterSubtype").SetValue(cardEffectData, TargetCharacterSubtype);
            AccessTools.Field(typeof(CardEffectData), "targetIgnoreBosses").SetValue(cardEffectData, TargetIgnoreBosses);
            AccessTools.Field(typeof(CardEffectData), "targetMode").SetValue(cardEffectData, TargetMode);
            AccessTools.Field(typeof(CardEffectData), "targetModeHealthFilter").SetValue(cardEffectData, TargetModeHealthFilter);
            AccessTools.Field(typeof(CardEffectData), "targetModeStatusEffectsFilter").SetValue(cardEffectData, TargetModeStatusEffectsFilter);
            AccessTools.Field(typeof(CardEffectData), "targetTeamType").SetValue(cardEffectData, TargetTeamType);
            AccessTools.Field(typeof(CardEffectData), "useIntRange").SetValue(cardEffectData, UseIntRange);
            AccessTools.Field(typeof(CardEffectData), "useStatusEffectStackMultiplier").SetValue(cardEffectData, UseStatusEffectStackMultiplier);

            CharacterData characterData = ParamCharacterData;
            if (ParamCharacterDataBuilder != null)
            {
                characterData = ParamCharacterDataBuilder.BuildAndRegister();
            }

            AccessTools.Field(typeof(CardEffectData), "paramCharacterData").SetValue(cardEffectData, characterData);

            CardUpgradeData upgrade = ParamCardUpgradeData;
            if (ParamCardUpgradeDataBuilder != null)
            {
                upgrade = ParamCardUpgradeDataBuilder.Build();
            }
            AccessTools.Field(typeof(CardEffectData), "paramCardUpgradeData").SetValue(cardEffectData, upgrade);

            // Field not allocated.
            var characterDataPool = new List<CharacterData>();
            characterDataPool.AddRange(ParamCharacterDataPool);
            foreach (var character in ParamCharacterDataPoolBuilder)
            {
                characterDataPool.Add(character.BuildAndRegister());
            }
            AccessTools.Field(typeof(CardEffectData), "paramCharacterDataPool").SetValue(cardEffectData, characterDataPool);

            return cardEffectData;
        }
    }
}