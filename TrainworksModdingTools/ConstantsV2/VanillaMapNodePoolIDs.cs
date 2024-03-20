namespace Trainworks.ConstantsV2
{
    /// <summary>
    /// Provides easy access to all of the base game's map node pool IDs
    /// Searchable in AssetStudio AssetName: MapNodeBucketListPrimary.
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
        /// Ticket count: Divine Temple (x3), Artifact (x1), Gold (x1)
        /// Enabled by Covenant 1. Disabled by No shards Mutator.
        /// </summary>
        public static readonly string PactNode = "DLC - Pact Nodes Bucket";
        ///<summary>
        /// Pact shard item found on ring 1: Gold or Artifact
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
        /// Reward Noide Rings 3-4 (w/ Ticket Count): Unit Draft Main/Sub (x2), Healing (x2)
        /// </summary>
        public static readonly string RewardsEarly = "Rewards Early";
        ///<summary>
        /// Reward Node Ring 2 (w/ Ticket Count): Unit Draft Main/Sub (x2), Gold (x3)
        /// </summary>
        public static readonly string RewardsFirstOnly = "Rewards First Only";
        ///<summary>
        /// Reward Node Ring 4: Gold or Card Purge.
        /// </summary>
        public static readonly string RewardsFirstSetExtra = "Rewards First Set Extra";
        ///<summary>
        /// Reward Node Rings 5-7 (w/ Ticket Count): Card Purges (x2), Gold (x4), Healing (x3), Artifact (x1), Cavern (x3)
        /// </summary>
        public static readonly string RewardsSecondSet = "Rewards Second Set";
        ///<summary>
        /// Reward Node Rings 5-7 (w/ Ticket Count): Card Purges (x2), Gold (x4), Healing (x3), Artifact (x1), Cavern (x3)
        /// </summary>
        public static readonly string RewardsSecondSetExtra = "Rewards Second Set - Extra";
        ///<summary>
        /// Reward Node Ring 8: Card Purges, Gold, Healing, or Duplication
        /// </summary>
        public static readonly string RewardsFinal = "Rewards Final";
        ///<summary>
        /// Ring 3 has a guaranteed Cavern Event that is accessible no matter the route you take.
        /// </summary>
        public static readonly string EventRing3 = "Event - Level 3";
        ///<summary>
        /// Ring 8 guarantees an Artifact Merchant.
        /// </summary>
        public static readonly string ArtifactMerchantRing8 = "Merchant - Level 7 Artifact Guaranteed" /*sic*/;
        ///<summary>
        /// Ring 8 Merchant: Guaranteed Merchant of Steel and Magic on this ring
        /// </summary>
        public static readonly string FinalMerchant = "Merchant - Final";
        ///<summary>
        /// Ring 5-7 Merchants. Available options (w/ Ticket Count): Artifact Merchant (x1), Merchant of Steel (x2), Merchant of Magic (x2), or Card Duplication (x1)
        /// </summary>
        public static readonly string LateMerchant = "Merchant - Late";
        ///<summary>
        /// Ring 3-4 Merchants. Available options: Merchant of Steel or Magic, an Artifact, or Card Duplication.
        /// </summary>
        public static readonly string EarlyMerchant = "Merchant - Early";
        ///<summary>
        /// Ring 2 Merchants. Merchant of Steel vs Merchant of Magic.
        /// </summary>
        public static readonly string FirstMerchant = "Merchant - First";
        /// <summary>
        /// The artifact freebie in limbo. (Not the Pact shard item).
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


        /// <summary>
        /// Do not use. Left in for compatibility. (Mispelled)
        /// </summary>
        public static readonly string FirstMerhant = "Merchant - First";
        ///<summary>
        /// Do not use. Left in for compatibility. (Wrong Ring Number)
        /// </summary>
        public static readonly string ArtifactMerchantRing7 = "Merchant - Level 7 Artifact Guaranteed" /*sic*/;
    }
}
