using BepInEx.Logging;
using System;
using System.Collections.Generic;

namespace Trainworks.Enums.MTTriggers
{
    /// <summary>
    /// An Extended Enum wrapping CardTriggerType.
    /// </summary>
    public class CardTrigger : ExtendedEnum<CardTrigger, CardTriggerType>
    {
        public CardTrigger(string localizationKey, int? ID = null) : base(localizationKey)
        {
            if (ID.HasValue)
            {
                Trainworks.Log(LogLevel.Warning, "CardTrigger: Specific ID requested, that will be ignored");
            }
            Dictionary<CardTriggerType, string> dict = (Dictionary<CardTriggerType, string>)typeof(CardTriggerTypeMethods).GetField("TriggerToLocalizationExpression", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
            dict[this.GetEnum()] = localizationKey;
        }

        [Obsolete("This function is unneccessary and has a bug")]
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
        public CharacterTrigger(string localizationKey, int? ID = null) : base(localizationKey)
        {
            if (ID.HasValue)
            {
                Trainworks.Log(LogLevel.Warning, "CharacterTrigger: Specific ID requested, that will be ignored");
            }
            CharacterTriggerData.TriggerToLocalizationExpression[this.GetEnum()] = localizationKey;
        }

        [Obsolete("This function is unneccessary and has a bug")]
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

