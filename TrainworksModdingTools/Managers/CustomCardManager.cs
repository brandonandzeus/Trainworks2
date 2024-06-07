using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using Trainworks.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Trainworks.Utilities;
using Malee;
using System;

namespace Trainworks.Managers
{
    /// <summary>
    /// Handles registration and storage of custom card data.
    /// </summary>
    public class CustomCardManager
    {
        /// <summary>
        /// Maps custom card IDs to their respective CardData.
        /// </summary>
        public static IDictionary<string, CardData> CustomCardData { get; } = new Dictionary<string, CardData>();

        /// <summary>
        /// CardPool containing all unlockable custom cards.
        /// Necessary to add to preloaded assets at start of run.
        /// Otherwise they will not be loaded ever.
        /// </summary>
        internal static CardPool UnlockableCustomCardsPool = ScriptableObject.CreateInstance<CardPool>();
        internal static ReorderableArray<CardData> customCardPoolDataList = null;
        /// <summary>
        /// Reference to UnlockableCustomCardsPool.cardDataList.
        /// </summary>
        internal static ReorderableArray<CardData> UnlockableCustomCardPoolDataList 
        { 
            get
            {
                if (customCardPoolDataList == null)
                    customCardPoolDataList = (ReorderableArray<CardData>)AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(UnlockableCustomCardsPool);
                return customCardPoolDataList;
            }
        }

        /// <summary>
        /// Register a custom card with the manager, allowing it to show up in game
        /// both in the logbook and whenever cards are chosen from the specified pools.
        /// </summary>
        /// <param name="cardData">The custom card data to register</param>
        /// <param name="cardPoolData">The card pools the custom card should be a part of</param>
        public static void RegisterCustomCard(CardData cardData, List<string> cardPoolData)
        {
            ContentValidator.Validate(cardData);
            if (!CustomCardData.ContainsKey(cardData.GetID()))
            {
                CustomCardData.Add(cardData.GetID(), cardData);
                CustomCardPoolManager.AddCardToPools(cardData, cardPoolData);
                ProviderManager.SaveManager.GetAllGameData().GetAllCardData().Add(cardData);
                if (cardData.GetUnlockLevel() > 1) 
                {
                    UnlockableCustomCardPoolDataList.Add(cardData);
                }
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate card data with name: " + cardData.name);
            }
        }

        /// <summary>
        /// Get the card data corresponding to the given ID
        /// </summary>
        /// <param name="cardID">ID of the card to get</param>
        /// <returns>The card data for the given ID</returns>
        public static CardData GetCardDataByID(string cardID)
        {
            // Search for custom card matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(cardID);
            if (CustomCardData.ContainsKey(guid))
            {
                return CustomCardData[guid];
            }

            // No custom card found; search for vanilla card matching ID
            var vanillaCard = ProviderManager.SaveManager.GetAllGameData().FindCardData(cardID);
            if (vanillaCard == null)
            {
                Trainworks.Log(LogLevel.Error, "Couldn't find card: " + cardID + " - This will cause crashes.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            }
            return vanillaCard;
        }
    }
}
