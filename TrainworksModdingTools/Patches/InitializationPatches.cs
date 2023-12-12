using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Managers;
using Trainworks.Interfaces;
using System.Linq;
using Trainworks.Utilities;
using ShinyShoe.Loading;

namespace Trainworks.Patches
{
    /// <summary>
    /// Provides managers with the references they need to function.
    /// </summary>
    [HarmonyPatch(typeof(SaveManager), "Initialize")]
    class SaveManagerInitializationPatch
    {
        public static void Postfix(SaveManager __instance)
        {
            CustomCharacterManager.LoadTemplateCharacter(__instance);
            CustomAssetManager.InitializeAssetConstructors();
        }
    }

    /// <summary>
    /// At this point, the Trainworks is fully set up.
    /// Initialize all Trainworks mods by calling their methods.
    /// </summary>
    [HarmonyPatch(typeof(AssetLoadingManager), "Start")]
    class AssetLoadingManagerInitializationPatch
    {
        public static void Postfix()
        {
            // Initialize replacement string dict by grabbing a reference to LocalizationGlobalParameterHandle.
            // Should be safe the object exists in memory at runtime as a static reference.
            LanguageManager manager;
            ProviderManager.TryGetProvider<LanguageManager>(out manager);
            var handler = AccessTools.Field(typeof(LanguageManager), "_paramHandler").GetValue(manager);
            var dict = AccessTools.Field(typeof(LocalizationGlobalParameterHandler), "_replacements").GetValue(handler);
            CustomLocalizationManager.ReplacementStrings = (Dictionary<string, ReplacementStringData>)dict;

            List <IInitializable> initializables =
                PluginManager.Plugins.Values.ToList()
                    .Where((plugin) => plugin is IInitializable)
                    .Select((plugin) => plugin as IInitializable)
                    .ToList();
            initializables.ForEach((initializable) => initializable.Initialize());
        }
    }

    [HarmonyPatch(typeof(LoadingScreen), "FadeOutFullScreen")]
    class SpriteAssetInitializationPatch
    {
        private static bool HasBuiltSpriteAssets = false;
        public static void Postfix()
        {
            // Here because not all clans Initialize via their Initialize.
            // Note for later, Not everything is guaranteed to be loaded by clans.
            // Not until the MainMenu screen is up.
            if (!HasBuiltSpriteAssets)
            {
                TMP_SpriteAssetUtils.Build();
                HasBuiltSpriteAssets = true;
            }
        }
    }
}
