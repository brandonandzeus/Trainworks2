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
        /// Localization Key for Wave Message.
        /// Must be set explicitly if WaveMessage is set.
        /// </summary>
        public String WaveMessageKey { get; set; }
        /// <summary>
        /// Value for Wave Message.
        /// If set this sets the localization for all languages.
        /// </summary>
        public string WaveMessage { get; set; }
        /// <summary>
        ///  Characters to spawn as part of this spawn group.
        ///  Note the ordering is reversed in game.
        /// </summary>
        public List<SpawnCharacterDataBuilder> Characters { get; set; }

        public SpawnGroupDataBuilder()
        {
            Characters = new List<SpawnCharacterDataBuilder>();
        }

        public SpawnGroupData Build()
        {
            SpawnGroupData spawnGroupData = new SpawnGroupData();

            AccessTools.Field(typeof(SpawnGroupData), "hasWaveMessage").SetValue(spawnGroupData, WaveMessageKey != null);
            AccessTools.Field(typeof(SpawnGroupData), "waveMessageKey").SetValue(spawnGroupData, WaveMessageKey);
            
            SpawnGroupData.CharacterDataContainerList characterList = (SpawnGroupData.CharacterDataContainerList) AccessTools.Field(typeof(SpawnGroupData), "characterDataContainerList").GetValue(spawnGroupData);
            characterList.Clear();
            foreach (var characterBuilder in Characters)
            {
                characterList.Add(characterBuilder.Build());
            }

            if (WaveMessage != null && WaveMessageKey != null)
            {
                BuilderUtils.ImportStandardLocalization(WaveMessageKey, WaveMessage);
            }

            return spawnGroupData;
        }
    }
}
