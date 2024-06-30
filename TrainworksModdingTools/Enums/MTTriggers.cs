using System.Collections.Generic;

namespace Trainworks.Enums.MTTriggers
{
    /// <summary>
    /// An Extended Enum wrapping CardTriggerType.
    /// </summary>
    public class CardTrigger : ExtendedEnum<CardTrigger, CardTriggerType>
    {
        private static int NumCardTriggers = 576;

        public CardTrigger(string localizationKey, int? ID = null) : base(localizationKey, ID ?? GetNewCardGUID())
        {
            Dictionary<CardTriggerType, string> dict = (Dictionary<CardTriggerType, string>)typeof(CardTriggerTypeMethods).GetField("TriggerToLocalizationExpression", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
            dict[this.GetEnum()] = localizationKey;
        }

        public static int GetNewCardGUID()
        {
            NumCardTriggers++;
            return NumCardTriggers;
        }

        public static implicit operator CardTrigger(CardTriggerType cardTriggerType)
        {
            return CardTrigger.Convert(cardTriggerType);
        }

        public static implicit operator CardTriggerType(CardTrigger extendedEnum)
        {
            return extendedEnum.GetEnum();
        }
    }

    /// <summary>
    /// An Extended Enum wrapping CharacterTriggerData.Trigger.
    /// </summary>
    public class CharacterTrigger : ExtendedEnum<CharacterTrigger, CharacterTriggerData.Trigger>
    {
        private static int NumCharTriggers = 576;

        public CharacterTrigger(string localizationKey, int? ID = null) : base(localizationKey, ID ?? GetNewCharacterGUID())
        {
            CharacterTriggerData.TriggerToLocalizationExpression[this.GetEnum()] = localizationKey;
        }

        public static int GetNewCharacterGUID()
        {
            NumCharTriggers++;
            return NumCharTriggers;
        }

        public static implicit operator CharacterTrigger(CharacterTriggerData.Trigger trigger)
        {
            return CharacterTrigger.Convert(trigger);
        }

        public static implicit operator CharacterTriggerData.Trigger(CharacterTrigger extendedEnum)
        {
            return extendedEnum.GetEnum();
        }
    }
}

