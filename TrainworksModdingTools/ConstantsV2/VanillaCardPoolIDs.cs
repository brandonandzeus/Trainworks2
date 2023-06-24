namespace Trainworks.ConstantsV2
{
    /// <summary>
    /// Provides easy access to all of the base game's card pool IDs
    /// </summary>
    public static class VanillaCardPoolIDs
    {
        /// <summary>
        /// Card pool used for the Cov 1 random pool and end-of-battle card rewards, among other things
        /// </summary>
        public static readonly string MegaPool = "MegaPool";
        /// <summary>
        /// Card pool of cards that are ineligible for Stackstone (Doublestack).
        /// </summary>
        public static readonly string ExcludedFromStackstone = "CardsExcludedFromJuice";
        /// <summary>
        /// Card pool of monsters that can be selected for Conscription Notice.
        /// </summary>
        public static readonly string ConscriptUnitPool = "ConscriptUnitPool";
        /// <summary>
        /// Used for Unit Banner rewards from Trials.
        /// </summary>
        public static readonly string UnitsAllBanner = "UnitsAllBanner";
        /// <summary>
        /// Hellhorned banner pool
        /// </summary>
        public static readonly string HellhornedBanner = "UnitsHellhornedBanner";
        /// <summary>
        /// Awoken banner pool
        /// </summary>
        public static readonly string AwokenBanner = "UnitsAwokenBanner";
        /// <summary>
        /// Stygian banner pool
        /// </summary>
        public static readonly string StygianBanner = "UnitsStygianBanner";
        /// <summary>
        /// Umbra banner pool
        /// </summary>
        public static readonly string UmbraBanner = "UnitsUmbraBanner";
        /// <summary>
        /// Melting Remnant banner pool
        /// </summary>
        public static readonly string RemnantBanner = "UnitsRemnantBanner";
        /// <summary>
        /// Consume spells for Hellhorned for the "Abandoned Train" (ClassPotions) event.
        /// </summary>
        public static readonly string HellhornedConsumeables = "Class1Potions";
        /// <summary>
        /// Consume spells for Awoken for the "Abandoned Train" (ClassPotions) event.
        /// </summary>
        public static readonly string AwokenConsumeables = "Class2Potions";
        /// <summary>
        /// Consume spells for Stygian for the "Abandoned Train" (ClassPotions) event.
        /// </summary>
        public static readonly string StygianConsumeables = "Class3Potions";
        /// <summary>
        /// Consume spells for Umbra for the "Abandoned Train" (ClassPotions) event.
        /// </summary>
        public static readonly string UmbraConsumeables = "Class4Potions";
        /// <summary>
        /// Consume spells for Remnant for the "Abandoned Train" (ClassPotions) event.
        /// </summary>
        public static readonly string RemnantConsumeables = "Class5Potions";
        /// <summary>
        /// Consume spells for Wurmkin for the "Abandoned Train" (ClassPotions) event.
        /// </summary>
        public static readonly string WurmkinConsumeables = "Class6Potions";
        /// <summary>
        /// Pool containing all champions
        /// </summary>
        public static readonly string ChampionPool = "ChampionPool";
        /// <summary>
        /// Pool of champions for the New Challenger Mutator
        /// </summary>
        public static readonly string NewChallengerChampionPool = "NewChallengerChampionPool";
        /// <summary>
        /// Pool used when an effect randomly generates an imp
        /// </summary>
        public static readonly string ImpPool = "ImpPool";
        /// <summary>
        /// Pool used when an effect randomly generates a morsel
        /// </summary>
        public static readonly string MorselPool = "Class5Food";
        /// <summary>
        /// Pool used when one of the Umbra starter cards randomly generates a morsel
        /// </summary>
        public static readonly string MorselPoolStarter = "Class5StarterFoodCard";
        /// <summary>
        /// Unknown. Assumed to be for the card level up system, that appears to have been scrapped.
        /// Contains select units with "CardTraitLevelMonsterState" from each clan except Wurmkin.
        /// </summary>
        public static readonly string LevelableUnits = "LevelableUnits";
        /// <summary>
        /// Pool for Blazing Arrows from Umbra.
        /// </summary>
        public static readonly string UmbraBlazingArrows2 = "Class5BlazingArrows2";
        /// <summary>
        /// Pool for Blazing Arrows from Umbra.
        /// </summary>
        public static readonly string UmbraBlazingArrows3 = "Class5BlazingArrows3";
        /// <summary>
        /// Dante Pool for his mutator. Dante and 3 Dante's Candle.
        /// </summary>
        public static readonly string DanteMutatorPool = "DanteMutatorPool";
        /// <summary>
        /// Heph Pool for her mutator. Heph and Good ol' Wingmaker.
        /// </summary>
        public static readonly string HephMutatorPool = "HephMutatorPool";
        /// <summary>
        /// Scourge Pool Sinners Burden.
        /// </summary>
        public static readonly string JunkPoolT1 = "JunkPoolT1";
        /// <summary>
        /// Scourge Pool Weight of Contritution.
        /// </summary>
        public static readonly string JunkPoolT2 = "JunkPoolT2";
        /// <summary>
        /// Scourge Pool Self-Mutilation.
        /// </summary>
        public static readonly string JunkPoolT3 = "JunkPoolT3";
        /// <summary>
        /// Scourge Pool for The Ultimate Penance.
        /// </summary>
        public static readonly string JunkPoolUltimate = "JunkPoolUltimate";
        /// <summary>
        /// Used for SpreadingSpores effect of replicating itself.
        /// </summary>
        public static readonly string SpreadingSpores = "ReplicatingHeal";
        public static readonly string AutomaticRailspikes = "ReplicatingSpellPool";
        public static readonly string SpikedriverColony = "ReplicatingUnitPool";
        public static readonly string IgnoredFromNexusSpike = "SpellMergeIgnore";

        // Singular Card Pools.
        public static readonly string AdaptiveMutationOnlyPool = "AdaptiveMutationOnlyPool";
        public static readonly string CalcifiedEmberOnlyPool = "CalcifiedEmberOnlyPool";
        public static readonly string Class5MorselMinerOnly = "Class5MorselMinerOnly";
        public static readonly string DanteOnlyPool = "DanteOnlyPool";
        public static readonly string DantesCandleOnlyPool = "DantesCandleOnlyPool";
        public static readonly string DraffOnlyPool = "DraffOnlyPool";
        public static readonly string EelGorgonOnlyPool = "EelGorgonOnlyPool";
        public static readonly string ExcavatedEmberOnlyPool = "ExcavatedEmberOnlyPool";
        public static readonly string FledglingImpOnlyPool = "FledglingImpOnlyPool";
        public static readonly string HephOnlyPool = "HephOnlyPool";
        public static readonly string LodestoneOnlyPool = "LodestoneOnlyPool";
        public static readonly string ImpStarterOnlyPool = "ImpStarterOnlyPool";
        public static readonly string SoulSiphonOnlyPool = "SoulSiphonOnlyPool";
        public static readonly string StingOnlyPool = "StingOnlyPool";
        public static readonly string TrainStewardOnly = "TrainStewardOnly";
        public static readonly string UnleashTheWildwoodOnlyPool = "UnleashTheWildwoodOnlyPool";
        public static readonly string VengefulShardOnlyPool = "VengefulShardOnlyPool";
        public static readonly string VineGraspOnlyPool = "VineGraspOnlyPool";
        public static readonly string WelderHelperOnlyPool = "WelderHelperOnlyPool";
        /// <summary>
        /// Pool of only 2 Train Stewards for the Mutator.
        /// </summary>
        public static readonly string TrainSteward2 = "TrainSteward2";
    }
}
