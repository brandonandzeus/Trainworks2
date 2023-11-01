using Malee;
using System;
using System.Collections.Generic;
using System.Text;
using static SpawnPatternData;
using UnityEngine.Serialization;
using UnityEngine;
using HarmonyLib;

namespace Trainworks.BuildersV2
{
    public class SpawnPatternDataBuilder
    {
        /// <summary>
        /// Defines the waves spawned in the scenario.
        /// </summary>
        public List<SpawnGroupPoolDataBuilder> SpawnGroupWaves { get; set; }
        /// <summary>
        /// True if the scenario is looping.
        /// </summary>
        public bool IsLoopingScenario { get; set; }
        /// <summary>
        /// Force the Crystal Variant of a boss.
        /// </summary>
        public bool ForceCrystalVariant { get; set; }
        /// <summary>
        /// Boss type.
        /// </summary>
        public BossType BossType { get; set; }
        /// <summary>
        /// Used for flying bosses only. Boss Character.
        /// </summary>
        public CharacterData BossCharacter { get; set; }
        /// <summary>
        /// Used for flying bosses only. Hard Boss Character.
        /// </summary>
        public CharacterData HardBossCharacter { get; set; }
        /// <summary>
        /// Used for divinity a boss character per floor.
        /// </summary>
        public List<CharacterData> BossCharactersPerFloor { get; set; }

        public SpawnPatternDataBuilder()
        {
            SpawnGroupWaves = new List<SpawnGroupPoolDataBuilder>();
            BossCharactersPerFloor = new List<CharacterData>();
        }

        public SpawnPatternData Build()
        {
            var spawnPatternData = new SpawnPatternData();

            SpawnPatternData.SpawnGroupPoolsDataList list = (SpawnPatternData.SpawnGroupPoolsDataList) AccessTools.Field(typeof(SpawnPatternData), "spawnGroupWaves").GetValue(spawnPatternData);
            list.Clear();
            foreach (var wave in SpawnGroupWaves)
            {
                list.Add(wave.Build());
            }

            AccessTools.Field(typeof(SpawnPatternData), "isLoopingScenario").SetValue(spawnPatternData, IsLoopingScenario);
            AccessTools.Field(typeof(SpawnPatternData), "forceCrystalVariant").SetValue(spawnPatternData, ForceCrystalVariant);
            AccessTools.Field(typeof(SpawnPatternData), "bossType").SetValue(spawnPatternData, BossType);
            AccessTools.Field(typeof(SpawnPatternData), "bossCharacter").SetValue(spawnPatternData, BossCharacter);
            AccessTools.Field(typeof(SpawnPatternData), "hardBossCharacter").SetValue(spawnPatternData, HardBossCharacter);
            AccessTools.Field(typeof(SpawnPatternData), "bossCharactersPerFloor").SetValue(spawnPatternData, BossCharactersPerFloor);

            return spawnPatternData;
        }
    }
}
