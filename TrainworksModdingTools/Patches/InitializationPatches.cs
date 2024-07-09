using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using ShinyShoe.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trainworks.Interfaces;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;

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
        public static List<Tuple<Type, Exception>> FailedToLoadMods = new List<Tuple<Type, Exception>>();

        public static void Postfix(AssetLoadingData ____assetLoadingData)
        {
            Trainworks.Log("[START] Loading all Trainworks Plugins");

            CustomCardPoolManager.GatherAllVanillaCardPools();

            var clm = CustomLocalizationManager.Instance();
            if (!LocalizationManager.ParamManagers.Contains(clm))
            {
                LocalizationManager.ParamManagers.Add(clm);
                LocalizationManager.LocalizeAll(true);
            }

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

            List<IInitializable> initializables =
                PluginManager.Plugins.Values.ToList()
                    .Where((plugin) => plugin is IInitializable)
                    .Select((plugin) => plugin as IInitializable)
                    .ToList();
            initializables.ForEach((initializable) => DoInitialize(initializable));
        }

        public static void DoInitialize(IInitializable mod)
        {
            try
            {
                mod.Initialize();
            }
            catch (Exception ex)
            {
                var t = new Tuple<Type, Exception>(mod.GetType(), ex);
                FailedToLoadMods.Add(t);
                Trainworks.Log(LogLevel.Error, string.Format("[CATASTROPHIC] {0} Raised an exception: {1}", mod.GetType(), ex.Message));
            }
        }
    }

    [HarmonyPatch(typeof(LoadingScreen), "FadeOutFullScreen")]
    class SpriteAssetInitializationPatch
    {
        private static bool HasBuiltSpriteAssets = false;
        public static void Postfix(ScreenManager ____screenManager)
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

                /*if (AssetLoadingManagerInitializationPatch.FailedToLoadMods.Count != 0)
                {
                    foreach (var type_ex in AssetLoadingManagerInitializationPatch.FailedToLoadMods)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendLine(string.Format("{0} failed to load due to the following Exception.", type_ex.Key.Name));
                        stringBuilder.AppendLine(type_ex.Value.ToString());
                        ____screenManager.ShowNotificationDialog(stringBuilder.ToString());
                    }
                }*/
            }
            // Do not add any code here, as this function is called multiple times in different locations.
        }
    }

    [HarmonyPatch(typeof(MainMenuScreen), "Initialize")]
    class ErrorReportingPatch
    {
        static ScreenManager screenManager;
        static int shown = 0;
        static bool HasShownErrors = false;
        public static void Postfix(ScreenManager ___screenManager)
        {
            screenManager = ___screenManager;
            if (!HasShownErrors && AssetLoadingManagerInitializationPatch.FailedToLoadMods.Count != 0)
            {
                ShowException();
            }
            HasShownErrors = true;
        }

        public static void ShowException()
        {
            if (shown >= AssetLoadingManagerInitializationPatch.FailedToLoadMods.Count)
                return;

            (Type type, Exception ex) = AssetLoadingManagerInitializationPatch.FailedToLoadMods[shown];
            shown++;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("{0} failed to load due to the following Exception.", type.FullName));
            stringBuilder.AppendLine(ex.ToString());
            screenManager.ShowNotificationDialog(stringBuilder.ToString(), ShowException);
            
        }
    }
}
