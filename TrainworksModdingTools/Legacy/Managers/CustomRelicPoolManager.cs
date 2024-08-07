﻿using System;
using System.Collections.Generic;

namespace Trainworks.Managers
{
    /// <summary>
    /// DEFUNCT. Do not use.
    /// </summary>
    [Obsolete("This class is no longer in use, Relics are added directly to the MegaRelicPool now so no need to call anything for that.", true)]
    public class CustomRelicPoolManager
    {
        /// <summary>
        /// Maps relic pool IDs to the CollectableRelicData of relics which can appear in them.
        /// Relics which naturally appear in the pool in the base game will not appear in these lists.
        /// </summary>
        public static IDictionary<string, List<CollectableRelicData>> CustomRelicPoolData { get; } = new Dictionary<string, List<CollectableRelicData>>();

        /// <summary>
        /// Add the relic to the relic pools with given IDs.
        /// </summary>
        /// <param name="relicData">CollectableRelicData to be added to the pools</param>
        /// <param name="relicPoolIDs">List of relic pool IDs to add the relic to</param>
        public static void AddRelicToPools(CollectableRelicData relicData, List<string> relicPoolIDs)
        {
            foreach (string relicPoolID in relicPoolIDs)
            {
                if (!CustomRelicPoolData.ContainsKey(relicPoolID))
                {
                    CustomRelicPoolData[relicPoolID] = new List<CollectableRelicData>();
                }
                CustomRelicPoolData[relicPoolID].Add(relicData);
            }
        }

        /// <summary>
        /// Gets a list of all relics added to the given relic pool by mods.
        /// Relics which naturally appear in the pool will not be returned.
        /// </summary>
        /// <param name="relicPoolID">ID of the relic pool to get relics for</param>
        /// <returns>A list of relics added to the relic pool with given ID by mods</returns>
        public static List<CollectableRelicData> GetRelicsForPool(string relicPoolID)
        {
            if (CustomRelicPoolData.ContainsKey(relicPoolID))
            {
                return CustomRelicPoolData[relicPoolID];
            }
            return new List<CollectableRelicData>();
        }
    }
}
