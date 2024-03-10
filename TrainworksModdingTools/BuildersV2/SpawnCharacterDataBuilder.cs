using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.BuildersV2
{
    public class SpawnCharacterDataBuilder
    {
        /// <summary>
        /// Character to spawn.
        /// </summary>
        public CharacterData CharacterData { get; set; }
        /// <summary>
        /// Required Covenant level to have this enemy spawn.
        /// </summary>
        public CovenantData RequiredCovenant { get; set; }
        /// <summary>
        /// If set replaces this character with the boss variant used for the scenario.
        /// </summary>
        public bool UseBossCharacter { get; set; }

        public SpawnGroupData.CharacterDataContainer Build()
        {
            var characterDataContainer = new SpawnGroupData.CharacterDataContainer();

            AccessTools.Field(typeof(SpawnGroupData.CharacterDataContainer), "characterData").SetValue(characterDataContainer, CharacterData);
            AccessTools.Field(typeof(SpawnGroupData.CharacterDataContainer), "requiredCovenant").SetValue(characterDataContainer, RequiredCovenant);
            AccessTools.Field(typeof(SpawnGroupData.CharacterDataContainer), "useBossCharacter").SetValue(characterDataContainer, UseBossCharacter);

            return characterDataContainer;
        }
    }
}
