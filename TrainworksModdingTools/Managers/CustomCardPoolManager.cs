using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Trainworks.ConstantsV2;
using UnityEngine;

namespace Trainworks.Managers
{
    public class CustomCardPoolManager
    {
        /// <summary>
        /// Maps custom card pool IDs to their actual CardPool instances.
        /// </summary>
        public static IDictionary<string, CardPool> CustomCardPools { get; } = new Dictionary<string, CardPool>();

        /// <summary>
        /// Cache of Vanilla Card Pools.
        /// </summary>
        internal static Dictionary<string, CardPool> VanillaCardPools = new Dictionary<string, CardPool>();
        // TODO: Remove when GetCardsForPool is no longer used.
        internal static List<CardData> EMPTY = new List<CardData>();

        /// <summary>
        /// Gets the Card MegaPool instance which contains every card in the game.
        /// </summary>
        public static CardPool GetMegaPool()
        {
            return VanillaCardPools[VanillaCardPoolIDs.MegaPool];
        }

        /// <summary>
        /// Gets the Constriction Unit Pool used by the Conscription Notice artifact.
        /// </summary>
        /// <returns></returns>
        public static CardPool GetConscriptUnitPool()
        {
            return VanillaCardPools[VanillaCardPoolIDs.ConscriptUnitPool];
        }

        public static CardPool GetVanillaCardPool(string name)
        {
            return VanillaCardPools[name];
        }

        /// <summary>
        /// Registers a Custom Card Pool.
        /// </summary>
        /// <param name="cardPool"></param>
        public static void RegisterCustomCardPool(CardPool cardPool)
        {
            if (VanillaCardPools.ContainsKey(cardPool.name))
            {
                Trainworks.Log(LogLevel.Warning, "Card Pool name" + cardPool.name + " is reserved");
            }
            if (!CustomCardPools.ContainsKey(cardPool.name))
            {
                CustomCardPools.Add(cardPool.name, cardPool);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate card pool with name: " + cardPool.name);
            }
        }

