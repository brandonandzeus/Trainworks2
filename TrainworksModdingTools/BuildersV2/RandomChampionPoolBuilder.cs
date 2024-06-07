using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class RandomChampionPoolBuilder
    {
        /// <summary>
        /// This is both its ID and its name. Must be unique.
        /// </summary>
        public string ChampionPoolID { get; set; }

        /// <summary>
        /// Champion card along with a set of upgrades to apply to the card when picked.
        /// </summary>
        public List<RandomChampionPool.GrantedChampionInfo> ChampionInfoList { get; set; }

        public RandomChampionPoolBuilder()
        {
            ChampionInfoList = new List<RandomChampionPool.GrantedChampionInfo>();
        }

        /// <summary>
        /// Builds the RandomChampionPool represented by this builder's parameters
        /// </summary>
        /// <returns>The newly created RandomChampionPool</returns>
        public RandomChampionPool Build()
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (ChampionPoolID == null)
            {
                Trainworks.Log(LogLevel.Warning, "Warning should provide a ChampionPoolID.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            }

            RandomChampionPool championPool = ScriptableObject.CreateInstance<RandomChampionPool>();
            championPool.name = ChampionPoolID;

            var championInfoList = (Malee.ReorderableArray<RandomChampionPool.GrantedChampionInfo>)AccessTools.Field(typeof(RandomChampionPool), "championInfoList").GetValue(championPool);
            championInfoList.CopyFrom(ChampionInfoList);

            return championPool;
        }
    }
}
