using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Trainworks.BuildersV2
{
    public class CardDataBuilder
    {
        private string cardID;

        /// <summary>
        /// Unique string used to store and retrieve the card data.
        /// Implicitly sets NameKey and OverrideDescriptionKey if null
        /// </summary>
        public string CardID
        {
            get { return cardID; }
            set
            {
                cardID = value;
                if (NameKey == null)
                {
                    NameKey = cardID + "_CardData_NameKey";
                }
                if (OverrideDescriptionKey == null)
                {
                    OverrideDescriptionKey = cardID + "_CardData_OverrideDescriptionKey";
                }
            }
        }

        /// <summary>
        /// The IDs of all card pools the card should be inserted into.
        /// </summary>
        public List<string> CardPoolIDs { get; set; }
        /// <summary>
        /// Ember cost of the card.
        /// </summary>
        public int Cost { get; set; }
        /// <summary>
        /// Determines whether the card has a normal ember cost or an X ember cost.
        /// </summary>
        public CardData.CostType CostType { get; set; }
        /// <summary>
        /// Name displayed on the card.
        /// Note this sets the localization for all languages.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Localization key for the card's name.
        /// You shouldn't need to set this directly as its automatically set by CardID
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Custom description text appended to the end of the card.
        /// Note this is set for all langauges if set.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Localization key for the card's description.
        /// You shouldn't need to set this as its automatically set by CardID.
        /// </summary>
        public string OverrideDescriptionKey { get; set; }
        /// <summary>
        /// ID of the clan the card is a part of. Leave null for clanless.
        /// Base game clan IDs should be retrieved via helper class "VanillaClanIDs".
        /// If using a custom clan ID, the custom clan must be built first.
        /// </summary>
        public string ClanID { get; set; }
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + AssetPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; protected set; }
        /// <summary>
        /// Custom asset path to load from relative to the plugin's path
        /// </summary>
        public string AssetPath { get; set; }
        /// <summary>
        /// Loading Info for loading a card's sprite from an asset bundle.
        /// </summary>
        public BundleAssetLoadingInfo BundleLoadingInfo { get; set; }
        /// <summary>
        /// Use an existing base game card's art by filling this in with the appropriate card's asset reference information.
        /// </summary>
        public AssetReferenceGameObject CardArtPrefabVariantRef { get; set; }
        /// <summary>
        /// Convenience Builder for Effects. Will be appended to this list of new card effects.
        /// </summary>
        public List<CardEffectDataBuilder> EffectBuilders { get; set; }
        /// <summary>
        /// Convenience Builder for Traits. Will be appended to this list of new card traits.
        /// </summary>
        public List<CardTraitDataBuilder> TraitBuilders { get; set; }
        /// <summary>
        /// Convenience Builder for Triggers. Will be appended to this list of new character triggers.
        /// <see cref="EffectTriggers"/>Effect Triggers are not used by any Cards in Monster Train.
        /// </summary>
        public List<CharacterTriggerDataBuilder> EffectTriggerBuilders { get; set; }
        /// <summary>
        /// Convenience Builder for Triggers. Will be appended to this list of new card triggers.
        /// </summary>
        public List<CardTriggerEffectDataBuilder> TriggerBuilders { get; set; }
        /// <summary>
        /// Set CardArtPrefabVariantRef without reflection.
        /// </summary>
        public Builders.AssetRefBuilder CardArtPrefabVariantRefBuilder { get; set; }
        /// <summary>
        /// List of pre-built card effects.
        /// </summary>
        public List<CardEffectData> Effects { get; set; }
        /// <summary>
        /// List of pre-built card traits.
        /// </summary>
        public List<CardTraitData> Traits { get; set; }
        /// <summary>
        /// List of pre-built character triggers.
        /// <warning>This parameter is not used by any cards. The only mention of it is via CardEffectAddTrigger which is an unused CardEffect.</warning> 
        /// </summary>
        public List<CharacterTriggerData> EffectTriggers { get; set; }
        /// <summary>
        /// List of pre-built card triggers.
        /// </summary>
        public List<CardTriggerEffectData> Triggers { get; set; }
        /// <summary>
        /// These upgrades are applied to all new instances of this card by default.
        /// </summary>
        public List<CardUpgradeData> StartingUpgrades { get; set; }
        /// <summary>
        /// StartingUpgrades as a list of CardUpgradeDataBuilder for convienence.
        /// These will be appended to StartingUpgrades.
        /// </summary>
        public List<CardUpgradeDataBuilder> StartingUpgradeBuilders { get; set; }
        /// <summary>
        /// Use an existing base game card's lore tooltip by adding its key to this list.
        /// </summary>
        public List<string> CardLoreTooltipKeys { get; set; }
        /// <summary>
        /// Whether or not the card has a target.
        /// </summary>
        public bool Targetless { get; set; }
        /// <summary>
        /// Whether or not the card targets a room.
        /// </summary>
        public bool TargetsRoom { get; set; }
        /// <summary>
        /// The type of card: Spell, Monster, Blight, Scourge, or Invalid.
        /// </summary>
        public CardType CardType { get; set; }
        /// <summary>
        /// The card's rarity: Common, Uncommon, Rare, Champion, Starter
        /// </summary>
        public CollectableRarity Rarity { get; set; }
        /// <summary>
        /// Level at which the card is unlocked.
        /// </summary>
        public int UnlockLevel { get; set; }
        public List<CardData> SharedMasteryCards { get; set; }
        public CardData LinkedMasteryCard { get; set; }
        /// <summary>
        /// Whether or not this card is displayed in the logbook when counting all of the player's mastered cards.
        /// </summary>
        public bool IgnoreWhenCountingMastery { get; set; }
        /// <summary>
        /// A cache for the card's sprite so it doesn't have to be reloaded repeatedly.
        /// </summary>
        // TODO this doesn't seem to be necessary, the cache appears to be loaded only once.
        public Sprite SpriteCache { get; set; }
        public CardInitialKeyboardTarget InitialKeyboardTarget { get; set; }
        public ShinyShoe.DLC RequiredDLC { get; set; }

        public CardDataBuilder()
        {
            CardPoolIDs = new List<string>();
            EffectBuilders = new List<CardEffectDataBuilder>();
            TraitBuilders = new List<CardTraitDataBuilder>();
            EffectTriggerBuilders = new List<CharacterTriggerDataBuilder>();
            TriggerBuilders = new List<CardTriggerEffectDataBuilder>();
            Effects = new List<CardEffectData>();
            Traits = new List<CardTraitData>();
            EffectTriggers = new List<CharacterTriggerData>();
            Triggers = new List<CardTriggerEffectData>();
            SharedMasteryCards = new List<CardData>();
            StartingUpgrades = new List<CardUpgradeData>();
            StartingUpgradeBuilders = new List<CardUpgradeDataBuilder>();
            CardLoreTooltipKeys = new List<string>();
            RequiredDLC = ShinyShoe.DLC.None;
            TargetsRoom = true;
            InitialKeyboardTarget = CardInitialKeyboardTarget.FrontFriendly;
            // TODO Get the FallbackData instance.

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the CardData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered CardData</returns>
        public CardData BuildAndRegister()
        {
            var cardData = Build();
            CustomCardManager.RegisterCustomCard(cardData, CardPoolIDs);
            return cardData;
        }

        /// <summary>
        /// Builds the CardData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created CardData</returns>
        public CardData Build()
        {
            if (cardID == null)
            {
                throw new BuilderException("CardID is required");
            }

            CardData cardData = ScriptableObject.CreateInstance<CardData>();
            var guid = GUIDGenerator.GenerateDeterministicGUID(CardID);
            AccessTools.Field(typeof(GameData), "id").SetValue(cardData, guid);
            cardData.name = CardID;

            // These are safe to do, the list is already allocated in the class.
            var cardEffects = cardData.GetEffects();
            cardEffects.AddRange(Effects);
            foreach (var builder in EffectBuilders)
            {
                cardEffects.Add(builder.Build());
            }
            var cardEffectTriggers = cardData.GetEffectTriggers();
            cardEffectTriggers.AddRange(EffectTriggers);
            foreach (var builder in EffectTriggerBuilders)
            {
                cardEffectTriggers.Add(builder.Build());
            }
            var cardTraits = cardData.GetTraits();
            cardTraits.AddRange(Traits);
            foreach (var builder in TraitBuilders)
            {
                cardTraits.Add(builder.Build());
            }
            var cardTriggers = cardData.GetCardTriggers();
            cardTriggers.AddRange(Triggers);
            foreach (var builder in TriggerBuilders)
            {
                cardTriggers.Add(builder.Build());
            }
            var startingUpgrades = cardData.GetUpgradeData();
            startingUpgrades.AddRange(StartingUpgrades);
            foreach (var builder in StartingUpgradeBuilders)
            {
                startingUpgrades.Add(builder.Build());
            }

            var linkedClass = ClanID == null ? null : CustomClassManager.GetClassDataByID(ClanID);
            AccessTools.Field(typeof(CardData), "cardLoreTooltipKeys").SetValue(cardData, CardLoreTooltipKeys);
            AccessTools.Field(typeof(CardData), "cardType").SetValue(cardData, CardType);
            AccessTools.Field(typeof(CardData), "cost").SetValue(cardData, Cost);
            AccessTools.Field(typeof(CardData), "costType").SetValue(cardData, CostType);
            AccessTools.Field(typeof(CardData), "ignoreWhenCountingMastery").SetValue(cardData, IgnoreWhenCountingMastery);
            AccessTools.Field(typeof(CardData), "initialKeyboardTarget").SetValue(cardData, InitialKeyboardTarget);
            AccessTools.Field(typeof(CardData), "linkedClass").SetValue(cardData, linkedClass);
            AccessTools.Field(typeof(CardData), "linkedMasteryCard").SetValue(cardData, LinkedMasteryCard);
            AccessTools.Field(typeof(CardData), "nameKey").SetValue(cardData, NameKey);
            AccessTools.Field(typeof(CardData), "overrideDescriptionKey").SetValue(cardData, OverrideDescriptionKey);
            AccessTools.Field(typeof(CardData), "rarity").SetValue(cardData, Rarity);
            AccessTools.Field(typeof(CardData), "requiredDLC").SetValue(cardData, RequiredDLC);
            AccessTools.Field(typeof(CardData), "sharedMasteryCards").SetValue(cardData, SharedMasteryCards);
            AccessTools.Field(typeof(CardData), "targetless").SetValue(cardData, Targetless);
            AccessTools.Field(typeof(CardData), "targetsRoom").SetValue(cardData, TargetsRoom);
            AccessTools.Field(typeof(CardData), "unlockLevel").SetValue(cardData, UnlockLevel);

            if (CardArtPrefabVariantRef == null)
            {
                if (CardArtPrefabVariantRefBuilder == null)
                {
                    if (BundleLoadingInfo != null)
                    {
                        BundleLoadingInfo.PluginPath = BaseAssetPath;
                        CardArtPrefabVariantRefBuilder = new Builders.AssetRefBuilder
                        {
                            AssetLoadingInfo = BundleLoadingInfo
                        };
                    }
                    else
                    {
                        var assetLoadingInfo = new AssetLoadingInfo()
                        {
                            FilePath = AssetPath,
                            PluginPath = BaseAssetPath,
                            AssetType = Builders.AssetRefBuilder.AssetTypeEnum.CardArt
                        };
                        CardArtPrefabVariantRefBuilder = new Builders.AssetRefBuilder
                        {
                            AssetLoadingInfo = assetLoadingInfo
                        };
                    }
                }
                CardArtPrefabVariantRef = CardArtPrefabVariantRefBuilder.BuildAndRegister();
            }
            AccessTools.Field(typeof(CardData), "cardArtPrefabVariantRef").SetValue(cardData, CardArtPrefabVariantRef);
            if (SpriteCache != null)
            {
                AccessTools.Field(typeof(CardData), "spriteCache").SetValue(cardData, SpriteCache);
            }

            BuilderUtils.ImportStandardLocalization(NameKey, Name);
            BuilderUtils.ImportStandardLocalization(OverrideDescriptionKey, Description);

            return cardData;
        }

        /// <summary>
        /// Creates an asset reference to an existing game file.
        /// Primarily useful for reusing base game art.
        /// Cards with custom art should not use this method.
        /// </summary>
        /// <param name="m_debugName">The asset's debug name (usually the path to it)</param>
        /// <param name="m_AssetGUID">The asset's GUID</param>
        public void CreateAndSetCardArtPrefabVariantRef(string m_debugName, string m_AssetGUID)
        {
            var assetReferenceGameObject = new AssetReferenceGameObject();
            AccessTools.Field(typeof(AssetReferenceGameObject), "m_debugName")
                    .SetValue(assetReferenceGameObject, m_debugName);
            AccessTools.Field(typeof(AssetReferenceGameObject), "m_AssetGUID")
                .SetValue(assetReferenceGameObject, m_AssetGUID);
            CardArtPrefabVariantRef = assetReferenceGameObject;

            AssetPath = m_AssetGUID;
        }
    }
}