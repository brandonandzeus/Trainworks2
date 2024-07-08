
using BepInEx.Logging;

namespace Trainworks.Enums
{
    public class StatusEffectTriggerStage : ExtendedByteEnum<StatusEffectTriggerStage, StatusEffectData.TriggerStage>
    {
        public StatusEffectTriggerStage(string Name, byte? ID = null) : base(Name)
        {
            if (ID.HasValue)
            {
                Trainworks.Log(LogLevel.Warning, "StatusEffectTriggerStage: Specific ID requested, that will be ignored");
            }
        }

        public static implicit operator StatusEffectData.TriggerStage(StatusEffectTriggerStage extendedEnum)
        {
            return extendedEnum.GetEnum();
        }
    }

}
