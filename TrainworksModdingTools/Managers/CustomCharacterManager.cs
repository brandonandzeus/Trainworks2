using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BepInEx.Logging;
using Trainworks.Builders;
using HarmonyLib;
using UnityEngine;
using ShinyShoe;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Trainworks.Utilities;

namespace Trainworks.Managers
{
    /// <summary>
    /// Handles registration and storage of custom character data.
    /// </summary>
    public class CustomCharacterManager
    {
        /// <summary>
        /// Maps custom character IDs to their respective CharacterData.
        /// </summary>
        public static IDictionary<string, CharacterData> CustomCharacterData { get; } = new Dictionary<string, CharacterData>();
        /// <summary>
        /// A default character prefab which is cloned to create custom characters.
        /// Essential for custom character art. Set during game startup.
        /// </summary>
        public static GameObject TemplateCharacter { get; set; }
        /// <summary>
        /// Maps custom characters to their respective synthesis data.
        /// </summary>
        public static IDictionary<CharacterData, CardUpgradeData> UnitSynthesisMapping = new Dictionary<CharacterData, CardUpgradeData>();


        /// <summary>
        /// Register a custom character with the manager, allowing it to show up in game.
        /// </summary>
        /// <param name="data">The custom character data to register</param>
        public static bool RegisterCustomCharacter(CharacterData data)
        {
            if (!CustomCharacterData.ContainsKey(data.GetID()))
            {
                CustomCharacterData.Add(data.GetID(), data);
                ProviderManager.SaveManager.GetAllGameData().GetAllCharacterData().Add(data);

                return true;
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate character data with name: " + data.name);
                return false;
            }
        }

        /// <summary>
        /// Get the character data corresponding to the given ID
        /// </summary>
        /// <param name="characterID">ID of the character to get</param>
        /// <returns>The character data for the given ID</returns>
        public static CharacterData GetCharacterDataByID(string characterID)
        {
            // Search for custom character matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(characterID);
            if (CustomCharacterData.ContainsKey(guid))
            {
                return CustomCharacterData[guid];
            }

            // No custom card found; search for vanilla character matching ID
            var vanillaChar = ProviderManager.SaveManager.GetAllGameData().GetAllCharacterData().Find((chara) =>
            {
                return chara.GetID() == characterID;
            });
            if (vanillaChar == null)
            {
                Trainworks.Log(LogLevel.Warning, "Couldn't find character: " + characterID + " - This will cause crashes.");
            }
            return vanillaChar;
        }

        /// <summary>
        /// Maps custom subtypes IDs to their respective SubtypeData.
        /// </summary>
        public static IDictionary<string, SubtypeData> CustomSubtypeData { get; } = new Dictionary<string, SubtypeData>();

        /// <summary>
        /// Registers a subtype, making it available for localization
        /// </summary>
        /// <param name="ID">The key used for assigning the subtype, and for its localization</param>
        public static void RegisterSubtype(string ID)
        {
            CustomSubtypeData.Add(ID, new SubtypeDataBuilder { _Subtype = ID }.Build());
        }

        /// <summary>
        /// Registers a subtype and a localization.
        /// Note that all languages will use the same localization.
        /// For more fine-tuned control, use the (string) overload of this method instead.
        /// </summary>
        /// <param name="ID">The key used for assigning the subtype, and for its localization</param>
        /// <param name="name">The subtype text in all languages</param>
        public static void RegisterSubtype(string ID, string name)
        {
            CustomSubtypeData.Add(ID, new SubtypeDataBuilder { _Subtype = ID }.Build());
            CustomLocalizationManager.ImportSingleLocalization(ID, "Text", "", "", "", "", name, name, name, name, name, name);
        }


        public static void LoadTemplateCharacter(SaveManager saveManager)
        {
            var characterData = saveManager.GetAllGameData().GetAllCharacterData()[0];
            var loadOperation = characterData.characterPrefabVariantRef.LoadAsset<GameObject>();
            loadOperation.Completed += TemplateCharacterLoadingComplete;
        }

        private static void TemplateCharacterLoadingComplete(IAsyncOperation<GameObject> asyncOperation)
        {
            TemplateCharacter = asyncOperation.Result;
            if (TemplateCharacter == null)
            {
                Trainworks.Log(LogLevel.Warning, "Failed to load character template");
            }
        }

        public static void RegisterUnitSynthesis(CharacterData characterData, CardUpgradeData cardUpgrade)
        {
            if (UnitSynthesisMapping.ContainsKey(characterData))
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate synthesis data for character: " + characterData.name);
                return;
            }
            UnitSynthesisMapping.Add(characterData, cardUpgrade);
        }

        /// <summary> Gets unit synthesis for a custom character. </summary>
        /// <param name="characterData"> Custom character data. </param>
        /// <returns> The unit synthesis if this is a custom character, null otherwise. </returns>

        public static CardUpgradeData GetUnitSynthesis(CharacterData characterData)
        {
            var cardUpgrades = ProviderManager.SaveManager.GetAllGameData().GetAllCardUpgradeData();

            if (!CustomCharacterData.ContainsKey(characterData.GetID()))
            {
                //Trainworks.Log("Not a custom character " + characterData);
                return null;
            }

            //Trainworks.Log("" + characterData);
            // New Builders should already have this cached.
            if (UnitSynthesisMapping.TryGetValue(characterData, out CardUpgradeData value))
            {
                //Trainworks.Log("Synthesis found for unit " + characterData + " " + value);
                return value;
            }
            // Legacy Builders need to find and cache the synthesis.
            // Search in reverse as its most likely at the end ignoring dummy synthesis data if found.
            else
            {
                var synthesis = cardUpgrades.FindLast(u => characterData == u.GetSourceSynthesisUnit() && u.GetUpgradeDescriptionKey() != "Default_dummy_synthesis_description");
                if (synthesis != null)
                {
                    UnitSynthesisMapping.Add(characterData, synthesis);
                    //Trainworks.Log("Synthesis found for unit " + characterData + " " + synthesis);
                    return synthesis;
                }
            }
            
            //Trainworks.Log("No synthesis found for unit " + characterData);

            // Last attempt find the dummy unit synthesis.
            return cardUpgrades.FindLast(u => characterData == u.GetSourceSynthesisUnit());
        }
    }
}
