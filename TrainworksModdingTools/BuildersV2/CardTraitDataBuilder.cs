﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using Trainworks.ConstantsV2;

namespace Trainworks.BuildersV2
{
    public class CardTraitDataBuilder
    {
        /// <summary>
        /// Type of the trait class to instantiate.
        /// This should be a class inheriting from CardTraitState
        /// </summary>
        public Type TraitStateType { get; set; }
        /// <summary>
        /// CardTraitState class to instantiate.
        /// Note that this isn't a simple string name of the class it is the class name plus the Assembly info if necessary.
        /// </summary>
        public string TraitStateName => BuilderUtils.GetEffectClassName(TraitStateType);
        /// <summary>
        /// CardData parameter.
        /// </summary>
        public CardData ParamCardData { get; set; }
        /// <summary>
        /// Type of card to target.
        /// </summary>
        public CardStatistics.CardTypeTarget ParamCardType { get; set; }
        /// <summary>
        /// Team to target.
        /// </summary>
        public Team.Type ParamTeamType { get; set; }
        /// <summary>
        /// Float parameter.
        /// </summary>
        public float ParamFloat { get; set; }
        /// <summary>
        /// Int parameter.
        /// </summary>
        public int ParamInt { get; set; }
        /// <summary>
        /// String parameter. Unfortunately this parameter isn't directly accessible in a Custom CardTraitState subclass.
        /// must be accesssed through the CardTraitData instance.
        /// </summary>
        public string ParamStr { get; set; }
        /// <summary>
        /// Subtype parameter.
        /// </summary>
        public string ParamSubtype { get; set; }
        /// <summary>
        /// Status effect array parameter.
        /// </summary>
        public List<StatusEffectStackData> ParamStatusEffects { get; set; }
        /// <summary>
        /// Entry Duration for retrieving a TrackedValue.
        /// Can be ThisBattle, ThisTurn, or PreviousTurn.
        /// Note that this isn't always meaningful during on the TrackedValue.
        /// </summary>
        public CardStatistics.EntryDuration ParamEntryDuration { get; set; }
        /// <summary>
        /// Type of TrackedValue to retrieve from CardStatistics.
        /// </summary>
        public CardStatistics.TrackedValueType ParamTrackedValue { get; set; }
        /// <summary>
        /// Use Scaling Params. This only seems to be set for Scaling Card Traits,
        /// however there's no references to the use of this field anywhere in the
        /// MT codebase so it appears optional. To reiterate setting this to true
        /// does nothing.
        /// </summary>
        public bool ParamUseScalingParams { get; set; }
        /// <summary>
        /// Card Upgrade Data Parameter. Unfortunately this parameter isn't directly accessible in a Custom CardTraitState subclass.
        /// must be accesssed through the CardTraitData instance.
        /// 
        /// The only usage of this Param is in Spellchain (CardTraitCopyOnPlay). Which applies an upgrade giving Purge and increasing the cost by 1.
        /// </summary>
        public CardUpgradeData ParamCardUpgradeData { get; set; }
        /// <summary>
        /// Convenience Card Upgrade Data Builder. Overrides ParamCardUpgradeData if set.
        /// </summary>
        public CardUpgradeDataBuilder ParamCardUpgradeDataBuilder { get; set; }
        /// <summary>
        /// Allows the trait to be removable.
        /// 
        /// As far as the base game this field is used for CardTraitCorruptRestricted (Extract)
        /// however the usage among cards with that specific trait is inconsistent.
        /// Only two cards in the game have it set to false Forced Contamination and Ambient Charge. 
        /// While all other Extract cards like Glugsider have it set to true (the default).
        /// 
        /// So???? shrug.
        /// </summary>
        public bool TraitIsRemovable { get; set; }
        /// <summary>
        /// An Enum that indicates how application of this CardTrait is handled.
        /// Either it can be ignored or each application increases ParamInt by 1.
        /// 
        /// If you are going to use StackMode.ParamInt then it works as expected on Nonpermanent 
        /// Card Traits (those were are not added via CardData.Traits)
        /// Use CardData.StartingUpgrades instead, if you need it on a card initially.
        /// Then using CardEffectAddTempCardUpgradeXXX card effects will work.
        /// Each time the trait is added to a card it will increase ParamInt by 1 
        /// StackMode.None's behaviour is to simply not allow the Trait to be added to the card multiple times.
        /// 
        /// The only use in the Base Game is CardTraitCopyOnPlay (Spellchain)
        /// However you can't apply the Spellchain upgrade on a card multiple times.
        /// 
        /// So???? shrug.
        /// </summary>
        public CardTraitData.StackMode StackMode { get; set; }

        public CardTraitDataBuilder()
        {
            ParamStatusEffects = new List<StatusEffectStackData>();
            ParamFloat = 1f;
            TraitIsRemovable = true;
            ParamTeamType = Team.Type.Heroes;
            ParamSubtype = VanillaSubtypeIDs.None;
        }

        /// <summary>
        /// Builds the CardTraitData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created CardTraitData</returns>
        public CardTraitData Build()
        {
            if (TraitStateType == null)
            {
                throw new BuilderException("TraitStateType is required");
            }

            // Doesn't inherit from ScriptableObject
            CardTraitData cardTraitData = new CardTraitData();
            AccessTools.Field(typeof(CardTraitData), "paramCardData").SetValue(cardTraitData, ParamCardData);
            AccessTools.Field(typeof(CardTraitData), "paramCardType").SetValue(cardTraitData, ParamCardType);
            AccessTools.Field(typeof(CardTraitData), "paramEntryDuration").SetValue(cardTraitData, ParamEntryDuration);
            AccessTools.Field(typeof(CardTraitData), "paramFloat").SetValue(cardTraitData, ParamFloat);
            AccessTools.Field(typeof(CardTraitData), "paramInt").SetValue(cardTraitData, ParamInt);
            AccessTools.Field(typeof(CardTraitData), "paramStatusEffects").SetValue(cardTraitData, ParamStatusEffects.ToArray());
            AccessTools.Field(typeof(CardTraitData), "paramStr").SetValue(cardTraitData, ParamStr);
            AccessTools.Field(typeof(CardTraitData), "paramSubtype").SetValue(cardTraitData, ParamSubtype);
            AccessTools.Field(typeof(CardTraitData), "paramTeamType").SetValue(cardTraitData, ParamTeamType);
            AccessTools.Field(typeof(CardTraitData), "paramTrackedValue").SetValue(cardTraitData, ParamTrackedValue);
            AccessTools.Field(typeof(CardTraitData), "paramUseScalingParams").SetValue(cardTraitData, ParamUseScalingParams);
            AccessTools.Field(typeof(CardTraitData), "traitIsRemovable").SetValue(cardTraitData, TraitIsRemovable);
            AccessTools.Field(typeof(CardTraitData), "traitStateName").SetValue(cardTraitData, TraitStateName);
            AccessTools.Field(typeof(CardTraitData), "stackMode").SetValue(cardTraitData, StackMode);
            var upgrade = ParamCardUpgradeData;
            if (ParamCardUpgradeDataBuilder != null)
            {
                upgrade = ParamCardUpgradeDataBuilder.Build();
            }

            AccessTools.Field(typeof(CardTraitData), "paramCardUpgradeData").SetValue(cardTraitData, upgrade);
            return cardTraitData;
        }
    }
}