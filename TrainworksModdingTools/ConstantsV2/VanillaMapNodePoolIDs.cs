namespace Trainworks.ConstantsV2
{
    /// <summary>
    /// Provides easy access to all of the base game's map node pool IDs
    /// </summary>
    public static class VanillaMapNodePoolIDs
    {
        // Note these items are a RandomMapDataContainer

        /// <summary>
        /// Special MapNodeID that holds all the clans' banners, but selects only the one corresponding to the current primary clan
        /// This is a RandomMapDataContainer a subclass of MapNodeData
        /// </summary>
        public static readonly string RandomChosenMainClassUnit = "RandomChosenMainClassUnit";
        /// <summary>
        /// Special MapNodeID that holds all the clans' banners, but selects only the one corresponding to the current allied clan
        /// This is a RandomMapDataContainer a subclass of MapNodeData
        /// </summary>
        public static readonly string RandomChosenSubClassUnit = "RandomChosenSubClassUnit";

        // Note the following items are of type MapNodeBucketContainer, true MapNodePools.

        ///<summary>
        /// Found in various rings other than limbo. Contains a Temple, Artifact, or Gold.
        /// Enabled by Covenant 1. Disabled by No shards Mutator.
        /// </summary>
        public static readonly string PactNode = "DLC - Pact Nodes Bucket";
        ///<summary>
        /// Found in various rings including limbo. Contains an Artifact, or Gold.
        /// Enabled by Covenant 1. Disabled by No shards Mutator.
        /// </summary>
        public static readonly string PactNodeGoldOrArtifact = "DLC - Dark Pact Gold or Artifact";
        ///<summary>
        /// Banner unit in limbo enabled by Luxury in Limbo (UnitBannersInLimbo) mutator.
        /// </summary>
        public static readonly string LimboBannerSub = "Rewards Limbo Mutator 2";
        ///<summary>
        /// Banner unit in limbo enabled by Luxury in Limbo (UnitBannersInLimbo) mutator.
        /// </summary>
        public static readonly string LimboBannerMain = "Rewards Limbo Mutator 1";
        ///<summary>
        ///
        /// </summary>
        public static readonly string RewardsEarly = "Rewards Early";
        ///<summary>
        ///
        /// </summary>
        public static readonly string RewardsFirstOnly = "Rewards First Only";
        ///<summary>
        ///
        /// </summary>
        public static readonly string RewardsFirstSetExtra = "Rewards First Set Extra";
        ///<summary>
        ///
        /// </summary>
        public static readonly string RewardsSecondSet = "Rewards Second Set";
        ///<summary>
        ///
        /// </summary>
        public static readonly string RewardsSecondSetExtra = "Rewards Second Set - Extra";
        ///<summary>
        ///
        /// </summary>
        public static readonly string RewardsFinal = "Rewards Final";
        ///<summary>
        /// Ring 3 has a guaranteed Cavern Event that is accessible no matter the route you take.
        /// </summary>
        public static readonly string EventRing3 = "Event - Level 3";
        ///<summary>
        /// Ring 7 guarantees an Artifact Merchant.
        /// </summary>
        public static readonly string ArtifactMerchantRing7 = "Merchant - Level 7 Artifact Guaranteed";
        ///<summary>
        ///
        /// </summary>
        public static readonly string FinalMerchant = "Merchant - Final";
        ///<summary>
        ///
        /// </summary>
        public static readonly string LateMerchant = "Merchant - Late";
        ///<summary>
        ///
        /// </summary>
        public static readonly string EarlyMerchant = "Merchant - Early";
        ///<summary>
        ///
        /// </summary>
        public static readonly string FirstMerhant = "Merchant - First";
        /// <summary>
        /// The freebie in limbo. (Not the Pact shard item).
        /// </summary>
        public static readonly string StarterBlessing = "Starter Blessing - Level 1";
        /// <summary>
        /// Third champion upgrade in Ring 7.
        /// The PurgeChampion mutator is the only mutator that disables this.
        /// </summary>
        public static readonly string ChampionUpgradeIII = "Third Champion Upgrade - Level 6" /*sic*/;
        /// <summary>
        /// Second champion upgrade in Ring 4.
        /// The PurgeChampion mutator is the only mutator that disables this.
        /// </summary>
        public static readonly string ChampionUpgradeII = "Second Champion Upgrade - Level 4";
        /// <summary>
        /// First champion upgrade in Limbo.
        /// The PurgeChampion mutator is the only mutator that disables this.
        /// </summary>
        public static readonly string ChampionUpgradeI = "First Champion Upgrade - Level 1";
    }
}
