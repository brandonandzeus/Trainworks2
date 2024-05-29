using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.CustomCardTraits
{
    /// <summary>
    /// Card Trait that handles scaling a Unit's size, safely.
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
    ///     ParamTrackedValue - TrackedValue statistic to use
    ///     ParamEntryDuration - Duration for the TrackedValue statistic.
    ///     ParamInt - Size multiplier applied to the tracked value and added to the upgrade.
    ///     ParamCardUpgradeData - CardUpgradeData that this card trait should apply to.
    /// </summary>
    public sealed class CardTraitScalingUpgradeUnitSizeSafely : CardTraitState
    {
        public override void OnApplyingCardUpgradeToUnit(CardState thisCard, CharacterState targetUnit, CardUpgradeState upgradeState, CardManager cardManager)
        {
            if (GetCardTraitData().GetCardUpgradeDataParam().GetAssetKey() != upgradeState.GetAssetName())
            {
                return;
            }
            int additionalSize = upgradeState.GetAdditionalSize();
            int additionalSize2 = GetAdditionalSize(cardManager.GetCardStatistics(), setForPreviewText: false);
            upgradeState.SetAdditionalSize(additionalSize + additionalSize2);
        }

        private int GetAdditionalSize(CardStatistics cardStatistics, bool setForPreviewText)
        {
            CardStatistics.StatValueData statValueData = default(CardStatistics.StatValueData);
            statValueData.cardState = GetCard();
            statValueData.trackedValue = GetParamTrackedValue();
            statValueData.entryDuration = GetParamEntryDuration();
            statValueData.cardTypeTarget = GetParamCardType();
            statValueData.paramSubtype = GetParamSubtype();
            statValueData.forPreviewText = setForPreviewText;
            CardStatistics.StatValueData statValueData2 = statValueData;
            int statValue = cardStatistics.GetStatValue(statValueData2);
            return GetParamInt() * statValue;
        }

        public override string GetCurrentEffectText(CardStatistics cardStatistics, SaveManager saveManager, RelicManager relicManager)
        {
            if (cardStatistics != null && cardStatistics.GetStatValueShouldDisplayOnCardNow(base.StatValueData))
            {
                return string.Format("CardTraitScalingUpgradeUnitSize_CurrentScaling_CardText".Localize(), GetAdditionalSize(cardStatistics, setForPreviewText: true));
            }
            return string.Empty;
        }
    }
}
