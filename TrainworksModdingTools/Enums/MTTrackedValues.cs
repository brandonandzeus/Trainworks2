using BepInEx.Logging;

namespace Trainworks.Enums
{
    /// <summary>
    /// An Extended Enum.
    /// </summary>
    public class TrackedValueType : ExtendedEnum<TrackedValueType, CardStatistics.TrackedValueType>
    {
        public TrackedValueType(string Name, int? ID = null) : base(Name)
        {
            if (ID.HasValue)
            {
                Trainworks.Log(LogLevel.Warning, "TrackedValueType: Specific ID requested, that will be ignored");
            }            
        }

        public static implicit operator CardStatistics.TrackedValueType(TrackedValueType extendedEnum)
        {
            return extendedEnum.GetEnum();
        }
    }
}
