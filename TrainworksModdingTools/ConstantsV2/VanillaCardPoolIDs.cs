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
        /// Wurmkin banner pool.
        /// </summary>
        public static readonly string WurmkinBanner = "UnitsWurmBanner";
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
        /// 
        /// This is technically unused in the base game. The only use is in BlankPages
        /// however ParamCardPool is unused within RelicEffectAddChampionCardToHand.
        /// </summary>
        public static readonly string ChampionPool = "ChampionPool";
        /// <summary>
        /// Pool of champions for the New Challenger Mutator
        /// </summary>
        public static readonly string NewChallengerChampionPool = "NewChallengerChampionPool";
        /// <summary>
        /// Pool used when an effect randomly generates an imp
        /// 
        /// Used by Imp-in-a-Box and Imp-cicle.
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
        /// DO NOT USE. The only references to this card pool are for the preloading system in AssetLoadingData.
        /// No reason to use this as cards will be preloaded if they are in the MegaPool or UnitsAllBanner.
        /// The union of those two sets should cover all of the cards in your clan.
        /// 
        /// Unknown. Assumed to be for the card level up system, that appears to have been scrapped.
        ///
        /// Contents: Husk Hermit, Awoken Hollow, Animus of Will, Horned Warrior, Steelworker, Demon Fiend,
        /// Parrafin Thug, Lady of the Reformed, Wickless Baron, Siren of the Sea, Coldcaelia, Titan Sentry,
        /// Morsel Maker, Shadoweater, Crucible Warden.
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
        /// Used by the enemy Reconciler.
        /// </summary>
        public static readonly string JunkPoolT1 = "JunkPoolT1";
        /// <summary>
        /// Scourge Pool Weight of Contritution.
        /// 
        /// Absolver, Chains the Sighted, and Archus's Shattering Darkshard.
        /// </summary>
        public static readonly string JunkPoolT2 = "JunkPoolT2";
        /// <summary>
        /// Scourge Pool Self-Mutilation.
        /// 
        /// Purifier, Thoughtbinder.
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
        /// <summary>
        /// Used for Automatic Railspikes slay effect.
        /// 
        /// Probably don't want to touch this one.
        /// </summary>
        public static readonly string AutomaticRailspikes = "ReplicatingSpellPool";
        /// <summary>
        /// Used for Spikedriver Colony's Extinguish effect.
        /// 
        /// Probably don't want to touch this one.
        /// </summary>
        public static readonly string SpikedriverColony = "ReplicatingUnitPool";
        /// <summary>
        /// Ignored from Nexus Spike Pool.
        /// 
        /// Use to prevent your cards from being fused into Nexus Spike. Currently contains
        /// Nexus Spike, Cannibalize, ImpPressive, ImpProvisation, Olde Magic, Preserve
        /// Offering Token, Returned Soul, Broken Memories, Spreading Spores.
        /// </summary>
        public static readonly string IgnoredFromNexusSpike = "SpellMergeIgnore";

        
        /// <summary>
        /// Single Card CardPool containing Calcified Ember for the Junked Up Mutator.
        /// 
        /// Probably shouldn't touch this one unless you are messing with the associated mutator.
        /// </summary>
        public static readonly string CalcifiedEmberOnlyPool = "CalcifiedEmberOnlyPool";
        /// <summary>
        /// CardPool used by Making of a Miner. Contains Morsel Miner.
        /// 
        /// Probably don't want to touch this one.
        /// </summary>
        public static readonly string Class5MorselMinerOnly = "Class5MorselMinerOnly";
        /// <summary>
        /// For Dante's Comedy mutator. Allows Dante to have Intrinsic.
        /// </summary>
        public static readonly string DanteOnlyPool = "DanteOnlyPool";
        /// <summary>
        /// Dante's Candle CardPool. Used for Dante's Essence.
        /// 
        /// Probably don't want to mess with this one.
        /// </summary>
        public static readonly string DantesCandleOnlyPool = "DantesCandleOnlyPool";
        /// <summary>
        /// Used by Sacrificial Resurrection. Singular CardPool containing Draff.
        /// 
        /// You shouldn't add to this card pool unless intending on modifying the afforementioned card.
        /// Otherwise the GeneratedTooltip will not show up (see CardEffectState.GetGeneratedCardDataForTooltip)
        /// </summary>
        public static readonly string DraffOnlyPool = "DraffOnlyPool";
        /// <summary>
        /// Pool containing only Eel Gorgon.
        /// 
        /// Want to prevent a unit from receiving status effect upgrades from shop upgrades?
        /// Add it to this CardPool.
        /// 
        /// Addition will prevent the unit from being eligible from:
        /// Furystone, Wickstone, Thornstone, Shieldstone, Runestone, Frenzystone,
        /// Immortalstone, and Speedstone.
        /// </summary>
        public static readonly string EelGorgonOnlyPool = "EelGorgonOnlyPool";
        /// <summary>
        /// Singular CardPool used by Ember Cache.
        /// 
        /// You shouldn't add to this card pool unless intending on modifying the afforementioned card.
        /// Otherwise the GeneratedTooltip will not show up (see CardEffectState.GetGeneratedCardDataForTooltip)
        /// </summary>
        public static readonly string ExcavatedEmberOnlyPool = "ExcavatedEmberOnlyPool";
        /// <summary>
        /// Gives Heph Intrinsic in Mutator If I had a Hammer.
        /// 
        /// Probably don't want to touch this.
        /// </summary>
        public static readonly string HephOnlyPool = "HephOnlyPool";
        /// <summary>
        /// Used by Mutator Heavy.
        /// 
        /// A singular CardPool containing the Deadweight Blight.
        /// You shouldn't add to the card pool unless intending on modifying the afforementioned mutator.
        /// </summary>
        public static readonly string LodestoneOnlyPool = "LodestoneOnlyPool";
        /// <summary>
        /// Singular CardPool used by Shardtail Queen Imp Parade I
        /// </summary>
        public static readonly string ImpStarterOnlyPool = "ImpStarterOnlyPool";
        /// <summary>
        /// Singular CardPool used by Shardtail Queen Imp Parade II, III.
        /// </summary>
        public static readonly string FledglingImpOnlyPool = "FledglingImpOnlyPool";
        /// <summary>
        /// Singular CardPool used by Shardtail Queen Imp Parade III.
        /// </summary>
        public static readonly string WelderHelperOnlyPool = "WelderHelperOnlyPool";
        /// <summary>
        /// Singular CardPool used by Stackstone.
        /// 
        /// Better to use the CardsExcludedFromJuice for futureproofing, but both CardPools
        /// restrict cards from DoubleStack.
        /// </summary>
        public static readonly string SoulSiphonOnlyPool = "SoulSiphonOnlyPool";
        /// <summary>
        /// Singular CardPool used by multiple effects. Contains only Sting.
        /// 
        /// You shouldn't add to this CardPool since many effects use it.
        /// 
        /// Wyldenten (Thornlord), Vinemother, Preserved Thorns, Thorn Fruit which all add Sting spells.
        /// Thorn Casing (Sting spells +10 damage and pierce).
        /// Channelheart (Sting spells +20 damage).
        /// </summary>
        public static readonly string StingOnlyPool = "StingOnlyPool";
        /// <summary>
        /// Used by the CardPulls to build the starter deck.
        /// 
        /// Only contains TrainSteward, probably don't want to modify this one.
        /// </summary>
        public static readonly string TrainStewardOnly = "TrainStewardOnly";
        /// <summary>
        /// Unleash the Wildwood. 
        /// 
        /// Adding to this CardPool restricts a spell from receiving magic power upgrades
        /// Powerstone, Surgestone, Spell Railspike, Harness The Titan, Extremestone, and Truestone
        /// use this CardPool.
        /// </summary>
        public static readonly string UnleashTheWildwoodOnlyPool = "UnleashTheWildwoodOnlyPool";
        /// <summary>
        /// Adaptive Mutation CardPool.
        /// 
        /// Adding to this CardPool restricts a spell from receiving magic power upgrades
        /// Powerstone, Surgestone, Spell Railspike, Harness The Titan, Extremestone, and Truestone
        /// use this CardPool.
        /// </summary>
        public static readonly string AdaptiveMutationOnlyPool = "AdaptiveMutationOnlyPool";
        /// <summary>
        /// CardPool containing only Vengeful Shard
        /// 
        /// Used by Seraph the Diligent, and Highpriest to the Light and Chains the Sighted (in the TLD fight).
        /// </summary>
        public static readonly string VengefulShardOnlyPool = "VengefulShardOnlyPool";
        /// <summary>
        /// CardPool containing only VineGrasp.
        /// 
        /// Used only by the Cursed Vines Artifact, probablyt don't want to modify this one.
        /// </summary>
        public static readonly string VineGraspOnlyPool = "VineGraspOnlyPool";

        /// <summary>
        /// Pool of only 2 Train Stewards for the Mutator At your service.
        /// </summary>
        public static readonly string TrainSteward2 = "TrainSteward2";
    }
}
