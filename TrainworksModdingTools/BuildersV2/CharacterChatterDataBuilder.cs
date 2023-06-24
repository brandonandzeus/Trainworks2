using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

// Credit to ThreeFishes for the base code, Adopted from Equestrian-Clan.
namespace Trainworks.BuildersV2
{
    /// <summary>
    /// Determines what the character can say in a variety of situations. All string vales are treated as localization keys.
    /// </summary>
    public class CharacterChatterDataBuilder
    {
        /// <summary>
        /// Chatter ID, this must be a unique id.
        /// </summary>
        public string ChatterID { get; set; }

        /// <summary>
        /// The gender of the speaking character.
        /// </summary>
        public CharacterChatterData.Gender Gender { get; set; }

        /// <summary>
        /// What the character can say when played.
        /// </summary>
        public List<string> CharacterAddedExpressionKeys { get; set; }

        /// <summary>
        /// What the character can say when attacking.
        /// </summary>
        public List<string> CharacterAttackingExpressionKeys { get; set; }

        /// <summary>
        /// What the character can say upon being defeated.
        /// </summary>
        public List<string> CharacterSlayedExpressionKeys { get; set; }

        /// <summary>
        /// What the character can say during the player's turn.
        /// </summary>
        public List<string> CharacterIdleExpressionKeys { get; set; }

        /// <summary>
        /// What the character can say when preforming specific, triggered actions. Each trigger can have its own list of sayings.
        /// </summary>
        public List<CharacterChatterData.TriggerChatterExpressionData> CharacterTriggerExpressionKeys { get; set; }

        public CharacterChatterDataBuilder()
        {
            Gender = CharacterChatterData.Gender.Neutral;
            CharacterAddedExpressionKeys = new List<string>();
            CharacterAttackingExpressionKeys = new List<string>();
            CharacterSlayedExpressionKeys = new List<string>();
            CharacterIdleExpressionKeys = new List<string>();
            CharacterTriggerExpressionKeys = new List<CharacterChatterData.TriggerChatterExpressionData>();
        }

        public List<CharacterChatterData.ChatterExpressionData> GetChatterExpressionData(List<string> keys)
        {
            if (keys.IsNullOrEmpty())
            {
                return null;
            }

            List<CharacterChatterData.ChatterExpressionData> expressions = new List<CharacterChatterData.ChatterExpressionData>();

            foreach (string key in keys)
            {
                if (!key.IsNullOrEmpty())
                {
                    expressions.Add(new CharacterChatterData.ChatterExpressionData()
                    {
                        locKey = key,
                    });
                }
            }

            return expressions;
        }

        public CharacterChatterData Build()
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (ChatterID == null)
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Warning should provide a ChatterID.");
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Stacktrace: " + Environment.StackTrace);

            }

            CharacterChatterData chatter = ScriptableObject.CreateInstance<CharacterChatterData>();
            chatter.name = ChatterID;

            AccessTools.Field(typeof(CharacterChatterData), "characterAddedExpressions").SetValue(chatter, GetChatterExpressionData(CharacterAddedExpressionKeys));
            AccessTools.Field(typeof(CharacterChatterData), "characterAttackingExpressions").SetValue(chatter, GetChatterExpressionData(CharacterAttackingExpressionKeys));
            AccessTools.Field(typeof(CharacterChatterData), "characterSlayedExpressions").SetValue(chatter, GetChatterExpressionData(CharacterSlayedExpressionKeys));
            AccessTools.Field(typeof(CharacterChatterData), "characterIdleExpressions").SetValue(chatter, GetChatterExpressionData(CharacterIdleExpressionKeys));
            AccessTools.Field(typeof(CharacterChatterData), "characterTriggerExpressions").SetValue(chatter, CharacterTriggerExpressionKeys);

            return chatter;
        }
    }
}