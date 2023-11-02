using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Builders;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class TrialDataBuilder
    {
        private String trialID;

        public String TrialID
        {
            get
            { 
                return trialID; 
            }
            set
            {
                trialID = value;
            }
        }

        /// <summary>
        /// Beneficial effect given to enemies for this trial.
        /// </summary>
        public SinsData Sin { get; set; }
        /// <summary>
        /// Convenience builder for Sin. If set the value of Sin is ignored.
        /// </summary>
        public SinsDataBuilder SinBuilder { get; set; }
        /// <summary>
        /// Reward for completing the trial.
        /// </summary>
        public RewardData Reward { get; set; }
        /// <summary>
        /// Convenience builder for Reward. If set the value of Reward is ignored.
        /// </summary>
        public IRewardDataBuilder RewardBuilder { get; set; }

        public TrialData Build()
        {
            if (TrialID == null)
            {
                throw new BuilderException("TrialID is required");
            }

            var trial = ScriptableObject.CreateInstance<TrialData>();

            var guid = GUIDGenerator.GenerateDeterministicGUID(TrialID);
            AccessTools.Field(typeof(GameData), "id").SetValue(trial, guid);
            trial.name = TrialID;

            var reward = Reward;
            if (RewardBuilder != null)
            {
                reward = RewardBuilder.Build();
            }
            var sin = Sin;
            if (SinBuilder != null)
            {
                sin = SinBuilder.Build();
            }

            AccessTools.Field(typeof(TrialData), "sin").SetValue(trial, sin);
            AccessTools.Field(typeof(TrialData), "reward").SetValue(trial, reward);

            return trial;
        }

        public TrialData BuildAndRegister()
        {
            TrialData trial = Build();
            CustomScenarioManager.RegisterCustomTrial(trial);
            return trial;
        }
    }
}
