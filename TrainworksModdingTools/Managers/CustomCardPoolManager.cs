using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using HarmonyLib;
using Trainworks.Builders;
using Trainworks.ConstantsV2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Trainworks.Managers
{
    public class CustomCardPoolManager
    {
        /// <summary>
        /// Maps card pool IDs to the CardData of cards which can appear in them.
        /// Cards which naturally appear in the pool in the base game will not appear in these lists.
        /// </summary>
        public static IDictionary<string, List<CardData>> CustomCardPoolData { get; } = new Dictionary<string, List<CardData>>();

        /// <summary>
        /// Maps custom card pool IDs to their actual CardPool instances.
        /// </summary>
        public static IDictionary<string, CardPool> CustomCardPools { get; } = new Dictionary<string, CardPool>();

        /// <summary>
        /// Gets the Card MegaPool instance which contains every card in the game.
        /// </summary>
        public static CardPool GetMegaPool()
        {
            var reward = ProviderManager.SaveManager.GetAllGameData().FindRewardData(VanillaRewardIDs.CardDraftMainClassReward) as DraftRewardData;
            if (reward != null)
            {
                return reward.GetDraftPool();
            }
            Trainworks.Log(LogLevel.Error, "Could not get MegaPool Instance");
            return null;
        }

        /// <summary>
        /// Gets the Constriction Unit Pool used by the Conscription Notice artifact.
        /// </summary>
        /// <returns></returns>
        public static CardPool GetConscriptUnitPool()
        {
            var relic = ProviderManager.SaveManager.GetAllGameData().FindCollectableRelicData(VanillaCollectableRelicIDs.ConscriptionNotice);
            if (relic == null)
            {
                Trainworks.Log(LogLevel.Error, "Could not get ConscriptionPool");
                return null;
            }
            return relic.GetFirstRelicEffectData<RelicEffectAddBattleCardToHandOnUnitTrigger>().GetParamCardPool();
        }

        public static void RegisterCustomCardPool(CardPool cardPool)
        {
            if (!CustomCardPools.ContainsKey(cardPool.name))
            {
                CustomCardPools.Add(cardPool.name, cardPool);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate card pool with name: " + cardPool.name);
            }
        }

        /// <summary>
        /// Add the card to the card pools with given IDs.
        /// </summary>
        /// <param name="cardData">CardData to be added to the pools</param>
        /// <param name="cardPoolIDs">List of card pool IDs to add the card to</param>
        public static void AddCardToPools(CardData cardData, List<string> cardPoolIDs)
        {
            foreach (string cardPoolID in cardPoolIDs)
            {
                if (CustomCardPools.ContainsKey(cardPoolID))
                {
                    var pool = CustomCardPools[cardPoolID];
                    var cardDataList = (Malee.ReorderableArray<CardData>)AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(pool);
                    cardDataList.Add(cardData);
                }
                else
                {
                    if (!CustomCardPoolData.ContainsKey(cardPoolID))
                    {
                        CustomCardPoolData[cardPoolID] = new List<CardData>();
                    }
                    CustomCardPoolData[cardPoolID].Add(cardData);
                }
            }
        }

        /// <summary>
        /// Gets a list of all cards added to the given card pool by mods.
        /// Cards which naturally appear in the pool will not be returned.
        /// </summary>
        /// <param name="cardPoolID">ID of the card pool to get cards for</param>
        /// <returns>A list of cards added to the card pool with given ID by mods</returns>
        public static List<CardData> GetCardsForPool(string cardPoolID)
        {
            if (CustomCardPoolData.ContainsKey(cardPoolID))
            {
                return CustomCardPoolData[cardPoolID];
            }
            return new List<CardData>();
        }

        /// <summary>
        /// Gets a list of all cards added to the given card pool by mods
        /// which satisfy the constraints specified by the parameters passed in.
        /// Cards which naturally appear in the pool will not be returned.
        /// </summary>
        /// <param name="cardPoolID">ID of the card pool to get cards for</param>
        /// <param name="classData">Card must be part of this class</param>
        /// <param name="paramRarity">Rarity which is compared against the rarities of the cards in the pool using rarityCondition</param>
        /// <param name="rarityCondition">Rarity condition which takes into account paramRarity and the rarities of the cards in the pool</param>
        /// <param name="testRarityCondition">Whether or not the rarity condition should be checked</param>
        /// <returns>A list of cards added to the card pool with given ID by mods, all of which satisfy the given constraints.</returns>
        public static List<CardData> GetCardsForPoolSatisfyingConstraints(string cardPoolID, ClassData classData, CollectableRarity paramRarity, CardPoolHelper.RarityCondition rarityCondition, bool testRarityCondition)
        {
            var allValidCards = GetCardsForPool(cardPoolID);

            var validCards = new List<CardData>();

            //if (rarityCondition == null)
            //{    
                //testRarityCondition = false;
            //}
            rarityCondition = (rarityCondition ?? EqualRarity);

            foreach (CardData cardData in allValidCards)
            {
                if (cardData.GetLinkedClass() == classData && (!testRarityCondition || rarityCondition(paramRarity, cardData.GetRarity())))
                {
                    validCards.Add(cardData);
                }
            }

            return validCards;
        }

        private static CardPoolHelper.RarityCondition EqualRarity = (CollectableRarity paramRarity, CollectableRarity cardRarity) => paramRarity == cardRarity;

        /// <summary>
        /// Gets a list of all cards added to the given card pool by mods
        /// which satisfy the constraints specified by the mask data passed in.
        /// Cards which naturally appear in the pool will not be returned.
        /// </summary>
        /// <param name="cardPoolID">ID of the card pool to get cards for</param>
        /// <param name="paramCardFilter">Constraints to satisfy</param>
        /// <param name="relicManager">a RelicManager</param>
        /// <returns>A list of cards added to the card pool with given ID by mods, all of which satisfy the given constraints.</returns>
        public static List<CardData> GetCardsForPoolSatisfyingConstraints(string cardPoolID, CardUpgradeMaskData paramCardFilter, RelicManager relicManager)
        {
            var allValidCards = GetCardsForPool(cardPoolID);
            var validCards = new List<CardData>();
            foreach (CardData cardData in allValidCards)
            {
                if (paramCardFilter == null)
                {
                    validCards.Add(cardData);
                }
                else if (paramCardFilter.FilterCard<CardData>(cardData, relicManager))
                {
                    validCards.Add(cardData);
                }
            }
            return validCards;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardPoolID"></param>
        /// <returns></returns>
        public static CardPool GetCustomCardPoolByID(string cardPoolID)
        {
            if (CustomCardPools.ContainsKey(cardPoolID))
            {
                return CustomCardPools[cardPoolID];
            }
            Trainworks.Log(LogLevel.Error, "Could not find card pool: " + cardPoolID);
            Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            return null;
        }

        /// <summary>
        /// Marks a custom card pool for preloading when a run starts.
        /// This is necessary if for instance you have an artifact that has the RelicEffectAddBattleCard or RelicEffectSpawnRandomUnitStartOfCombat
        /// Custom Assets are only loaded when they are needed (in contrast with standard assets which are all loaded only if the clan is selected at the start of a run).
        /// So if you somehow get a card or spawn a character outside of a card draft it will not be loaded due to how the codebase works.
        ///
        /// You don't need to call this function for card unlocks, these are done automatically. Card Unlocks also aren't preloaded.
        /// 
        /// Note it doesn't make sense to set both game_assets and clan_assets to true.
        /// </summary>
        /// <param name="cardPoolID">The card pool ID</param>
        /// <param name="clan_assets">Mark the card pool as one containing clan assets. These cards will only be loaded if the clan is selected for a run.</param>
        /// <param name="game_assets">Mark the card pool as one containing general game assets. These cards will always be loaded (Useful for things like Custom Blights or Scourges).</param>
        public static void MarkCardPoolForPreloading(string cardPoolID, bool clan_assets = true, bool game_assets = false)
        {
            MarkCardPoolForPreloading(CustomCardPools[cardPoolID], clan_assets, game_assets);
        }

        /// <summary>
        /// Marks a custom card pool for preloading when a run starts.
        /// This is necessary if for instance you have an artifact that has the RelicEffectAddBattleCard or RelicEffectSpawnRandomUnitStartOfCombat
        /// Custom Assets are only loaded when they are needed (in contrast with standard assets which are all loaded only if the clan is selected at the start of a run).
        /// So if you somehow get a card or spawn a character outside of a card draft it will not be loaded due to how the codebase works.
        ///
        /// You don't need to call this function for card unlocks, these are done automatically. Card Unlocks also aren't preloaded.
        /// 
        /// Note it doesn't make sense to set both game_assets and clan_assets to true.
        /// </summary>
        /// <param name="cardPool">The card pool</param>
        /// <param name="clan_assets">Mark the card pool as one containing clan assets. These cards will only be loaded if the clan is selected for a run.</param>
        /// <param name="game_assets">Mark the card pool as one containing general game assets. These cards will always be loaded (Useful for things like Custom Blights or Scourges).</param>
        public static void MarkCardPoolForPreloading(CardPool cardPool, bool clan_assets = true, bool game_assets = false)
        {
            if (cardPool == null)
            {
                Trainworks.Log(LogLevel.Error, "Attempted to mark a null CardPool for preloading");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
                return;
            }

            var assetLoadingManager = AssetLoadingManager.GetInst();
            var assetLoadingData = (AssetLoadingData)AccessTools.Field(typeof(AssetLoadingManager), "_assetLoadingData").GetValue(assetLoadingManager);
            if (game_assets)
            {
                assetLoadingData.CardPoolsAlwaysLoad.Add(cardPool);
            }
            else if (clan_assets)
            {
                assetLoadingData.CardPoolsAll.Add(cardPool);
            }
        }
    }
}
