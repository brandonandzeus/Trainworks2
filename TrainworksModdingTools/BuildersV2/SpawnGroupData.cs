using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using static SpawnGroupData;

namespace Trainworks.BuildersV2
{
    public class SpawnGroupDataBuilder
    {
        /// <summary>
        /// Use a Wave Message.
        /// </summary>
        public bool HasWaveMessage { get; set; }
        /// <summary>
        /// Localization Key for Wave Message.
        /// Required to be set if HasWaveMessage is true.
        /// </summary>
        public String WaveMessasgeKey { get; set; }
        /// <summary>
        /// Value for Wave Message.
        /// If set this sets the localization for all languages.
        /// </summary>
        public string WaveMessage { get; set; }
        /// <summary>
        ///  Characters to spawn as part of this spawn group.
        /// </summary>
        public List<SpawnCharacterDataBuilder> CharacterDataContainerList { get; set; }

        public SpawnGroupData Build()
        {
            SpawnGroupData spawnGroupData = new SpawnGroupData();

            AccessTools.Field(typeof(SpawnGroupData), "hasWaveMessage").SetValue(spawnGroupData, HasWaveMessage);
            AccessTools.Field(typeof(SpawnGroupData), "waveMessageKey").SetValue(spawnGroupData, WaveMessasgeKey);
            
            SpawnGroupData.CharacterDataContainerList characterList = (SpawnGroupData.CharacterDataContainerList) AccessTools.Field(typeof(SpawnGroupData), "characterDataContainerList").GetValue(spawnGroupData);
            characterList.Clear();
            foreach (var characterBuilder in CharacterDataContainerList)
            {
                characterList.Add(characterBuilder.Build());
            }

            return spawnGroupData;
        }
    }
}
