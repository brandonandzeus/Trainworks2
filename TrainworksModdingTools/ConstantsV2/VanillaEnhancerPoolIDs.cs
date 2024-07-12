namespace Trainworks.ConstantsV2
{
    /// <summary>
    /// Provides easy access to all of the base game's enhancer pool IDs
    /// </summary>
    public static class VanillaEnhancerPoolIDs
    {
        /// <summary>
        /// The first slot in the Spell Upgrade Shop, always contains a -1 Ember upgrade.
        /// </summary>
        public static readonly string SpellUpgradePoolCostReduction = "SpellUpgradePoolCostReduction";
        /// <summary>
        /// The second slot in the Spell Upgrade Shop. Contains 'Magic Power +10' and 'Consume, Magic Power +20'. 
        /// </summary>
        public static readonly string SpellUpgradePoolCommon = "SpellUpgradePoolCommon";
        /// <summary>
        /// The third slot in the Spell Upgrade Shop. Contains Holdover, remove Consume, Permafrost and Doublestack. Permafrost is Stygian themed but not limited.
        /// </summary>
        public static readonly string SpellUpgradePool = "SpellUpgradePool";
        /// <summary>
        /// The first two slots in the Unit Upgrade Shop. Contains +10 attack, +25 Health, and +10/+10. Also contains a class specific upgrade for your main and subclass.
        /// </summary>
        public static readonly string UnitUpgradePoolCommon = "UnitUpgradePoolCommon";
        /// <summary>
        /// The last slots in the Unit Upgrade Shop. Contains themed rare upgrades available for all classes. Multistrike (Hellhorned), Quick (Awoken), Largestone (Umbra) and Endless (Remnant). Stygian is in rare Spell upgrades.
        /// </summary>
        public static readonly string UnitUpgradePool = "UnitUpgradePool";
        /// <summary>
        /// The common slot in the Divine Temple. Intrinsic, + 10 and Pierce, and -1 Cost and Purge are here.
        /// </summary>
        public static readonly string SpellUpgradePoolDarkPactCommon = "SpellUpgradePoolDarkPactCommon";
        /// <summary>
        /// The uncommon slot in the Divine Temple. The +30 Magic Power, -2 Cost, and Spellchain are here. 
        /// </summary>
        public static readonly string SpellUpgradePoolDarkPactUncommon = "SpellUpgradePoolDarkPactUncommon";
        /// <summary>
        /// An Enhancer Pool used by CapriciousReflection and Mutator UpgradedDrafts.
        /// Contents: Powerstone Surgestone Emberstone x2 Stackstone Freezestone Keepstone Eternalstone Immortalstone x3 
        ///           Largestone x3 Thornstone x2 Furystone x2 Wickstone Runestone x2 Shieldstone x2 Frenzystone x3 Speedstone x3
        ///           Strengthstone x2 Battlestone x2 Heartstone x2
        /// </summary>
        public static readonly string DraftUpgradePool = "DraftUpgradePool";
        /// <summary>
        /// EnhancerPool used by Wurmkin (in ClassData) to apply Infused to a random card draft.
        /// Contents: Corruptstone
        /// </summary>
        public static readonly string WurmkinInfusedPool = "Class6CorruptEnhancerPool";
    }
}
