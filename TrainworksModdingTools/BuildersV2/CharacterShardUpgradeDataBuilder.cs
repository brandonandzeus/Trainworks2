using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.BuildersV2
{
    /// <summary>
    /// Specifies shard upgrades for enemy characters
    /// </summary>
    public class CharacterShardUpgradeDataBuilder
    {
        /// <summary>
        /// Indicates that the upgrade can be applied repeatedly.
        /// </summary>
        public bool IsRepeatable { get; set; }
        /// <summary>
        /// Indicates that also a Covenent Level is required.
        /// </summary>
        public bool CovenantUpgrade { get; set; }
        /// <summary>
        /// Required Covenant Level.
        /// </summary>
        public int RequiredCovenantLevel { get; set; }
        /// <summary>
        /// Number of Pact Shards required.
        /// </summary>
        public int CrystalsCount { get; set; }
        /// <summary>
        /// Upgrade to apply to the character.
        /// </summary>
        public CardUpgradeData UpgradeData { get; set; }
        /// <summary>
        /// Convenience Builder for UpgradeData. If set the value of UpgradeData is ignored.
        /// </summary>
        public CardUpgradeDataBuilder UpgradeDataBuilder { get; set; }

        public CharacterShardUpgradeDataBuilder()
        {
            IsRepeatable = true;
            RequiredCovenantLevel = 10;
            CrystalsCount = 10;
        }

        public CharacterShardUpgradeData Build()
        {
            CharacterShardUpgradeData shardUpgrade = new CharacterShardUpgradeData();

            CardUpgradeData upgradeData = UpgradeData;
            if (UpgradeDataBuilder != null)
            {
                upgradeData = UpgradeDataBuilder.Build();
            }

            AccessTools.Field(typeof(CharacterShardUpgradeData), "isRepeatable").SetValue(shardUpgrade, IsRepeatable);
            AccessTools.Field(typeof(CharacterShardUpgradeData), "covenantUpgrade").SetValue(shardUpgrade, CovenantUpgrade);
            AccessTools.Field(typeof(CharacterShardUpgradeData), "requiredCovenantLevel").SetValue(shardUpgrade, RequiredCovenantLevel);
            AccessTools.Field(typeof(CharacterShardUpgradeData), "crystalsCount").SetValue(shardUpgrade, CrystalsCount);
            AccessTools.Field(typeof(CharacterShardUpgradeData), "upgradeData").SetValue(shardUpgrade, upgradeData);

            return shardUpgrade;
        }
    }
}
