using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    // Adopted from ThreeFishes's Equestrian Clan.
    public class CardUpgradeMaskDataBuilder
    {
        public string CardUpgradeMaskID { get; set; }
        public CardType CardType { get; set; }
        /// <summary>
        /// Note that these are subtype keys required, not the localized name.
        /// </summary>
        public List<string> RequiredSubtypes { get; set; }
        /// <summary>
        /// Note that these are subtype keys required, not the localized name.
        /// </summary>
        public List<string> ExcludedSubtypes { get; set; }
        /// <summary>
        /// Status Effect Requirement.
        /// </summary>
        public List<StatusEffectStackData> RequiredStatusEffects { get; set; }
        /// <summary>
        /// Status Effect Exclusion.
        /// </summary>
        public List<StatusEffectStackData> ExcludedStatusEffects { get; set; }
        /// <summary>
        /// Note if you are checking for a CardTrait in the base game feel free to use the exact class name of it.
        /// However if it is a custom CardTrait you will need to pass typeof(Class).AssemblyQualifiedName.
        /// </summary>
        public List<string> RequiredCardTraits { get; set; }
        /// <summary>
        /// Note if you are checking for a CardTrait in the base game feel free to use the exact class name of it.
        /// However if it is a custom CardTrait you will need to pass typeof(Class).AssemblyQualifiedName.
        /// </summary>
        public List<string> ExcludedCardTraits { get; set; }
        /// <summary>
        /// Note if you are checking for a CardEffect in the base game feel free to use the exact class name of it.
        /// However if it is a custom CardEffect you will need to pass typeof(Class).AssemblyQualifiedName.
        /// </summary>
        public List<string> RequiredCardEffects { get; set; }
        /// <summary>
        /// Note if you are checking for a CardEffect in the base game feel free to use the exact class name of it.
        /// However if it is a custom CardEffect you will need to pass typeof(Class).AssemblyQualifiedName.
        /// </summary>
        public List<string> ExcludedCardEffects { get; set; }
        /// <summary>
        /// If there are any cards in this pool, then only the cards present in the CardPools will be allowed.
        /// </summary>
        public List<CardPool> AllowedCardPools { get; set; }
        /// <summary>
        /// No cards in any of these CardPools will be allowed.
        /// </summary>
        public List<CardPool> DisallowedCardPools { get; set; }
        /// <summary>
        /// Size Requirement.
        /// </summary>
        public List<int> RequiredSizes { get; set; }
        /// <summary>
        /// Size Exclusion.
        /// </summary>
        public List<int> ExcludedSizes { get; set; }
        /// <summary>
        /// Cost Range cards whose cost does not fall into this range will be exlcuded.
        /// </summary>
        public Vector2 CostRange { get; set; }
        /// <summary>
        /// Excludes Characters whose getAttack field is set to false.
        /// </summary>
        public bool ExcludeNonAttackingMonsters { get; set; }
        /// <summary>
        /// Operator determines if we require all or at least one
        /// </summary>
        public CardUpgradeMaskDataBuilder.CompareOperator RequiredSubtypesOperator { get; set; }
        /// <summary>
        /// Operator determines if we require all or at least one
        /// </summary>
        public CardUpgradeMaskDataBuilder.CompareOperator ExcludedSubtypesOperator { get; set; }
        /// <summary>
        /// Operator determines if we require all or at least one
        /// </summary>
        public CardUpgradeMaskDataBuilder.CompareOperator RequiredStatusEffectsOperator { get; set; }
        /// <summary>
        /// Operator determines if we require all or at least one
        /// </summary>
        public CardUpgradeMaskDataBuilder.CompareOperator ExcludedStatusEffectsOperator { get; set; }
        /// <summary>
        /// Operator determines if we require all or at least one
        /// </summary>
        public CardUpgradeMaskDataBuilder.CompareOperator RequiredCardTraitsOperator { get; set; }
        /// <summary>
        /// Operator determines if we require all or at least one
        /// </summary>
        public CardUpgradeMaskDataBuilder.CompareOperator ExcludedCardTraitsOperator { get; set; }
        /// <summary>
        /// Operator determines if we require all or at least one
        /// </summary>
        public CardUpgradeMaskDataBuilder.CompareOperator RequiredCardEffectsOperator { get; set; }
        /// <summary>
        /// Operator determines if we require all or at least one
        /// </summary>
        public CardUpgradeMaskDataBuilder.CompareOperator ExcludedCardEffectsOperator { get; set; }
        /// <summary>
        /// Allows the upgrade to be only applied to X cost cards.
        /// </summary>
        public bool RequireXCost { get; set; }
        /// <summary>
        /// Disallows X cost cards from receiving the upgrade.
        /// </summary>
        public bool ExcludeXCost { get; set; }
        /// <summary>
        /// This is the reason why a card is filtered away from having this upgrade applied to it
        /// </summary>
        public CardState.UpgradeDisabledReason UpgradeDisabledReason { get; set; }

        public CardUpgradeMaskDataBuilder()
        {
            CardType = CardType.Invalid;
            RequiredSubtypes = new List<string>();
            ExcludedSubtypes = new List<string>();
            RequiredStatusEffects = new List<StatusEffectStackData>();
            ExcludedStatusEffects = new List<StatusEffectStackData>();
            RequiredCardTraits = new List<string>();
            ExcludedCardTraits = new List<string>();
            RequiredCardEffects = new List<string>();
            ExcludedCardEffects = new List<string>();
            AllowedCardPools = new List<CardPool>();
            DisallowedCardPools = new List<CardPool>();
            RequiredSizes = new List<int>();
            ExcludedSizes = new List<int>();
            CostRange = new Vector2(0f, 99f);
        }

        /// <summary>
        /// Builds the CardUpgradeMaskData represented by this builders's parameters recursively;
        /// </summary>
        /// <returns>The newly created CardUpgradeMaskData</returns>
        public CardUpgradeMaskData Build()
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (CardUpgradeMaskID == null)
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Warning should provide a CardUpgradeMaskDataID.");
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Stacktrace: " + Environment.StackTrace);
            }

            CardUpgradeMaskData cardUpgradeMaskData = ScriptableObject.CreateInstance<CardUpgradeMaskData>();
            cardUpgradeMaskData.name = CardUpgradeMaskID;
            Type realEnumType = AccessTools.Inner(typeof(CardUpgradeMaskData), "CompareOperator");
            AccessTools.Field(typeof(CardUpgradeMaskData), "allowedCardPools").SetValue(cardUpgradeMaskData, AllowedCardPools);
            AccessTools.Field(typeof(CardUpgradeMaskData), "cardType").SetValue(cardUpgradeMaskData, CardType);
            AccessTools.Field(typeof(CardUpgradeMaskData), "costRange").SetValue(cardUpgradeMaskData, CostRange);
            AccessTools.Field(typeof(CardUpgradeMaskData), "disallowedCardPools").SetValue(cardUpgradeMaskData, DisallowedCardPools);
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedCardEffects").SetValue(cardUpgradeMaskData, ExcludedCardEffects);
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedCardEffectsOperator").SetValue(cardUpgradeMaskData, Enum.ToObject(realEnumType, ExcludedCardEffectsOperator));
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedCardTraits").SetValue(cardUpgradeMaskData, ExcludedCardTraits);
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedCardTraitsOperator").SetValue(cardUpgradeMaskData, Enum.ToObject(realEnumType, ExcludedCardTraitsOperator));
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedSizes").SetValue(cardUpgradeMaskData, ExcludedSizes);
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedStatusEffects").SetValue(cardUpgradeMaskData, ExcludedStatusEffects);
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedStatusEffectsOperator").SetValue(cardUpgradeMaskData, Enum.ToObject(realEnumType, ExcludedStatusEffectsOperator));
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedSubtypes").SetValue(cardUpgradeMaskData, ExcludedSubtypes);
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedSubtypesOperator").SetValue(cardUpgradeMaskData, Enum.ToObject(realEnumType, ExcludedSubtypesOperator));
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludeNonAttackingMonsters").SetValue(cardUpgradeMaskData, ExcludeNonAttackingMonsters);
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludeXCost").SetValue(cardUpgradeMaskData, ExcludeXCost);
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredCardEffects").SetValue(cardUpgradeMaskData, RequiredCardEffects);
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredCardEffectsOperator").SetValue(cardUpgradeMaskData, Enum.ToObject(realEnumType, RequiredCardEffectsOperator));
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredCardTraits").SetValue(cardUpgradeMaskData, RequiredCardTraits);
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredCardTraitsOperator").SetValue(cardUpgradeMaskData, Enum.ToObject(realEnumType, RequiredCardTraitsOperator));
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredSizes").SetValue(cardUpgradeMaskData, RequiredSizes);
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredStatusEffects").SetValue(cardUpgradeMaskData, RequiredStatusEffects);
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredStatusEffectsOperator").SetValue(cardUpgradeMaskData, Enum.ToObject(realEnumType, RequiredStatusEffectsOperator));
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredSubtypes").SetValue(cardUpgradeMaskData, RequiredSubtypes);
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredSubtypesOperator").SetValue(cardUpgradeMaskData, Enum.ToObject(realEnumType, RequiredSubtypesOperator));
            AccessTools.Field(typeof(CardUpgradeMaskData), "requireXCost").SetValue(cardUpgradeMaskData, RequireXCost);
            AccessTools.Field(typeof(CardUpgradeMaskData), "upgradeDisabledReason").SetValue(cardUpgradeMaskData, UpgradeDisabledReason);
            return cardUpgradeMaskData;
        }

        public enum CompareOperator
        {
            And,
            Or
        }
    }
}