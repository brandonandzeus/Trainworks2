using HarmonyLib;
using System.Collections.Generic;

namespace Trainworks.BuildersV2
{
    /// <summary>
    /// Builder a FollowupEventData.
    /// Followup events are events that occur after an event.
    /// These can have conditions on when they appear later on in a run.
    /// </summary>
    public class FollowupEventDataBuilder
    {
        /// <summary>
        /// StoryEvent to show as a followup.
        /// </summary>
        public StoryEventData FollowupEvent { get; set; }
        /// <summary>
        /// Can show after victory (i.e. The Giv'em hell event followup at the end of the game).
        /// </summary>
        public bool CanShowAfterVictory { get; set; }
        /// <summary>
        /// Conditions which when true allows the followup event to happen.
        /// </summary>
        public List<FollowupConditionDataBuilder> Conditions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FollowupEventDataBuilder()
        {
            Conditions = new List<FollowupConditionDataBuilder>();
        }

        /// <summary>
        /// Builds the FollowupEventData
        /// </summary>
        /// <returns></returns>
        public FollowupEventData Build()
        {
            FollowupEventData followup = new FollowupEventData();

            List<FollowupConditionData> conditions = new List<FollowupConditionData>();
            foreach (var condition in Conditions)
            {
                conditions.Add(condition.Build());
            }

            AccessTools.Field(typeof(FollowupEventData), "followupEvent").SetValue(followup, FollowupEvent);
            AccessTools.Field(typeof(FollowupEventData), "canShowAfterVictory").SetValue(followup, CanShowAfterVictory);
            AccessTools.Field(typeof(FollowupEventData), "conditions").SetValue(followup, conditions);

            return followup;
        }
    }
}