using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Trainworks.Enums;
using UnityEngine;
using static DiscordRpc;
using static PyreArmorUI;
using static ShinyShoe.Loading.SaveDataFileTaskBase;

namespace Trainworks.Custom.CardEffects
{
    /// <summary>
    /// A genericized version of CardEffectRandomDiscard. It's kinda like CardEffectRecursion except stuff goes in trash.
    /// Allows you to move cards from anywhere to the discard. If moving cards from the Consume or Eaten piles the cards
    /// are restored (which adds to deck), then drawn, then discarded.
    /// 
    /// Params:
    ///   TargetCardType: (Required) Target card filter. Note default is Spell cards only. Set to CardType.Invalid to disable filtering.
    ///   TargetCharacterSubtype: If TargetCardType is Monster additional filtering based on subtype.The default is the None subtype which matches all.
    ///   ParamCardUpgrade: CardUpgrade to apply.Note that CardUpgradeMaskData filters can be applied to restrict the upgrade from hitting non matching cards.
    ///   ParamBool: If true the CardUpgrade is applied permanently.
    /// </summary>
    public sealed class CardEffectRandomDiscardFromCardPile : CardEffectBase
    {
        public override bool CanPlayAfterBossDead => false;
        public override bool CanApplyInPreviewMode => false;

        // Small delay before discarding drawn cards.
        private static readonly float DelayBeforeDiscard = 0.1f;
        // Unfortunately these timings aren't specified in the AnimationTiming class.
        private static readonly float RestoreDelayBeforeDiscard = 0.55f;

        public static readonly List<TargetMode> HandTargets = new List<TargetMode>
        {
            TargetMode.Hand,
            TargetMode.LastDrawnCard,
            /*
            TargetMode.LastDrawnCard.Invert(),
            // Same as hand.pretty much
            TargetModes.PlayedCard.Invert()
            */
        };
        private readonly List<CardState> targets = new List<CardState>();
        private readonly List<CardState> selected = new List<CardState>();

        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            if (TargetsHand(cardEffectState.GetTargetMode()))
            {
                switch (cardEffectParams.cardManager.GetNumCardsInHand())
                {
                    case 0:
                        return false;
                    case 1:
                        return cardEffectParams.cardManager.GetHand()[0] != cardEffectParams.playedCard;
                    default:
                        return true;
                };
            }
            else
            {
                return cardEffectParams.targetCards.Count > 0;
            }
        }

        public static bool TargetsHand(TargetMode targetMode)
        {
            return HandTargets.Contains(targetMode);
        }

        public HandUI.DrawSource GetDrawSource(TargetMode targetMode)
        {
            switch(targetMode)
            {
                case TargetMode.Discard:
                    return HandUI.DrawSource.Discard;
                // Consume/Eaten will go back to deck.
                case TargetMode.Exhaust:
                    return HandUI.DrawSource.Consume;
                case TargetMode.Eaten:
                    return HandUI.DrawSource.Eaten;
                default:
                    return HandUI.DrawSource.Deck;
            }
        }

        public float GetDrawDelay(TargetMode targetMode, BalanceData.AnimationTimingData timings)
        {
            switch(targetMode)
            {
                case TargetMode.Discard:
                    return timings.cardDrawAnimationDuration;
                case TargetMode.Eaten:
                case TargetMode.Exhaust:

                    return timings.cardConsumeReturnAnimationDuration + timings.cardDrawAnimationDuration + RestoreDelayBeforeDiscard;
                default:
                    return timings.cardDrawAnimationDuration;
            }
        }

