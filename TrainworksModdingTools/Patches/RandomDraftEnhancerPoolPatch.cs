using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

// This patch fixes ClassData#RandomDraftEnhancerPool. Only Wurmkin sets it and the devs did not consider the case where the other clan has it set too.
namespace Trainworks.Patches
{
    [HarmonyPatch(typeof(CardDraftScreen), nameof(CardDraftScreen.Setup))]
    public class RandomDraftEnhancerPoolPatch
    {
        public static void Postfix(bool fromRewardNode, CardStatistics ___cardStatistics, SaveManager ___saveManager, MonsterManager ___monsterManager, HeroManager ___heroManager, RelicManager ___relicManager, List<IDraftableUI> ___draftItems)
        {
            ClassData mainClass = ___saveManager.GetMainClass();
            ClassData subClass = ___saveManager.GetSubClass();
            EnhancerPool enhancerPool2 = mainClass.GetRandomDraftEnhancerPool();
            EnhancerPool enhancerPool = subClass.GetRandomDraftEnhancerPool();

            // If one Enhancer Pool or the clans are the same exit.
            if (enhancerPool2 == null || enhancerPool == null || mainClass == subClass)
            {
                return;
            }

            // The Primary and secondary clan has an EnhancerPool set
            // So apply the secondary clan's Enhancer Effect to a random card.
            List<int> list = null;
            int numCards = ___draftItems.Count;
            if (numCards > 0)
            {
                int num = Mathf.Min(___saveManager.GetNumCardsToApplyRandomDraftEnhancerTo(), numCards);
                list = new List<int>(num);
                for (int i = 0; i < numCards; i++)
                {
                    list.Add(i);
                }
                list.Shuffle(RngId.Rewards);
                list.RemoveRange(num - 1, numCards - num);
            }
            for (int j = 0; j < numCards; j++)
            {
                CardUI cardUI = (CardUI)___draftItems[j];
                CardState cardState = cardUI.GetCardState();
                if (list != null && list.Contains(j) && (!fromRewardNode || !cardState.HasTrait(typeof(CardTraitLevelMonsterState))))
                {
                    CardUpgradeData paramCardUpgradeData = enhancerPool.GetAllChoices().RandomElement(RngId.Rewards).GetEffects()[0].GetParamCardUpgradeData();
                    CardUpgradeState cardUpgradeState = Activator.CreateInstance<CardUpgradeState>();
                    cardUpgradeState.Setup(paramCardUpgradeData);
                    cardState.Upgrade(cardUpgradeState, ___saveManager, ignoreUpgradeAnimation: true);
                    // This is to refresh the card in the UI.
                    cardUI.ApplyStateToUI(cardState, ___cardStatistics, ___monsterManager, ___heroManager, ___relicManager, ___saveManager, ___saveManager.GetMastery(cardState));
                }
            }
        }
    }
}
