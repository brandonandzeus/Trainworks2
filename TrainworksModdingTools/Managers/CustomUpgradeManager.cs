using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Trainworks.Managers;
using Trainworks.Utilities;

namespace Trainworks.ManagersV2
{
    public static class CustomUpgradeManager
    {
        public static IDictionary<string, CardUpgradeData> CustomUpgradeData = new Dictionary<string, CardUpgradeData>();

        public static void RegisterCustomUpgrade(CardUpgradeData upgrade)
        {
            if (!CustomUpgradeData.ContainsKey(upgrade.GetID()))
            {
                CustomUpgradeData.Add(upgrade.GetID(), upgrade);

                var upgradeList = ProviderManager.SaveManager.GetAllGameData().GetAllCardUpgradeData();
                var existingEntry = upgradeList.Where(u => upgrade.GetID() == u.GetID()).FirstOrDefault();

                if (existingEntry != null)
                {
                    upgradeList.Remove(existingEntry);
                }

                upgradeList.Add(upgrade);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate upgrade data with name: " + upgrade.name);
            }
        }

        /// <summary>
        /// Get the upgrade data corresponding to the given ID
        /// </summary>
        /// <param name="upgradeID">ID of the upgrade to get</param>
        /// <returns>The CardUpgradeData for the given ID</returns>
        public static CardUpgradeData GetCardUpgradeByID(string upgradeID)
        {
            // Search for custom mutator matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(upgradeID);
            if (CustomUpgradeData.TryGetValue(guid, out CardUpgradeData value))
            {
                return value;
            }

            // No custom upgrade found; search for vanilla upgrade matching ID
            var vanillaUpgrade = ProviderManager.SaveManager.GetAllGameData().FindCardUpgradeData(upgradeID);
            if (vanillaUpgrade == null)
            {
                Trainworks.Log(LogLevel.Warning, "Couldn't find upgrade: " + upgradeID + " - This will cause crashes.");
            }
            return vanillaUpgrade;
        }
    }
}
