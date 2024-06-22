using System.Collections;
using System.Collections.Generic;

namespace Trainworks.Custom.CardEffects
{
    /// <summary>
    /// Like CardEffectAddTempCardUpgradeToCardsInHand, but uses TargetMode to select which cards get upgraded.
    /// Further filtering can be done with the CardUpgrade's UpgradeMaskData. Additionally this CardEffect
    /// allows you to permanently upgrade the cards.
    /// 
    /// This card effect doesn't suffer the limitations of the afforementioned CardEffect, it can be used in
    /// Charactere's Trigger effects.
    /// 
    /// Parameters:
    ///   ParamCardUpgrade: CardUpgrade to apply. The filters in the CardUpgrade further filter from TargetCards.
    ///   ParamBool: False for temporary upgrade. True for permanent upgrade.
    /// </summary>
    public sealed class CardEffectAddCardUpgradeToTargetCards : CardEffectBase, ICardEffectTipTooltip
    {
        public override bool CanPlayAfterBossDead => false;

        public override bool CanApplyInPreviewMode => false;

        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            foreach (CardState item in cardEffectParams.targetCards)
            {
                // CardUpgradeMask filtering
                bool flag = false;
                foreach (CardUpgradeMaskData filter in cardEffectState.GetParamCardUpgradeData().GetFilters())
                {
                    if (!filter.FilterCard(item, cardEffectParams.relicManager))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    continue;
                }

                var sourceCard = cardEffectState.GetParentCardState() ?? cardEffectParams.selfTarget.GetSpawnerCard();
                CardUpgradeState cardUpgradeState = new CardUpgradeState();
                cardUpgradeState.Setup(cardEffectState.GetParamCardUpgradeData());

                // The card trait can reject the card upgrade too.
                // But this also can modify the card upgrade so make sure the upgrade is clean.
                flag = false;
                foreach (CardTraitState traitState in sourceCard.GetTraitStates())
                {
                    if (!traitState.OnCardBeingUpgraded(item, sourceCard, cardEffectParams.cardManager, cardUpgradeState))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    continue;
                }

                if (cardEffectState.GetParamBool())
                {
                    // The additional steps seen below are handled in CardState.Upgrade.
                    item.Upgrade(cardUpgradeState, cardEffectParams.saveManager);
                }
                else
                {
                    cardUpgradeState.SetAttackDamage(cardUpgradeState.GetAttackDamage() * item.GetMagicPowerMultiplierFromTraits());
                    cardUpgradeState.SetAdditionalHeal(cardUpgradeState.GetAdditionalHeal() * item.GetMagicPowerMultiplierFromTraits());
                    item.GetTemporaryCardStateModifiers().AddUpgrade(cardUpgradeState);
                    item.UpdateCardBodyText();
                }

                if (cardEffectParams.cardManager?.GetCardInHand(item) != null)
                {
                    cardEffectParams.cardManager?.RefreshCardInHand(item, cleanupTweens: false);
                    cardEffectParams.cardManager.GetCardInHand(item)?.ShowEnhanceFX();
                }
            }
            yield break;
        }

        public override void GetTooltipsStatusList(CardEffectState cardEffectState, ref List<string> outStatusIdList)
        {
            CardEffectAddTempCardUpgradeToCardsInHand.GetTooltipsStatusList(cardEffectState.GetSourceCardEffectData(), ref outStatusIdList);
        }

        public string GetTipTooltipKey(CardEffectState cardEffectState)
        {
            if (cardEffectState.GetParamCardUpgradeData() != null && cardEffectState.GetParamCardUpgradeData().HasUnitStatUpgrade())
            {
                return "TipTooltip_StatChangesStick";
            }
            return null;
        }
    }

}
