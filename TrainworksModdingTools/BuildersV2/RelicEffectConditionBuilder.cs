using HarmonyLib;
using Trainworks.ConstantsV2;

// Changes: Capitialized properties to keep consistency across the framework.
namespace Trainworks.BuildersV2
{
    public class RelicEffectConditionBuilder
    {
        public CardStatistics.TrackedValueType ParamTrackedValue { get; set; }
        public CardStatistics.CardTypeTarget ParamCardType { get; set; }
        /// <summary>
        /// Determines if we should use the total trigger count as the tracked value instead of using a value from CardStatistics. Use this to limit triggers to X per turn or per battle.
        /// </summary>
        public bool ParamTrackTriggerCount { get; set; }
        public CardStatistics.EntryDuration ParamEntryDuration { get; set; }
        public RelicEffectCondition.Comparator ParamComparator { get; set; }
        public int ParamInt { get; set; }
        /// <summary>
        /// Allow multiple triggers per duration that is defined as Entry Duration. This can be used to limit a relic from trigger more than X times per turn or per battle 
        /// </summary>
        public bool AllowMultipleTriggersPerDuration { get; set; }
        public string ParamSubtype { get; set; }

        public RelicEffectConditionBuilder()
        {
            ParamSubtype = VanillaSubtypeIDs.None;
            ParamComparator = RelicEffectCondition.Comparator.Equal | RelicEffectCondition.Comparator.GreaterThan;
            AllowMultipleTriggersPerDuration = true;
        }

        /// <summary>
        /// Builds the RelicEffectCondition represented by this builder's parameters
        /// </summary>
        /// <returns>The newly created RelicEffectCondition</returns>
        public RelicEffectCondition Build()
        {
            // Doesn't inherit from Scriptable Object.
            RelicEffectCondition relicEffectCondition = new RelicEffectCondition();
            AccessTools.Field(typeof(RelicEffectCondition), "paramTrackedValue").SetValue(relicEffectCondition, ParamTrackedValue);
            AccessTools.Field(typeof(RelicEffectCondition), "paramCardType").SetValue(relicEffectCondition, ParamCardType);
            AccessTools.Field(typeof(RelicEffectCondition), "paramTrackTriggerCount").SetValue(relicEffectCondition, ParamTrackTriggerCount);
            AccessTools.Field(typeof(RelicEffectCondition), "paramEntryDuration").SetValue(relicEffectCondition, ParamEntryDuration);
            AccessTools.Field(typeof(RelicEffectCondition), "paramComparator").SetValue(relicEffectCondition, ParamComparator);
            AccessTools.Field(typeof(RelicEffectCondition), "paramInt").SetValue(relicEffectCondition, ParamInt);
            AccessTools.Field(typeof(RelicEffectCondition), "allowMultipleTriggersPerDuration").SetValue(relicEffectCondition, AllowMultipleTriggersPerDuration);
            AccessTools.Field(typeof(RelicEffectCondition), "paramSubtype").SetValue(relicEffectCondition, ParamSubtype);
            return relicEffectCondition;
        }
    }
}