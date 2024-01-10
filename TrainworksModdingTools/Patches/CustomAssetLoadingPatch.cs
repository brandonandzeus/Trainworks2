using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Harmony;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ShinyShoe;
using Trainworks.Managers;

namespace Trainworks.Patches
{
    /// <summary>
    /// Override the asset loading procedure when loading custom assets
    /// </summary>
    [HarmonyPatch(typeof(AssetLoadingManager), "StartLoad", new Type[] { typeof(AssetReference), typeof(IAddressableAssetOwner), typeof(bool) })]
    class CustomAssetLoadingPatch
    {
        static MethodInfo setAssetInfoAssetMethod;

        static bool Prefix(ref AssetLoadingManager __instance, 
                            ref Dictionary<Hash128, AssetLoadingManager.AssetInfo> ____assetsLoaded, 
                            ref int ____numLoadingTasksRunning, 
                            ref AssetReference assetRef, 
                            ref IAddressableAssetOwner assetOwner
                           )
        {
            if (CustomAssetManager.RuntimeKeyToAssetInfo.ContainsKey(assetRef.RuntimeKey))
            {
                if (____assetsLoaded.TryGetValue(assetRef.RuntimeKey, out AssetLoadingManager.AssetInfo info))
                {
                    info.ownerCount++;
                }
                else
                {
                    info = new AssetLoadingManager.AssetInfo(assetRef);
                    ____assetsLoaded[assetRef.RuntimeKey] = info;
                }
                if (!info.Loading && !info.Loaded)
                {
                    info.status = AssetLoadingManager.AssetStatus.Loading;
                    ____numLoadingTasksRunning++;

                    // This load should be done asynchronously, but it isn't hurting anything.
                    // The overall loading action is done asynchronously through either LoadClassAssets
                    // for preloaded assets, or through DraftRewardData LoadAdditionalCards task.
                    var asset = CustomAssetManager.LoadGameObjectFromAssetRef(assetRef);

                    ____numLoadingTasksRunning--;

                    if (setAssetInfoAssetMethod == null)
                    {
                        setAssetInfoAssetMethod = AccessTools.Method(typeof(AssetLoadingManager), "SetAssetInfoAsset");
                    }
                    setAssetInfoAssetMethod.Invoke(__instance, new object[] { info, asset });
                    info.status = AssetLoadingManager.AssetStatus.Loaded;
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AssetLoadingManager), "Unload")]
    class CustomAssetUnloadingPatch
    {
        static MethodInfo setAssetInfoAssetMethod;

        public static bool Prefix(
            ref AssetLoadingManager __instance,
            ref AssetReference assetRef,
            IAddressableAssetOwner assetOwner,
            bool simulate,
            ref Dictionary<Hash128, AssetLoadingManager.AssetInfo> ____assetsLoaded,
            ref bool __result
            )
        {
            if (CustomAssetManager.RuntimeKeyToAssetInfo.ContainsKey(assetRef.RuntimeKey))
            {
                ____assetsLoaded.TryGetValue(assetRef.RuntimeKey, out AssetLoadingManager.AssetInfo info);
                if (info == null)
                {
                    Trainworks.Log(LogLevel.Debug, $"No asset entry for asset being unloaded {assetRef}");
                    __result = true;
                    return false;
                }
                if (info.Failed || info.Loading)
                {
                    Trainworks.Log(LogLevel.Debug, $"Trying to unload an asset that has failed to load or loading {assetRef}");
                    __result = true;
                    return false;
                }
                info.ownerCount--;
                if (info.ownerCount < 0)
                {
                    info.ownerCount = 0;
                }
                if (info.ownerCount == 0)
                {
                    if (info.instCount > 0)
                    {
                        Trainworks.Log(LogLevel.Warning, string.Format("Unloading asset {1} failed.  It has {0} instances in scene.  We expect that soon this instance will be destroyed so that this asset unloads.", info.instCount, assetRef.DebugName));
                        __result = false;
                        return false;
                    }

                    if (setAssetInfoAssetMethod == null)
                    {
                        setAssetInfoAssetMethod = AccessTools.Method(typeof(AssetLoadingManager), "SetAssetInfoAsset");
                    }

                    UnityEngine.Object.Destroy(info.asset);
                    setAssetInfoAssetMethod.Invoke(__instance, new object[] { info, null });
                    ____assetsLoaded.Remove(info.runtimeKey);
                    __result = true;
                }
                return false;
            }
            return true;
        }
    }
}
