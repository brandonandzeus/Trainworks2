using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trainworks.Utilities;

namespace Trainworks.Managers
{
    /// <summary>
    /// Manager for migration IDs to new ones.
    /// 
    /// This is in order to preserve SaveData / RunHistory.
    /// </summary>
    public static class GameDataMigrationManager
    {
        /// <summary>
        /// Forward Migrations
        /// </summary>
        internal static Dictionary<string, string> MigratedCardUpgradeIDs = new Dictionary<string, string>();


        /// <summary>
        /// Migrates an Existing CardUpgradeData to use a new ID. When CardUpgradeData is searched if an ID matches it will be replaced with the new ID specified preventing a CardUpgrade from not being found.
        /// </summary>
        /// <param name="oldID"></param>
        /// <param name="newID"></param>
        public static void MigrateCardUpgradeID(string oldID, string newID)
        {
            string oldGuid = GUIDGenerator.GenerateDeterministicGUID(oldID);
            string newGuid = GUIDGenerator.GenerateDeterministicGUID(newID);
            MigratedCardUpgradeIDs[oldGuid] = newGuid;
        }

        /// <summary>
        /// Please do not call this function unless you are migrating a CardUpgradeData off Builders.CardUpgradeDataBuilder to BuildersV2.CardUpgradeDataBuilder.
        /// 
        /// Old style CardUpgrades had their ID change as part of making conversion from old style CardUpgradeDataBuilder to BuildersV2.CardUpgradeDataBuilder easier w/o breaking RunHistory.
        /// oldID == UpgradeTitleKey -> GuidGenerator.GenerateDeterministicGUID(UpgradeTitleKey). Use UpgradeTitleKey as UpgradeID.
        /// </summary>
        public static void MigrateOldCardUpgradeID(string oldID, string guid)
        {
            MigratedCardUpgradeIDs[oldID] = guid;
        }
    }
}
