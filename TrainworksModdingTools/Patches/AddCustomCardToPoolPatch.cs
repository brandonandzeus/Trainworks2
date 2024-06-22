﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trainworks.Managers;

// TODO Verify that these patches are sufficient.
// Accessing a CardPool by indices will not work with these patches since the cards aren't actually added.
// However implementing patches for random access will cause all Custom Cards to load at startup which
// increases run startup time by 200% (+1.2 seconds).
namespace Trainworks.Patches
{
    /// <summary>
    /// Adds custom cards to their appropriate pools.
    /// This one is used particularly when choosing for a card reward.
    /// </summary>
    [HarmonyPatch(typeof(CardPoolHelper), "GetCardsForClass")]
    [HarmonyPatch(new Type[] { typeof(CardPool), typeof(ClassData), typeof(int), typeof(CollectableRarity), typeof(SaveManager), typeof(CardPoolHelper.RarityCondition), typeof(bool) })]
    class AddCustomCardToPoolPatch
    {
        static void Postfix(ref List<CardData> __result, ref CardPool cardPool, ClassData classData, int classLevel, CollectableRarity paramRarity, CardPoolHelper.RarityCondition rarityCondition, bool testRarityCondition)
        {
            List<CardData> customCardsToAddToPool = CustomCardPoolManager.GetCardsForPoolSatisfyingConstraints(cardPool.name, classData, paramRarity, rarityCondition, testRarityCondition);
            __result.AddRange(customCardsToAddToPool);
            __result.RemoveAll((CardData card) => card.GetUnlockLevel() > classLevel);
        }
    }

    /// <summary>
    /// Adds custom cards to their appropriate pools.
    /// This one is used particularly when pulling cards from card effect mid-battle.
    /// </summary>
    [HarmonyPatch]
    class AddCustomCardToPoolPatch2
    {
        static MethodBase TargetMethod()
        {
            var methods = typeof(CardEffectState)
                .GetMethods()
                .Where(method => method.Name == "GetFilteredCardListFromPool")
                .Where(method => !method.IsStatic)
                .Cast<MethodBase>();
            return methods.First();
        }

        static void Postfix(ref bool __result, ref CardPool ___paramCardPool, CardUpgradeMaskData ___paramCardFilter, RelicManager relicManager, ref List<CardData> toProcessCards)
        {
            List<CardData> customCardsToAddToPool = CustomCardPoolManager.GetCardsForPoolSatisfyingConstraints(___paramCardPool.name, ___paramCardFilter, relicManager);
            toProcessCards.AddRange(customCardsToAddToPool);
            __result = toProcessCards.Count > 0;
        }
    }

    /// <summary>
    /// Adds custom cards to their appropriate pools.
    /// This one is used particularly when loading assets during loading screens.
    /// </summary>
    [HarmonyPatch]
    class AddCustomCardToPoolPatch3
    {
        static MethodBase TargetMethod()
        {
            var methods = typeof(CardEffectState)
                .GetMethods()
                .Where(method => method.Name == "GetFilteredCardListFromPool")
                .Where(method => method.IsStatic)
                .Cast<MethodBase>();
            return methods.First();
        }

        static void Postfix(ref bool __result, CardPool paramCardPool, CardUpgradeMaskData paramCardFilter, RelicManager relicManager, ref List<CardData> toProcessCards)
        {
            if (paramCardPool != null)
            {
                List<CardData> customCardsToAddToPool = CustomCardPoolManager.GetCardsForPoolSatisfyingConstraints(paramCardPool.name, paramCardFilter, relicManager);
                toProcessCards.AddRange(customCardsToAddToPool);
                __result = toProcessCards.Count > 0;
            }
        }
    }
}
