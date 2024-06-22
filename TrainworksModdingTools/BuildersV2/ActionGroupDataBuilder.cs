using HarmonyLib;
using System.Collections.Generic;

namespace Trainworks.BuildersV2
{
    /// <summary>
    /// This class controls the actions for flying bosses.
    /// </summary>
    public class ActionGroupDataBuilder
    {
        /// <summary>
        /// ???
        /// </summary>
        public int NumRepeats { get; set; }
        /// <summary>
        /// Boss Actions.
        /// </summary>
        public List<BossActionData> Actions { get; set; }
        /// <summary>
        /// Convienence Builder for Actions. Will be appeneded to Actions.
        /// </summary>
        public List<BossActionDataBuilder> ActionBuilders { get; set; }

        public ActionGroupDataBuilder()
        {
            NumRepeats = 1;
            Actions = new List<BossActionData>();
            ActionBuilders = new List<BossActionDataBuilder>();
        }

        public ActionGroupData Build()
        {
            var actionGroupData = new ActionGroupData();

            var actions = new List<BossActionData>();
            actions.AddRange(Actions);
            foreach (var action in ActionBuilders)
                actions.Add(action.Build());

            AccessTools.Field(typeof(ActionGroupData), "_numRepeats").SetValue(actionGroupData, NumRepeats);
            AccessTools.Field(typeof(ActionGroupData), "_actions").SetValue(actionGroupData, actions);

            return actionGroupData;
        }
    }
}