        /// <summary>
        /// Add the card to the card pools with given IDs.
        /// </summary>
        /// <param name="cardData">CardData to be added to the pools</param>
        /// <param name="cardPoolIDs">List of card pool IDs to add the card to</param>
        public static void AddCardToPools(CardData cardData, List<string> cardPoolIDs)
        {
            foreach (string cardPoolID in cardPoolIDs)
            {
                CardPool cardPool = null;
                if (VanillaCardPools.ContainsKey(cardPoolID))
                {
                    cardPool = VanillaCardPools[cardPoolID];
                    if (cardPool == null)
                    {
                        Trainworks.Log(LogLevel.Warning, "CardPool " + cardPoolID + " is unsupported, ignoring.");
                        continue;
                    }
                }
                else if (CustomCardPools.ContainsKey(cardPoolID))
                {
                    cardPool = CustomCardPools[cardPoolID];
                }
                // Apparently people don't use CardPoolBuilder and register a CardPool beforehand.
                if (cardPool == null)
                {
                    Trainworks.Log(LogLevel.Warning, "Could not find card pool: " + cardPoolID + ". Creating a card pool");
                    cardPool = ScriptableObject.CreateInstance<CardPool>();
                    cardPool.name = cardPoolID;
                    CustomCardPools.Add(cardPoolID, cardPool);
                }

                var cardDataList = (Malee.ReorderableArray<CardData>)AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(cardPool);
                cardDataList.Add(cardData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardPoolID"></param>
        /// <returns></returns>
        public static CardPool GetCustomCardPoolByID(string cardPoolID)
        {
            if (CustomCardPools.ContainsKey(cardPoolID))
            {
                return CustomCardPools[cardPoolID];
            }
            Trainworks.Log(LogLevel.Error, "Could not find card pool: " + cardPoolID);
            Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            return null;
        }

        
        // TODO remove once CustomClanHelper doesn't use this anymore.
        [Obsolete("UNSUPPORTED DO NOT USE. Call GetCustomCardPoolByID(id) or GetVanillaCardPool(id) then GetAllChoices(). Cards are directly added to the CardPools they belong to so this function doesn't do anything", true)]
        public static List<CardData> GetCardsForPool(string pool)
        {
            return EMPTY;
        }

        /// <summary>
        /// Marks a custom card pool for preloading when a run starts.
        /// This is necessary if for instance you have an artifact that has the RelicEffectAddBattleCard or RelicEffectSpawnRandomUnitStartOfCombat.
        /// 
        /// As of Trainworks 2.5.0 You don't need to call this function for cards within the MegaPool or UnitsAllBanner.
        ///
        /// You don't need to call this function for card unlocks, these are done automatically. Card Unlocks also aren't preloaded.
        /// 
        /// Note it doesn't make sense to set both game_assets and clan_assets to true.
        /// </summary>
        /// <param name="cardPoolID">The card pool ID</param>
        /// <param name="clan_assets">Mark the card pool as one containing clan assets. These cards will only be loaded if the clan is selected for a run.</param>
        /// <param name="game_assets">Mark the card pool as one containing general game assets. These cards will always be loaded (Useful for things like Custom Blights or Scourges).</param>
        public static void MarkCardPoolForPreloading(string cardPoolID, bool clan_assets = true, bool game_assets = false)
        {
            MarkCardPoolForPreloading(CustomCardPools[cardPoolID], clan_assets, game_assets);
        }

        /// <summary>
        /// Marks a custom card pool for preloading when a run starts.
        /// This is necessary if for instance you have an artifact that has the RelicEffectAddBattleCard or RelicEffectSpawnRandomUnitStartOfCombat.
        /// 
        /// As of Trainworks 2.5.0 You don't need to call this function for cards within the MegaPool or UnitsAllBanner.
        ///
        /// You don't need to call this function for card unlocks, these are done automatically. Card Unlocks also aren't preloaded.
        /// 
        /// Note it doesn't make sense to set both game_assets and clan_assets to true.
        /// </summary>
        /// <param name="cardPool">The card pool</param>
        /// <param name="clan_assets">Mark the card pool as one containing clan assets. These cards will only be loaded if the clan is selected for a run.</param>
        /// <param name="game_assets">Mark the card pool as one containing general game assets. These cards will always be loaded (Useful for things like Custom Blights or Scourges).</param>
        public static void MarkCardPoolForPreloading(CardPool cardPool, bool clan_assets = true, bool game_assets = false)
        {
            if (cardPool == null)
            {
                Trainworks.Log(LogLevel.Error, "Attempted to mark a null CardPool for preloading");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
                return;
            }

            var assetLoadingManager = AssetLoadingManager.GetInst();
            var assetLoadingData = (AssetLoadingData)AccessTools.Field(typeof(AssetLoadingManager), "_assetLoadingData").GetValue(assetLoadingManager);
            if (game_assets)
            {
                assetLoadingData.CardPoolsAlwaysLoad.Add(cardPool);
            }
            else if (clan_assets)
            {
                assetLoadingData.CardPoolsAll.Add(cardPool);
            }
        }

        internal static void GatherAllVanillaCardPools()
        {

            var allGameData = ProviderManager.SaveManager.GetAllGameData();


            VanillaCardPools.Add(VanillaCardPoolIDs.MegaPool, (allGameData.FindRewardData(VanillaRewardIDs.CardDraftMainClassReward) as DraftRewardData)?.GetDraftPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.ConscriptUnitPool, allGameData.FindCollectableRelicData(VanillaCollectableRelicIDs.ConscriptionNotice).GetFirstRelicEffectData<RelicEffectAddBattleCardToHandOnUnitTrigger>()?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.UnitsAllBanner, (allGameData.FindRewardData(VanillaRewardIDs.CardDraftLevelUpUnitMainOrAllied) as DraftRewardData)?.GetDraftPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.ChampionPool, allGameData.FindCollectableRelicData(VanillaCollectableRelicIDs.BlankPages)?.GetFirstRelicEffectData<RelicEffectAddChampionCardToHand>()?.GetParamCardPool());


            VanillaCardPools.Add(VanillaCardPoolIDs.HellhornedBanner, (allGameData.FindRewardData(VanillaRewardIDs.CardDraftLevelUpUnitHellhorned) as DraftRewardData)?.GetDraftPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.AwokenBanner, (allGameData.FindRewardData(VanillaRewardIDs.CardDraftLevelUpUnitAwoken) as DraftRewardData)?.GetDraftPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.StygianBanner, (allGameData.FindRewardData(VanillaRewardIDs.CardDraftLevelUpUnitStygian) as DraftRewardData)?.GetDraftPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.UmbraBanner, (allGameData.FindRewardData(VanillaRewardIDs.CardDraftLevelUpUnitUmbra) as DraftRewardData)?.GetDraftPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.RemnantBanner, (allGameData.FindRewardData(VanillaRewardIDs.CardDraftLevelUpUnitRemnant) as DraftRewardData)?.GetDraftPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.WurmkinBanner, (allGameData.FindRewardData(VanillaRewardIDs.CardDraftLevelUpUnitWurm) as DraftRewardData)?.GetDraftPool());


            var rewards = allGameData.FindStoryEventData(VanillaStoryEventIDs.ClassPotions)?.GetPossibleRewards();
            // HellhornedConsumeables (Class1Potions) - WurmkinConsumeables (Class6Potions).
            foreach (var reward in rewards)
            {
                var cprdata = reward as CardPoolRewardData;
                var cardPool = AccessTools.Field(typeof(CardPoolRewardData), "cardPool").GetValue(cprdata) as CardPool;
                VanillaCardPools.Add(cardPool.name, cardPool);
            }

            VanillaCardPools.Add(VanillaCardPoolIDs.NewChallengerChampionPool, allGameData.FindMutatorData(VanillaMutatorIDs.ANewChallenger)?.GetFirstRelicEffectData<RelicEffectReplaceChampion>()?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.ImpPool, allGameData.FindCollectableRelicData(VanillaCollectableRelicIDs.Impcicle)?.GetFirstRelicEffectData<RelicEffectAddBattleCardToHand>()?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.MorselPool, allGameData.FindCollectableRelicData(VanillaCollectableRelicIDs.Shadelamp)?.GetFirstRelicEffectData<RelicEffectAddBattleCardToHandOnUnitTrigger>()?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.MorselPoolStarter, allGameData.FindCollectableRelicData(VanillaCollectableRelicIDs.AbandonedAntumbra)?.GetFirstRelicEffectData<RelicEffectAddBattleCardToHand>()?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.Class5MorselMinerOnly, allGameData.FindCardData(VanillaCardIDs.MakingofaMorsel)?.GetEffects()[0]?.GetParamCardPool());

            var assetLoadingManager = AssetLoadingManager.GetInst();
            var assetLoadingData = (AssetLoadingData)AccessTools.Field(typeof(AssetLoadingManager), "_assetLoadingData").GetValue(assetLoadingManager);
            foreach (var pool in assetLoadingData.CardPoolsAll)
            {
                if (pool.name == VanillaCardPoolIDs.LevelableUnits)
                {
                    VanillaCardPools.Add(VanillaCardPoolIDs.LevelableUnits, pool);
                    break;
                }
            }

            VanillaCardPools.Add(VanillaCardPoolIDs.UmbraBlazingArrows2, allGameData.FindCardData(VanillaCardIDs.BlazingBolts)?.GetEffects()[1]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.UmbraBlazingArrows3, allGameData.FindCardData(VanillaCardIDs.BlazingBolts2)?.GetEffects()[2]?.GetParamCardPool());

            VanillaCardPools.Add(VanillaCardPoolIDs.DanteMutatorPool, allGameData.FindMutatorData(VanillaMutatorIDs.DantesComedy)?.GetFirstRelicEffectData<RelicEffectAddCardsStartOfRun>()?.GetParamCardPool());
            var mask = allGameData.FindMutatorData(VanillaMutatorIDs.DantesComedy)?.GetFirstRelicEffectData<RelicEffectAddTempUpgrade>()?.GetParamCardUpgradeData()?.GetFilters()[0];
            var pools = AccessTools.Field(typeof(CardUpgradeMaskData), "allowedCardPools").GetValue(mask) as List<CardPool>;
            VanillaCardPools.Add(VanillaCardPoolIDs.DanteOnlyPool, pools[0]);
            VanillaCardPools.Add(VanillaCardPoolIDs.HephMutatorPool, allGameData.FindMutatorData(VanillaMutatorIDs.IfIhadaHammer)?.GetFirstRelicEffectData<RelicEffectAddCardsStartOfRun>()?.GetParamCardPool());
            mask = allGameData.FindMutatorData(VanillaMutatorIDs.IfIhadaHammer)?.GetFirstRelicEffectData<RelicEffectAddTempUpgrade>()?.GetParamCardUpgradeData()?.GetFilters()[0];
            pools = AccessTools.Field(typeof(CardUpgradeMaskData), "allowedCardPools").GetValue(mask) as List<CardPool>;
            VanillaCardPools.Add(VanillaCardPoolIDs.HephOnlyPool, pools[0]);


            VanillaCardPools.Add(VanillaCardPoolIDs.JunkPoolT1, CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.Reconciler)?.GetTriggers()[0]?.GetEffects()[0]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.JunkPoolT2, CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.Absolver)?.GetTriggers()[0]?.GetEffects()[0]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.JunkPoolT3, CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.Purifier)?.GetTriggers()[0]?.GetEffects()[0]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.JunkPoolUltimate, CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.FeltheWingsofLightJunk)?.GetBossActionData()[0]?.GetActions()[0]?.GetActionEffectData()[0]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.SpreadingSpores, allGameData.FindCardData(VanillaCardIDs.SpreadingSpores)?.GetEffects()[2]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.AutomaticRailspikes, allGameData.FindCardData(VanillaCardIDs.AutomaticRailspikes)?.GetUpgradeData()[0]?.GetCardTriggerUpgrades()[0]?.GetCardEffects()[0]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.SpikedriverColony, allGameData.FindCardData(VanillaCardIDs.SpikedriverColony)?.GetUpgradeData()[0]?.GetTriggerUpgrades()[0]?.GetEffects()[0]?.GetParamCardPool());

