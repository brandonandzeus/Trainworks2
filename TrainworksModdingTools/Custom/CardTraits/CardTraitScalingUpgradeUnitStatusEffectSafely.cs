using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.CustomCardTraits
{
    /// <summary>
    /// Card Trait that handles scaling a Unit's status effects, safely.
    /// This CardTrait should only be used on Cards which spawn units.
    /// The issue with using a CardTrait on a Unit card which scales is that it indiscrimanitely scales
    /// *every* upgrade the unit applies to itself (or others), which may not be the intended effect.
    /// 
    /// The famous example Kinhost Carapace + Heaven's Aid (Revenge: +1 Attack) buff.
    /// Since the Trigger added by the Heaven's Aid upgrade applies a card upgrade to units,
    /// the upgrade is modified by the Card Trait on Kinhost Carapace.
    /// 
    /// 
    /// Note that Spell cards which applies upgrades are unaffected since the CardTraits on the played
    /// card modify the upgrade applied, not the Card traits on the unit on which the upgrade is applied to.
    /// 
    /// Required Params:
    ///     ParamTrackedValue - TrackedValue statistic to use.
    ///     ParamEntryDuration - Duration for the TrackedValue statistic.
    ///     ParamInt - Size multiplier applied to the tracked value and added to the upgrade.
    ///     ParamCardUpgradeData - CardUpgradeData that this card trait should apply to.
    ///     ParamStatusEffects - Status Effects to add. Note that if the status effect is not present in the upgrade it will be added.
    /// </summary>
    public sealed class CardTraitScalingUpgradeUnitStatusEffectSafely : CardTraitState
    {
        public override void OnApplyingCardUpgradeToUnit(CardState thisCard, CharacterState targetUnit, CardUpgradeState upgradeState, CardManager cardManager)
        {
            if (GetCardTraitData().GetCardUpgradeDataParam().GetAssetKey() != upgradeState.GetAssetName())
            {
                return;
            }
            int additionalStacks = GetAdditionalStacks(cardManager.GetCardStatistics());
            StatusEffectStackData[] array = GetParamStatusEffects();
            foreach (StatusEffectStackData statusEffectStackData in array)
            {
                upgradeState.AddStatusEffectUpgradeStacks(statusEffectStackData.statusId, additionalStacks);
            }
        }

        private int GetAdditionalStacks(CardStatistics cardStatistics)
        {
            CardStatistics.StatValueData statValueData = default(CardStatistics.StatValueData);
            statValueData.cardState = GetCard();
            statValueData.trackedValue = GetParamTrackedValue();
            statValueData.entryDuration = GetParamEntryDuration();
            statValueData.cardTypeTarget = GetParamCardType();
            statValueData.paramSubtype = GetParamSubtype();
            CardStatistics.StatValueData statValueData2 = statValueData;
            int statValue = cardStatistics.GetStatValue(statValueData2);
            return GetParamInt() * statValue;
        }

        public override string GetCurrentEffectText(CardStatistics cardStatistics, SaveManager saveManager, RelicManager relicManager)
        {
            if (cardStatistics != null && cardStatistics.GetStatValueShouldDisplayOnCardNow(base.StatValueData))
            {
                return string.Format("CardTraitScalingAddStatusEffect_CurrentScaling_CardText".Localize(), GetAdditionalStacks(cardStatistics));
            }
            return string.Empty;
        }
    }

}
