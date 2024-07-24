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
        /// 
        /// This isn't set by any cards in the base game. 
        /// Technically used by CardEffectAddBattle/RunCard
        /// If set, this parameter interacts with ParamCardPool
        /// and will filter cards out of the CardPool specified.
        /// see: CardEffectData.GetFilteredCardListFromPool
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
        /// Convenience Builder for CharacterDataPool parameter. This will be appended to CharacterDataPool when built.
        /// </summary>
        public List<CharacterDataBuilder> ParamCharacterDataPoolBuilder { get; set; }
        /// <summary>
        /// RoomData parameter.
        /// Note: Not useful and not used by any cards. All cards in the game specify rooms via their index with ParamInt.
        /// </summary>
        public RoomData ParamRoomData { get; set; }
        /// <summary>
        /// Status effect stack data parameter.
        /// </summary>
        public List<StatusEffectStackData> ParamStatusEffects { get; set; }
        /// <summary>
        /// Timing Delays. It is a Vector3 with x, y, z being the delay (in seconds) in Normal, Fast, Ultra speeds respectively.
        /// The timing delay happens before this effect is applied.
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
        /// <summary>
        /// Specifies the Character's animation to play when hit with this CardEFfect.
        /// </summary>
        public CharacterUI.Anim AnimToPlay { get; set; }
        public VfxAtLoc AppliedToSelfVFX { get; set; }
        public VfxAtLoc AppliedVFX { get; set; }
        /// <summary>
        /// Specifies to use a range over ParamInt.
        /// If set then ParamMinInt and ParamMaxInt needs to be set.
        /// </summary>
        public bool UseIntRange { get; set; }
        /// <summary>
        /// Use Status Effect Stack Multiplier. Indicates that the effect's power should be multiplied by the targets status effect stack count.
        /// This is not commonly used. The only effect that uses it is Devourer of Death.
        /// </summary>
        public bool UseStatusEffectStackMultiplier { get; set; }
        /// <summary>
        /// Used by CardEffectAddBattleCard and CardEffectAddRunCard
        /// Allows the cards generated from those effects will copy the modifiers from this card.
        /// </summary>
        public bool CopyModifiersFromSource { get; set; }
        /// <summary>
        /// Only used by CardEffectAddBattleCard.
        /// Not used by any cards in the base game.
        /// Appears to filter out Cards from the CardPool specified in the card effect to only those in your Main/Sub clan.
        /// </summary>
        public bool FilterBasedOnMainSubClass { get; set; }
        /// <summary>
        /// Hides the CardEffects tooltips.
        /// </summary>
        public bool HideTooltip { get; set; }
        /// <summary>
        /// Used by CardEffectAddBattleCard and CardEffectAddUpgradedCopy.
        /// Ignore temporary CardModifiers from this card.
        /// Appears to not be used by any CardEffects in game.
        /// </summary>
        public bool IgnoreTemporaryModifiersFromSource { get; set; }
        /// <summary>
        /// Indicates if the effect should be tested if it hits a valid target.
        /// This is needed if the card targets multiple different targets (such as a Character and then the Deck).
        /// Defaults to true.
        /// </summary>
        public bool ShouldTest { get; set; }
        /// <summary>
        /// Used only by CardEffectRecursion.
        /// Parameter to CardEffectRecursion to specify what cards are targetted by its effect.
        /// </summary>
        public CardEffectData.CardSelectionMode TargetCardSelectionMode { get; set; }
        /// <summary>
        /// Used to filter out Cards of a specific card type in Certain Card targetting CardEffects.
        /// (Defaults to Spell).
        /// </summary>
        public CardType TargetCardType { get; set; }
        /// <summary>
        /// Used to filter out target characters without the specified subtype.
        /// </summary>
        public string TargetCharacterSubtype { get; set; }
        /// <summary>
        /// Used to indicate that a boss character can't be targetted by this card effect.
        /// </summary>
        public bool TargetIgnoreBosses { get; set; }
        /// <summary>
        /// Specifies that the Pyre can't be targetted by this CardEffect.
        /// </summary>
        public bool TargetIgnorePyre { get; set; }
        /// <summary>
        /// TargetMode, specifies the Target of this CardEffect.
        /// This along with TargetTeamType specifies Characters to target.
        /// </summary>
        public TargetMode TargetMode { get; set; }
        /// <summary>
        /// Used to filter out targets to just those that matches a health condition.
        /// </summary>
        public CardEffectData.HealthFilter TargetModeHealthFilter { get; set; }
        /// <summary>
        /// Used to filter out targets to just those that have a particular status id.
        /// Example Lady of the Reformed applying burnout (emphemeral) to burnout units.
        /// </summary>
        public string[] TargetModeStatusEffectsFilter { get; set; }
        /// <summary>
        /// Target Team Type. This along with TargetMode will specify Characters to target.
        /// Team.Type is an Enum Flag meaning you can specify both Heroes and Monsters by simply using bitwise OR.
        /// Note that Team.Type.Heroes are the enemies and Team.Type.Monsters are the player's units.
        /// </summary>
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
            // TODO don't allocate if it isn't needed.
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