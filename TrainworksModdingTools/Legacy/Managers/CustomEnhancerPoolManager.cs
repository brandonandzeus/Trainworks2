﻿using System.Collections.Generic;

// Deprecated in favor of Trainworks.BuildersV2.EnhancerDataBuilder and Trainworks.Managers.CustomEnhancerManager
// This class has a bug in that it doesn't add the Enhancers to AllGameData.
namespace Trainworks.Managers
{
    public class CustomEnhancerPoolManager
    {
        /// <summary>
        /// Enhancer pool to add the EnhancerData of Enhancers which can appear.
        /// Enhancers which naturally appear in the pool in the base game will not appear in these lists.
        /// </summary>
        public static IDictionary<string, List<EnhancerData>> CustomEnhancerPoolData { get; } = new Dictionary<string, List<EnhancerData>>();

        /// <summary>
        /// Add the enhancer to the pool.
        /// </summary>
        /// <param name="enhancerData">EnhancerData to be added to the pool</param>
        /// <param name="enhancerPoolID">Name of the Enhancer pool to add to</param>
        public static void AddEnhancerToPool(EnhancerData enhancerData, string enhancerPoolID)
        {
            if (!CustomEnhancerPoolData.ContainsKey(enhancerPoolID))
            {
                CustomEnhancerPoolData[enhancerPoolID] = new List<EnhancerData>();
            }
            if (!CustomEnhancerPoolData[enhancerPoolID].Contains(enhancerData))
            {
                CustomEnhancerPoolData[enhancerPoolID].Add(enhancerData);
            }
        }
    }
}
