using HarmonyLib;
using ShinyShoe.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

// This patch fixes ClassData#starterCardUpgrade and ClassData#StarterRelics
// Unfortunately these two fields are only used by Wurmkin, and the devs did not handle the case
// where the other clan selected also have these fields set.
namespace Trainworks.Patches
{
    // This fixes starter cards for covenant disabled.
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.LoadClassesFromStartingConditions))]
    public class ClanStarterCardUpgradePatch
    {
        static bool Prefix(SaveManager __instance, StartingConditions startingConditions, AllGameData ___allGameData)
        {
            var GetRandomUnlockedClass = __instance.GetType().GetMethod("GetRandomUnlockedClass", BindingFlags.NonPublic | BindingFlags.Instance);
            var GetRandomUnlockedChampionIndex = __instance.GetType().GetMethod("GetRandomUnlockedChampionIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            var SetMainClass = __instance.GetType().GetMethod("SetMainClass", BindingFlags.NonPublic | BindingFlags.Instance);
            var SetSubClass = __instance.GetType().GetMethod("SetSubClass", BindingFlags.NonPublic | BindingFlags.Instance);

            ClassData classData = ___allGameData.FindClassData(startingConditions.Class);
            ClassData classData2 = ___allGameData.FindClassData(startingConditions.Subclass);
            int num = startingConditions.MainChampionIndex;
            int championIndex = startingConditions.SubChampionIndex;
            bool flag = startingConditions.RandomMainClass || classData == null;
            if (flag)
            {
                classData = (ClassData)GetRandomUnlockedClass.Invoke(__instance, new object[] { classData2 });
                num = (int)GetRandomUnlockedChampionIndex.Invoke(__instance, new object[] { classData });
            }
            bool flag2 = startingConditions.RandomSubClass || classData2 == null;
            if (flag2)
            {
                classData2 = (ClassData)GetRandomUnlockedClass.Invoke(__instance, new object[] { classData });
                championIndex = (int)GetRandomUnlockedChampionIndex.Invoke(__instance, new object[] { classData2 });
            }

            SetMainClass.Invoke(__instance, new object[] { classData, num, flag });
            SetSubClass.Invoke(__instance, new object[] { classData2, championIndex, flag2 });

            List<CardData> list = new List<CardData>();
            __instance.GetBalanceData().GetInitialStarterDeck().CollectCards(list, __instance);

            foreach (CardData item in list)
            {
                CardStateModifiers cardStateModifiers = null;
                if (SaveManager.CheckIfCardShouldBeUpgradedByClassStarterUpgrade(item, classData, classData2))
                {
                    // The only change, consider both classes.
                    CardUpgradeData cardUpgradeData = classData.GetStarterCardUpgrade();
                    if (cardUpgradeData != null)
                    {
                        cardStateModifiers = new CardStateModifiers();

                        CardUpgradeState cardUpgradeState = Activator.CreateInstance<CardUpgradeState>();
                        cardUpgradeState.Setup(cardUpgradeData);

                        cardStateModifiers.AddUpgrade(cardUpgradeState);
                    }

                    // Handle the case where FullClanMod is installed ignore if the classes are the same.
                    if (classData != classData2)
                    {
                        cardUpgradeData = classData2.GetStarterCardUpgrade();
                        if (cardUpgradeData != null)
                        {
                            cardStateModifiers = cardStateModifiers ?? new CardStateModifiers();

                            CardUpgradeState cardUpgradeState = Activator.CreateInstance<CardUpgradeState>();
                            cardUpgradeState.Setup(cardUpgradeData);

                            cardStateModifiers.AddUpgrade(cardUpgradeState);
                        }
                    }
                }
                __instance.AddCardToDeck(item, cardStateModifiers);
            }
            __instance.AddCardToDeck(classData.GetChampionCard(num));

            List<RelicData> starterRelics = classData.GetStarterRelics();
            for (int i = 0; i < 2; i++)
            {
                foreach (RelicData item2 in starterRelics)
                {
                    __instance.AddRelic(item2);
                }
                starterRelics = classData2.GetStarterRelics();
                // Handle the case where FullClanMod is installed. Don't want to double add a relic.
                if (classData == classData2) break;
            }

            return false;
        }
    }

    // This fixes it for Convenants 1-25
    // Apparently Covenant 1 Purges everything except for the Champion and then rebuilds the deck. Wasteful... but whatever.
    [HarmonyPatch(typeof(RelicEffectAddCardSetStartOfRun), nameof(RelicEffectAddCardSetStartOfRun.ApplyEffect))]
    public class ClanStarterCardUpgradePatch2
    {
        public static bool Prefix(RelicEffectParams relicEffectParams, List<CardData> ___showcaseCards, CardSetBuilder ___cardSetBuilder)
        {
            ___showcaseCards.Clear();
            if (___cardSetBuilder == null)
            {
                Log.Error(LogGroups.Gameplay, "Cannot add card set to the deck, CardSetBuilder must be assigned to relic effect");
                return false;
            }
            List<CardData> list = new List<CardData>();
            ___cardSetBuilder.CollectCards(list, relicEffectParams.saveManager, ___showcaseCards);
            ClassData mainClass = relicEffectParams.saveManager.GetMainClass();
            ClassData subClass = relicEffectParams.saveManager.GetSubClass();
            foreach (CardData item in list)
            {
                CardStateModifiers cardStateModifiers = null;
                if (SaveManager.CheckIfCardShouldBeUpgradedByClassStarterUpgrade(item, mainClass, subClass))
                {
                    // The only change, consider both classes.
                    CardUpgradeData cardUpgradeData = mainClass.GetStarterCardUpgrade();
                    if (cardUpgradeData != null)
                    {
                        cardStateModifiers = new CardStateModifiers();

                        CardUpgradeState cardUpgradeState = Activator.CreateInstance<CardUpgradeState>();
                        cardUpgradeState.Setup(cardUpgradeData);

                        cardStateModifiers.AddUpgrade(cardUpgradeState);
                    }
                    // There is a case where FullClanMod is installed. If the clans are the same then ignore
                    if (mainClass != subClass)
                    {
                        cardUpgradeData = subClass.GetStarterCardUpgrade();
                        if (cardUpgradeData != null)
                        {
                            cardStateModifiers = cardStateModifiers ?? new CardStateModifiers();

                            CardUpgradeState cardUpgradeState = Activator.CreateInstance<CardUpgradeState>();
                            cardUpgradeState.Setup(cardUpgradeData);

                            cardStateModifiers.AddUpgrade(cardUpgradeState);
                        }
                    }
                }
                relicEffectParams.saveManager.AddCardToDeck(item, cardStateModifiers);
            }

            return false;
        }
    }
}
