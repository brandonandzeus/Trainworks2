using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class RelicPoolBuilder
    {
        /// <summary>
        /// This is both its ID and its name. Must be unique.
        /// </summary>
        public string RelicPoolID { get; set; }

        /// <summary>
        /// The IDs of Collectable Relics to put into the pool
        /// </summary>
        public List<string> RelicIDs { get; set; }
        /// <summary>
        /// CollectableRelicDatas  to put into the pool
        /// </summary>
        public List<CollectableRelicData> Relics { get; set; }

        public RelicPoolBuilder()
        {
            RelicIDs = new List<string>();
            Relics = new List<CollectableRelicData>();
        }

        /// <summary>
        /// Builds the RelicPool represented by this builder's parameters
        /// and registers it with CustomCollectableRelicManager.
        /// </summary>
        /// <returns>The newly registered EnhancerPool</returns>
        public RelicPool BuildAndRegister()
        {
            var pool = Build();
            CustomCollectableRelicManager.RegisterRelicPool(pool);
            return pool;        }

        /// <summary>
        /// Builds the RelicPool represented by this builder's parameters
        /// </summary>
        /// <returns>The newly created RelicPool</returns>
        public RelicPool Build()
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (RelicPoolID == null)
            {
                // Doesn't affect localization keys so setting to warning.
                Trainworks.Log(LogLevel.Warning, "Warning should provide a RelicPoolID.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            }

            RelicPool relicPool = ScriptableObject.CreateInstance<RelicPool>();
            relicPool.name = RelicPoolID;
            var relicDataList = (Malee.ReorderableArray<RelicData>)AccessTools.Field(typeof(RelicPool), "relicDataList").GetValue(relicPool);
            foreach (string id in RelicIDs)
            {
                CollectableRelicData relic = CustomCollectableRelicManager.GetRelicDataByID(id);
                if (relic != null)
                {
                    relicDataList.Add(relic);
                }
            }
            foreach (CollectableRelicData data in Relics)
            {
                relicDataList.Add(data);
            }
            return relicPool;
        }
    }
}
