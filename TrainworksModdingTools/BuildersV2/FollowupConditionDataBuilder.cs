using HarmonyLib;
using System;

namespace Trainworks.BuildersV2
{
    /// <summary>
    /// Builder for FollowupConditionData. Specifies a Condition for a Story Event to occur.
    /// </summary>
    public class FollowupConditionDataBuilder
    {
        /// <summary>
        /// Type inheriting from FollowupConditionBase that performs the evaluation of the condition
        /// given the other params in this class.
        /// </summary>
        public Type ConditionType { get; set; }
        /// <summary>
        /// Possibly? fully qualified type name for the condition.
        /// </summary>
        public string ConditionName => BuilderUtils.GetEffectClassName(ConditionType);
        public int ParamInt { get; set; }
        public int ParamAdditionalInt { get; set; }
        public string ParamString { get; set; }
        public RelicData ParamRelicData { get; set; }
        public CardData ParamCardData { get; set; }
        /// <summary>
        /// Do not use, left in for futureproofing.
        /// The behaviour of this field currently is just to set ParamCardData.
        /// No FollowupConditionBase subclass has access this field instead,
        /// upon loading ParamCardData is filled with CardData corresponding to the id.
        /// </summary>
        public string ParamCardDataId { get; set; }

        public FollowupConditionDataBuilder()
        {
        }

        /// <summary>
        /// Builds the FollowupConditionData
        /// </summary>
        /// <returns></returns>
        public FollowupConditionData Build()
        {
            var followupCond = new FollowupConditionData();

            AccessTools.Field(typeof(FollowupConditionData), "conditionName").SetValue(followupCond, ConditionName);
            AccessTools.Field(typeof(FollowupConditionData), "paramInt").SetValue(followupCond, ParamInt);
            AccessTools.Field(typeof(FollowupConditionData), "paramAdditionalInt").SetValue(followupCond, ParamAdditionalInt);
            AccessTools.Field(typeof(FollowupConditionData), "paramString").SetValue(followupCond, ParamString);
            AccessTools.Field(typeof(FollowupConditionData), "paramRelicData").SetValue(followupCond, ParamRelicData);
            AccessTools.Field(typeof(FollowupConditionData), "paramCardData").SetValue(followupCond, ParamCardData);
            AccessTools.Field(typeof(FollowupConditionData), "paramCardDataId").SetValue(followupCond, ParamCardDataId);

            return followupCond;
        }
    }
}