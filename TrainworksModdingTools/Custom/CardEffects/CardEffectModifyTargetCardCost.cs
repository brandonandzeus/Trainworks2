using System.Collections;

namespace Trainworks.Custom.CardEffects
{
    /// <summary>
    /// Generic version of CardEffectModifyCardCost. The original version was hardcoded to target the Hand.
    /// 
    /// Params:
    ///    ParamInt: Cost modification. Positive will increase. Negative decreases.
    ///    TargetCardType: Cards type to restrict target cards to. Defaults to Spell Cards. Set to CardType.Invalid to disable this filter.
    /// </summary>
    public sealed class CardEffectModifyTargetCardCost : CardEffectBase
    {
        public override bool CanApplyInPreviewMode => false;

        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            bool permanent = cardEffectState.GetParamBool();

            CardUpgradeState cardUpgradeState = new CardUpgradeState();
            cardUpgradeState.Setup();
            cardUpgradeState.SetCostReduction(-cardEffectState.GetParamInt());
            cardUpgradeState.SetXCostReduction(-cardEffectState.GetParamInt());
            foreach (CardState item in cardEffectParams.targetCards)
            {
                if (item.GetCardType() == cardEffectState.GetTargetCardType() || cardEffectState.GetTargetCardType() == CardType.Invalid)
                {
                    item.GetTemporaryCardStateModifiers().AddUpgrade(cardUpgradeState);
                }
            }
            yield break;
        }
    }

}
