using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;

namespace Trainworks.Patches
{
    /// <summary>
    /// SaveData/RunHistory Preservation patches.
    /// 
    /// Currently due to a design flaw in Builders.CardUpgradeDataBuilder (not generating GUIDs) the progress of rewriting to use the BuildersV2 namespace will break RunHistory for older runs.
    /// This patch is provided to prevent a case where a null CardUpgradeData is returned.
    /// </summary>
    [HarmonyPatch(typeof(AllGameData), nameof(AllGameData.FindCardUpgradeData))]
    public class GameDataMigrationPatches
    {
        public static void Prefix(ref string id)
        {
            if (GameDataMigrationManager.MigratedCardUpgradeIDs.ContainsKey(id))
            {
                id = GameDataMigrationManager.MigratedCardUpgradeIDs[id];
            }
        }
    }
}