        private void PickRandomCards(List<CardState> allCards, CardType filterToCardType, CardState playedCard, SubtypeData subtypeData)
        {
            targets.Clear();
            foreach (CardState allCard in allCards)
            {
                bool flag = true;
                CharacterData spawnCharacterData = allCard.GetSpawnCharacterData();
                if (allCard == playedCard)
                {
                    continue;
                }
                if (spawnCharacterData != null && subtypeData != null && !subtypeData.IsNone)
                {
                    flag = spawnCharacterData.GetSubtypes()?.Contains(subtypeData) ?? false;
                }
                if (flag && filterToCardType != CardType.Invalid)
                {
                    flag = allCard.GetCardType() == filterToCardType;
                }
                if (flag)
                {
                    targets.Add(allCard);
                }
            }
            targets.Shuffle(RngId.Battle);
        }

        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            var sourceCardState = cardEffectState.GetParentCardState() ?? cardEffectParams.cardTriggeredCharacter?.GetSpawnerCard() ?? cardEffectParams.selfTarget?.GetSpawnerCard();
            PickRandomCards(cardEffectParams.targetCards, cardEffectState.GetTargetCardType(), sourceCardState, cardEffectState.GetTargetCharacterSubtype());            
            int num = Math.Min(cardEffectState.GetIntInRange(), targets.Count);

            selected.Clear();
            for (int i = 0; i < num; i++)
            {
                selected.Add(targets[i]);
            }

            if (!TargetsHand(cardEffectState.GetTargetMode()))
            {
                for (int num3 = 0; num3 <= num - 1; num3++)
                {
                    var card = selected[num3];
                    cardEffectParams.cardManager.RestoreExhaustedOrEatenCard(card);
                    ApplyCardUpgrade(cardEffectState, card, cardEffectParams.cardManager, cardEffectParams.relicManager, cardEffectParams.saveManager);
                    cardEffectParams.cardManager.DrawSpecificCard(card, 0f, GetDrawSource(cardEffectState.GetTargetMode()), sourceCardState, num3, num);
                }
                // Wait before doing the discard, so that the player sees the cards.
                yield return CoreUtil.WaitForSecondsOrBreak(GetDrawDelay(cardEffectState.GetTargetMode(), cardEffectParams.allGameData.GetBalanceData().GetAnimationTimingData()) + DelayBeforeDiscard);
            }
            else
            {
                foreach (var card in selected)
                {
                    ApplyCardUpgrade(cardEffectState, card, cardEffectParams.cardManager, cardEffectParams.relicManager, cardEffectParams.saveManager);
                }
            }

            float effectDelay = cardEffectParams.allGameData.GetBalanceData().GetAnimationTimingData().cardEffectDiscardAnimationDelay;
            CardManager.DiscardCardParams discardCardParams = new CardManager.DiscardCardParams();
            foreach (CardState item in selected)
            {
                discardCardParams.effectDelay = effectDelay;
                discardCardParams.discardCard = item;
                discardCardParams.triggeredByCard = true;
                discardCardParams.triggeredCard = sourceCardState;
                discardCardParams.wasPlayed = false;
                yield return cardEffectParams.cardManager.DiscardCard(discardCardParams);
            }
        }

        private void ApplyCardUpgrade(CardEffectState cardEffectState, CardState card, CardManager cardManager, RelicManager relicManager, SaveManager saveManager)
        {
            var cardUpgradeData = cardEffectState.GetParamCardUpgradeData();
            if (cardUpgradeData == null)
            {
                return;
            }
            foreach (CardUpgradeMaskData filter in cardUpgradeData.GetFilters())
            {
                if (!filter.FilterCard(card, relicManager))
                {
                    return;
                }
            }
            CardUpgradeState cardUpgradeState = new CardUpgradeState();
            cardUpgradeState.Setup(cardUpgradeData);

            foreach (CardTraitState traitState in cardEffectState.GetParentCardState().GetTraitStates())
            {
                if (!traitState.OnCardBeingUpgraded(card, cardEffectState.GetParentCardState(), cardManager, cardUpgradeState))
                {
                    return;
                }
            }
            if (cardEffectState.GetParamBool())
            {
                // The additional steps seen below are handled in CardState.Upgrade.
                card.Upgrade(cardUpgradeState, saveManager);
            }
            else
            {
                cardUpgradeState.SetAttackDamage(cardUpgradeState.GetAttackDamage() * card.GetMagicPowerMultiplierFromTraits());
                cardUpgradeState.SetAdditionalHeal(cardUpgradeState.GetAdditionalHeal() * card.GetMagicPowerMultiplierFromTraits());
                card.GetTemporaryCardStateModifiers().AddUpgrade(cardUpgradeState);
                card.UpdateCardBodyText();
            }
        }

    }
}
