using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Trainworks.Managers;
using Trainworks.Interfaces;
using System.Linq;
using Trainworks.Utilities;
using ShinyShoe.Loading;
using I2.Loc;

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
        public static void Postfix(AssetLoadingData ____assetLoadingData)
        {
            Trainworks.Log("[START] Loading all Trainworks Plugins");
            // Ensure that unlockable cards are loaded, otherwise they will never be loaded.
            // (UnlockScreen doesn't have a call to LoadAdditionalCards).
            // All others are OK since in DraftRewardData the assets are loaded right then.
            // Loading all assets is quite time intensive increasing the load time by 200%
            // For other preloaded assets see CustomCardPoolManager.MarkCardPoolForPreloading.
            ____assetLoadingData.CardPoolsAll.Add(CustomCardManager.UnlockableCustomCardsPool);

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
            // Here because not all clans Initialize via their Initialize (Equestrian)
            // Note for later, Not every custom GameData object is guaranteed to be loaded by clans.
            // Not until the MainMenu screen is up.
            if (!HasBuiltSpriteAssets)
            {
                TMP_SpriteAssetUtils.Build();
                CustomLocalizationManager.ImportLocalizationData();
                HasBuiltSpriteAssets = true;
                Trainworks.Log("[END] All Trainworks Plugins are loaded.");
            }
            // Do not add any code here, as this function is called multiple times in diiffrent locations.
        }
    }
}
