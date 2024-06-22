using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Trainworks.BuildersV2
{
    /// <summary>
    /// Controls a single boss action and the effects when the action is performed.
    /// </summary>
    public class BossActionDataBuilder
    {
        /// <summary>
        /// Class implementing IBossActionBehavior to instantiate for this boss action.
        /// </summary>
        public Type ActionBehaviorType { get; set; }
        public string ActionBehaviorName => BuilderUtils.GetEffectClassName(ActionBehaviorType);
        /// <summary>
        /// 
        /// </summary>
        public string TooltipOverrideKey { get; set; }
        /// <summary>
        /// List of CardEffects to play when the action is performed.
        /// </summary>
        public List<CardEffectData> ActionEffects { get; set; }
        /// <summary>
        /// Convienence Builder for ActionEffects will be appended when built.
        /// </summary>
        public List<CardEffectDataBuilder> ActionEffectBuilders { get; set; }
        /// <summary>
        /// Team type to target.
        /// </summary>
        public Team.Type ParamTeamType { get; set; }

        public BossActionDataBuilder()
        {
            TooltipOverrideKey = string.Empty;
            ActionEffects = new List<CardEffectData>();
            ActionEffectBuilders = new List<CardEffectDataBuilder>();
            ParamTeamType = Team.Type.None;
        }

        public BossActionData Build()
        {
            if (ActionBehaviorType == null)
            {
                throw new BuilderException("ActionBehaviorType is required");
            }

            List<CardEffectData> actionEffects = new List<CardEffectData>();
            actionEffects.AddRange(ActionEffects);
            foreach (var effect in ActionEffectBuilders)
                actionEffects.Add(effect.Build());

            var data = new BossActionData();
            AccessTools.Field(typeof(BossActionData), "_actionBehaviorName").SetValue(data, ActionBehaviorName);
            AccessTools.Field(typeof(BossActionData), "_tooltipOverrideKey").SetValue(data, TooltipOverrideKey);
            AccessTools.Field(typeof(BossActionData), "_actionEffects").SetValue(data, actionEffects);
            AccessTools.Field(typeof(BossActionData), "_paramTeamType").SetValue(data, ParamTeamType);

            return data;
        }
    }
}
