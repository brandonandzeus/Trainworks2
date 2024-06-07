using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.Utilities
{
    public static class ContentValidator
    {
        public static void Validate(CardData cardData)
        {
            // The Starter rarity is unused. Not even the starter cards in game have the rarity set to Starter.
            // This may cause issues if the card is present in a CardPool, since CardPools are filtered when pulling from them
            // A card marked with this rarity will never be chosen. If it is the only card in the pool the game will crash.
            if (cardData.GetRarity() == CollectableRarity.Starter)
            {
                Trainworks.Log(LogLevel.Warning, "CardData " + cardData.name + " has rarity set to Starter. " +
                    "The Starter rarity is considered an invalid value for CardData and may cause problems if present in CardPools. Consider removing the rarity from this card.");
            }
        }

    }
}