            var smrd = (allGameData.FindRewardData(VanillaRewardIDs.SpellMergeReward) as SpellMergeRewardData);
            VanillaCardPools.Add(VanillaCardPoolIDs.IgnoredFromNexusSpike, AccessTools.Field(typeof(SpellMergeRewardData), "cardsToIgnore")?.GetValue(smrd) as CardPool);

            mask = allGameData.FindCardUpgradeData(VanillaCardUpgradeDataIDs.Powerstone)?.GetFilters()[1];
            pools = AccessTools.Field(typeof(CardUpgradeMaskData), "disallowedCardPools").GetValue(mask) as List<CardPool>;
            // UnleashTheWildwoodOnlyPool, AdaptiveMutationOnlyPool
            foreach (var pool in pools)
            {
                VanillaCardPools.Add(pool.name, pool);
            }


            VanillaCardPools.Add(VanillaCardPoolIDs.CalcifiedEmberOnlyPool, allGameData.FindMutatorData(VanillaMutatorIDs.JunkedUp)?.GetFirstRelicEffectData<RelicEffectAddCardsStartOfRun>()?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.DantesCandleOnlyPool, allGameData.FindCardUpgradeData(VanillaCardUpgradeDataIDs.MonsterDanteSynthesis)?.GetTriggerUpgrades()[0]?.GetEffects()[0]?.GetParamCardPool());

