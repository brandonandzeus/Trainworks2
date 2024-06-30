namespace Trainworks.Enums
{
    /// <summary>
    /// An Extended Enum.
    /// </summary>
    public class TrackedValueType : ExtendedEnum<TrackedValueType, CardStatistics.TrackedValueType>
    {
        private static int NumTrackedValues = 576;

        public TrackedValueType(string Name, int? ID = null) : base(Name, ID ?? GetNewCharacterGUID())
        {
        }

        public static implicit operator CardStatistics.TrackedValueType(TrackedValueType extendedEnum)
        {
            return extendedEnum.GetEnum();
        }

        public static int GetNewCharacterGUID()
        {
            NumTrackedValues++;
            return NumTrackedValues;
        }
    }
}
