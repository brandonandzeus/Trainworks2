using BepInEx.Logging;
using System;
using System.Collections.Generic;
using Trainworks.Builders;

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



        public static void PreBuild(BuildersV2.CardTraitDataBuilder builder)
        {
            // Apply the patch for UBP mod Attuned must have ParamInt set to 5.
            if (builder.TraitStateName == "CardTraitStrongerMagicPower" && builder.ParamInt == 0)
            {
                Trainworks.Log(LogLevel.Warning, "Attuned trait added to card with ParamInt = 0, Setting it to 5 for compatibility with the UBP mod");
                builder.ParamInt = 5;
            }
        }


        // ===================================
        // = Old builder fixes
        // ===================================
        public static void PreBuild(Builders.CardTraitDataBuilder oldBuilder)
        {
            // TraitStateName's that include Assembly Info when it shouldn't.
            // Breaks Cards like Spellogenesis.
            oldBuilder.TraitStateName = TryFixClassName(oldBuilder.TraitStateName);

            // Apply the patch for UBP mod Attuned must have ParamInt set to 5.
            if (oldBuilder.TraitStateName == "CardTraitStrongerMagicPower" && oldBuilder.ParamInt == 0)
            {
                Trainworks.Log(LogLevel.Warning, "Attuned trait added to card with ParamInt = 0, Setting it to 5 for compatibility with the UBP mod");
                oldBuilder.ParamInt = 5;
            }
        }

        public static void PreBuild(Builders.CardEffectDataBuilder oldBuilder)
        {
            // EffectStateName's that include Assembly Info when it shouldn't.
            // Futureproofing in case anyone wants to filter out Base Game Effects
            oldBuilder.EffectStateName = TryFixClassName(oldBuilder.EffectStateName);
        }

        public static void PreBuild(Builders.RoomModifierDataBuilder oldBuilder)
        {
            // ClassName's that include Assembly Info when it shouldn't.
            // Futureproofing in case anyone wants to filter out Base Game Effects
            oldBuilder.RoomStateModifierClassName = TryFixClassName(oldBuilder.RoomStateModifierClassName);
        }

        public static void PreBuild(CardTriggerData oldBuilder)
        {
            // ClassName's that include Assembly Info when it shouldn't.
            // Futureproofing in case anyone wants to filter out Base Game Effects
            oldBuilder.cardTriggerEffect = TryFixClassName(oldBuilder.cardTriggerEffect);
            oldBuilder.buffEffectType = TryFixClassName(oldBuilder.buffEffectType);
        }

        public static void PreBuild(Builders.RelicEffectDataBuilder oldBuilder)
        {
            // Ehh what the hell might as well fix these too.
            // Nothing currently filters relic effects, and by the time the game is running they are already converted to Types.
            oldBuilder.RelicEffectClassName = TryFixClassName(oldBuilder.RelicEffectClassName);
        }

        public static string TryFixClassName(string className)
        {
            // If the class name doesn't indicate its from the base game do nothing.

            // A malicious case where someone names their class the same as one in the Base Game and include the AssemblyQualifiedName
            // This code would return the base game's class name thus changing behavior.
            if (!className.Contains("Assembly-CSharp"))
                return className;

            Type type = Type.GetType(className);
            // If type ends up being null then it's a definitely a Base Game Type so don't need to do anything.
            if (type != null)
            {
                string EffectStateName2 = BuildersV2.BuilderUtils.GetEffectClassName(type);
                // If there's a change (if the above function doesn't return a Fully Qualified Name) return the name only.
                if (className != EffectStateName2)
                {
                    Trainworks.Log(LogLevel.Debug, "Changed " + className + " to " +  EffectStateName2 + " the fully QualifiedAssemblyName is only required for CustomEFfects");
                    return EffectStateName2;
                }
            }
            return className;
        }
    }
}