            VanillaCardPools.Add(VanillaCardPoolIDs.DraffOnlyPool, allGameData.FindCardData(VanillaCardIDs.SacrificialResurrection)?.GetEffects()[1]?.GetParamCardPool());
            mask = allGameData.FindCardUpgradeData(VanillaCardUpgradeDataIDs.Furystone)?.GetFilters()[0];
            pools = AccessTools.Field(typeof(CardUpgradeMaskData), "disallowedCardPools")?.GetValue(mask) as List<CardPool>;
            VanillaCardPools.Add(VanillaCardPoolIDs.EelGorgonOnlyPool, pools[0]);
            VanillaCardPools.Add(VanillaCardPoolIDs.ExcavatedEmberOnlyPool, allGameData.FindCardData(VanillaCardIDs.EmberCache)?.GetEffects()[0]?.GetParamCardPool());

            VanillaCardPools.Add(VanillaCardPoolIDs.ImpStarterOnlyPool, allGameData.FindCardUpgradeData(VanillaCardUpgradeDataIDs.ImpParade)?.GetTriggerUpgrades()[0]?.GetEffects()[0]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.FledglingImpOnlyPool, allGameData.FindCardUpgradeData(VanillaCardUpgradeDataIDs.ImpParadeII)?.GetTriggerUpgrades()[0]?.GetEffects()[0]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.WelderHelperOnlyPool, allGameData.FindCardUpgradeData(VanillaCardUpgradeDataIDs.ImpParadeIII)?.GetTriggerUpgrades()[0]?.GetEffects()[1]?.GetParamCardPool());

