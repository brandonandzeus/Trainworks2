using BepInEx.Logging;
using System;
using System.Collections.Generic;

namespace Trainworks.Utilities
{
    public static class ContentValidator
    {
        private static List<String> SCALING_UNIT_TRAITS = new List<String>()
        {
            "CardTraitScalingUpgradeUnitAttack",
            "CardTraitScalingUpgradeUnitHealth",
            "CardTraitScalingUpgradeUnitSize",
            "CardTraitScalingUpgradeUnitStatusEffect",
        };

        public static void Validate(CardData card)
        {
            // The Starter rarity is unused. Not even the starter cards in game have the rarity set to Starter.
            // This may cause issues if the card is present in a CardPool, since CardPools are filtered when pulling from them
            // A card marked with this rarity will never be chosen. If it is the only card in the pool the game will crash.
            if (card.GetRarity() == CollectableRarity.Starter)
            {
                Trainworks.Log(LogLevel.Warning, "CardData " + card.name + " has rarity set to Starter. " +
                    "The Starter rarity is considered an invalid value for CardData and may cause problems if present in CardPools. Consider removing the rarity from this card.");
            }
            if (card.IsSpawnerCard())
            {
                Validate(card, card.GetSpawnCharacterData());
            }
        }

        public static void Validate(CharacterData character)
        {
        }

        public static void Validate(CardData card, CharacterData character)
        {
            foreach (var trait in card.GetTraits())
            {
                if (SCALING_UNIT_TRAITS.Contains(trait.GetTraitStateName()))
                {
                    Trainworks.Log(LogLevel.Warning, "CardData " + card.name + " spawns a character " + character.name + " and has a CardTrait with a scaling upgrade effect." +
                        " Consider using " + trait.GetTraitStateName() + "Safely as any upgrades the character applies to itself and others will be scaled which is probably not what you intended.");
                }
            }
        }

        public static void Validate(CardUpgradeData upgrade)
        {
            if (upgrade.IsUnitSynthesisUpgrade() && upgrade.GetSourceSynthesisUnit() == null)
            {
                Trainworks.Log(LogLevel.Warning, "CardUpgrade " + upgrade.name + " has isUnitSynthesis set, but no SourceSynthesisUnit set.");
            }
        }

        public static void Validate(CollectableRelicData relic)
        {

        }

    }
}
