﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.Custom.CardEffects
{
    /// <summary>
    /// Generic version of CardEffectModifyCardCost. The original version was hardcoded to target the Hand.
    /// 
    /// Params:
    ///    ParamInt: Cost modification. Positive will increase. Negative decreases.
    ///    ParamBool: True to make the cost modification permanent, false for temporary.
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
                    if (permanent)
                    {
                        item.Upgrade(cardUpgradeState, cardEffectParams.saveManager);
                    }
                    else
                    {
                        item.GetTemporaryCardStateModifiers().AddUpgrade(cardUpgradeState);
                    }
                }
            }
            yield break;
        }
    }

}
