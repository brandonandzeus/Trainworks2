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
using System.Reflection;
using static RotaryHeart.Lib.DataBaseExample;

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
        /// Maps RoomModifierClassName to the custom sprite.
        /// </summary>
        public static IDictionary<string, Sprite> CustomRoomModifierIcons = new Dictionary<string, Sprite>();


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

        /// <summary>
        /// Marks a CharacterData to be preloaded always.
        /// This is only necessary for custom enemies and characters not spawned by a card.
        /// 
        /// You don't need to call this function if the associated spawner card is already marked for preloading.
        /// </summary>
        /// <param name="characterID">Character ID</param>
        public static void MarkCharacterForPreloading(string characterID)
        {
            MarkCharacterForPreloading(CustomCharacterData[characterID]);
        }

        /// <summary>
        /// Marks a CharacterData to be preloaded always.
        /// This is only necessary for custom enemies and characters not spawned by a card.
        /// 
        /// You don't need to call this function if the associated spawner card is already marked for preloading.
        /// </summary>
        /// <param name="character">CharacterData to mark for preloading</param>
        public static void MarkCharacterForPreloading(CharacterData character)
        {
            if (character == null)
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to mark a null CharacterData for preloading ignoring.");
                return;
            }

            var assetLoadingManager = AssetLoadingManager.GetInst();
            var assetLoadingData = (AssetLoadingData)AccessTools.Field(typeof(AssetLoadingManager), "_assetLoadingData").GetValue(assetLoadingManager);
            assetLoadingData.CharactersAlwaysLoad.Add(character);
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

        /// <summary>
        /// Setup a Custom Room Modifier Icon sprite
        /// Note that each instance of a RoomModifierData with the same RoomModifierClassType will share the icon. 
        /// </summary>
        /// <param name="RoomModifierClassType">Type inheriting from RoomStateModifierBase</param>
        /// <param name="icon_path">Relative path to an icon to use for the CharacterUI.</param>
        /// <param name="tooltip_icon_path">Ignored if icon_path is 24x24. Icon for use in tooltips must be 24x24</param>
        public static void AddCustomRoomModifierIcon(Type RoomModifierClassType, string icon_path, string tooltip_icon_path = null)
        {
            // This prevents mutating original game data, if you change the icon for one Room Modifier
            // it will change them all. Probably not what you want.
            if (BuildersV2.BuilderUtils.IsFromBaseGame(RoomModifierClassType))
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Room Modifier Class Type: " + RoomModifierClassType.FullName + " is a base game Room Modifier. Ignoring.");
                return;
            }

            var assembly = Assembly.GetCallingAssembly();
            var base_path = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];

            var full_icon_path = base_path + "/" + icon_path;

            var class_name = BuildersV2.BuilderUtils.GetEffectClassName(RoomModifierClassType);
            Sprite sprite = CustomAssetManager.LoadSpriteFromPath(full_icon_path);
            CustomRoomModifierIcons.Add(class_name, sprite);

            if (sprite.texture.width == 24 && sprite.texture.height == 24 && tooltip_icon_path == null)
            {
                _ = TMP_SpriteAssetUtils.AddTextIcon(full_icon_path, sprite.name);
            }
            else if (tooltip_icon_path != null)
            {
                _ = TMP_SpriteAssetUtils.AddTextIcon(base_path + "/" + tooltip_icon_path, sprite.name);
            }
        }

        /// <summary>
        /// Setup a custom Icon for Character Triggers.
        /// Modifies the internal Trigger Sprite Dictionary.
        /// </summary>
        /// <param name="trigger">Trigger Type, should be a custom Trigger</param>
        /// <param name="icon_path">Relative path to an icon to use for the CharacterUI.</param>
        /// <param name="tooltip_icon_path">Ignored if icon_path is 24x24. Icon for use in tooltips must be 24x24</param>
        public static void AddCustomTriggerIcon(CharacterTriggerData.Trigger trigger, string icon_path, string tooltip_icon_path = null)
        {
            var statusManager = StatusEffectManager.Instance;
            var displayData = (StatusEffectsDisplayData)AccessTools.Field(typeof(StatusEffectManager), "displayData").GetValue(statusManager);
            var triggerIcons = (StatusEffectsDisplayData.TriggerSpriteDict)AccessTools.Field(typeof(StatusEffectsDisplayData), "triggerIcons").GetValue(displayData);

            var assembly = Assembly.GetCallingAssembly();
            var base_path = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];

            var full_icon_path = base_path + "/" + icon_path;

            Sprite sprite = CustomAssetManager.LoadSpriteFromPath(full_icon_path);
            triggerIcons.Add(trigger, sprite);

            if (sprite.texture.width == 24 && sprite.texture.height == 24 && tooltip_icon_path == null)
            {
                _ = TMP_SpriteAssetUtils.AddTextIcon(full_icon_path, sprite.name);
            }
            else if (tooltip_icon_path != null)
            {
                _ = TMP_SpriteAssetUtils.AddTextIcon(base_path + "/" + tooltip_icon_path, sprite.name);
            }
        }
    }
}
