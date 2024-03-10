using HarmonyLib;
using System;
using System.Collections.Generic;
using Trainworks.ManagersV2;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class EnhancerPoolBuilder
    {
        /// <summary>
        /// This is both its ID and its name. Must be unique.
        /// </summary>
        public string EnhancerPoolID { get; set; }

        /// <summary>
        /// The IDs of enhancers to put into the pool
        /// </summary>
        public List<string> EnhancerIDs { get; set; }
        /// <summary>
        /// EnhancerData of enhancers to put into the pool
        /// </summary>
        public List<EnhancerData> Enhancers { get; set; }

        public EnhancerPoolBuilder()
        {
            EnhancerIDs = new List<string>();
            Enhancers = new List<EnhancerData>();
        }

        /// <summary>
        /// Builds the EnhancerPool represented by this builder's parameters
        /// and registers it with the CustomEnhancerPoolManager.
        /// </summary>
        /// <returns>The newly registered EnhancerPool</returns>
        public EnhancerPool BuildAndRegister()
        {
            var enhancerPool = Build();
            CustomEnhancerManager.RegisterEnhancerPool(enhancerPool);
            return enhancerPool;
        }

        /// <summary>
        /// Builds the EnhancerPool represented by this builder's parameters
        /// </summary>
        /// <returns>The newly created EnhancerPool</returns>
        public EnhancerPool Build()
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (EnhancerPoolID == null)
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Warning should provide a EnhancerPoolID.");
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Stacktrace: " + Environment.StackTrace);
            }

            EnhancerPool enhancerPool = ScriptableObject.CreateInstance<EnhancerPool>();
            enhancerPool.name = EnhancerPoolID;
            var relicDataList = (Malee.ReorderableArray<EnhancerData>)AccessTools.Field(typeof(EnhancerPool), "relicDataList").GetValue(enhancerPool);
            foreach (string enhancerID in EnhancerIDs)
            {
                EnhancerData enhancer = CustomEnhancerManager.GetEnhancerByID(enhancerID);
                if (enhancer != null)
                {
                    relicDataList.Add(enhancer);
                }
            }
            foreach (EnhancerData enhancerData in Enhancers)
            {
                relicDataList.Add(enhancerData);
            }
            return enhancerPool;
        }
    }
}