            VanillaCardPools.Add(VanillaCardPoolIDs.LodestoneOnlyPool, allGameData.FindMutatorData(VanillaMutatorIDs.Heavy)?.GetFirstRelicEffectData<RelicEffectAddCardsStartOfRun>()?.GetParamCardPool());

            mask = allGameData.FindCardUpgradeData(VanillaCardUpgradeDataIDs.Stackstone)?.GetFilters()[4];
            pools = AccessTools.Field(typeof(CardUpgradeMaskData), "disallowedCardPools").GetValue(mask) as List<CardPool>;
            VanillaCardPools.Add(VanillaCardPoolIDs.ExcludedFromStackstone, pools[0]);
            mask = allGameData.FindCardUpgradeData(VanillaCardUpgradeDataIDs.Stackstone)?.GetFilters()[5];
            pools = AccessTools.Field(typeof(CardUpgradeMaskData), "disallowedCardPools").GetValue(mask) as List<CardPool>;
            VanillaCardPools.Add(VanillaCardPoolIDs.SoulSiphonOnlyPool, pools[0]);

            VanillaCardPools.Add(VanillaCardPoolIDs.StingOnlyPool, allGameData.FindCollectableRelicData(VanillaCollectableRelicIDs.Thornfruit)?.GetFirstRelicEffectData<RelicEffectAddBattleCardToHand>()?.GetParamCardPool());
            var csb = allGameData.FindCovenantData(VanillaCovenentIDs.Ascension01StrongerEnemies)?.GetFirstRelicEffectData<RelicEffectAddCardSetStartOfRun>()?.GetParamCardSetBuilder();
            var cpulls = AccessTools.Field(typeof(CardSetBuilder), "paramCardPulls").GetValue(csb) as List<CardPull>;
            VanillaCardPools.Add(VanillaCardPoolIDs.TrainStewardOnly, AccessTools.Field(typeof(CardPull), "cardPool").GetValue(cpulls[2]) as CardPool);

            VanillaCardPools.Add(VanillaCardPoolIDs.VengefulShardOnlyPool, CustomCharacterManager.GetCharacterDataByID(VanillaCharacterIDs.SeraphtheDiligent)?.GetBossActionData()[0]?.GetActions()[0]?.GetActionEffectData()[0]?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.VineGraspOnlyPool, allGameData.FindCollectableRelicData(VanillaCollectableRelicIDs.CursedVines)?.GetFirstRelicEffectData<RelicEffectAddBattleCardToHand>()?.GetParamCardPool());
            VanillaCardPools.Add(VanillaCardPoolIDs.TrainSteward2, allGameData.FindMutatorData(VanillaMutatorIDs.AtYourService)?.GetFirstRelicEffectData<RelicEffectAddCardsStartOfRun>()?.GetParamCardPool());
        }
    }
}
