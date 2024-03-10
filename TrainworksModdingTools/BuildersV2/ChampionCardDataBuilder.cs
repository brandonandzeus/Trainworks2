using System.ComponentModel;
using System.Reflection;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class ChampionCardDataBuilder : CardDataBuilder
    {
        /// <summary>
        /// Champion CharacterData.
        /// </summary>
        public CharacterDataBuilder Champion { get; set; }
        /// <summary>
        /// Starter Card to pair with the Champion.
        /// </summary>
        public string StarterCardID { get; set; }
        /// <summary>
        /// The UpgradeTree for the Dark Forge.
        /// </summary>
        public CardUpgradeTreeDataBuilder UpgradeTree { get; set; }
        /// <summary>
        /// Icon Path for the Logbook.
        /// </summary>
        public string ChampionIconPath { get; set; }
        /// <summary>
        /// Sound Effect when the champion is selected.
        /// </summary>
        public string ChampionSelectedCue { get; set; }

        public ChampionCardDataBuilder() : base()
        {
            Rarity = CollectableRarity.Champion;

            EffectBuilders.Add(new CardEffectDataBuilder
            {
                EffectStateType = typeof(CardEffectSpawnMonster),
                TargetMode = TargetMode.DropTargetCharacter,
            });

            CardType = CardType.Monster;
            TargetsRoom = true;

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the CardData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered CardData</returns>
        public CardData BuildAndRegister(int ChampionIndex = 0)
        {
            var cardData = Build();
            CustomCardManager.RegisterCustomCard(cardData, CardPoolIDs);

            var Clan = cardData.GetLinkedClass();

            ChampionData ClanChamp = Clan.GetChampionData(ChampionIndex);
            ClanChamp.championCardData = cardData;
            if (ChampionIconPath != null)
            {
                Sprite championIconSprite = CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + ChampionIconPath);
                ClanChamp.championIcon = championIconSprite;
            }
            var starterCard = CustomCardManager.GetCardDataByID(StarterCardID);
            if (starterCard == null)
            {
                throw new BuilderException("Could not find starter card for Champion");
            }
            ClanChamp.starterCardData = starterCard;
            if (UpgradeTree == null)
            {
                throw new BuilderException("UpgradeTree Missing for Champion");
            }
            ClanChamp.upgradeTree = UpgradeTree.Build();
            ClanChamp.championSelectedCue = ChampionSelectedCue;

            return cardData;
        }

        public new CardData BuildAndRegister()
        {
            return BuildAndRegister(0);
        }

        /// <summary>
        /// Builds the CardData represented by this builder's parameters
        /// i.e. all CardEffectBuilders in EffectBuilders will also be built.
        /// </summary>
        /// <returns>The newly created CardData</returns>
        public new CardData Build()
        {
            Champion.SubtypeKeys.Add(VanillaSubtypeIDs.Champion);
            EffectBuilders[0].ParamCharacterDataBuilder = Champion;

            CardData cardData = base.Build();

            return cardData;
        }
    }
}
