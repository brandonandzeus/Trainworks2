using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class RelicRewardDataBuilder : GrantableRewardDataBuilderBase, IRewardDataBuilder
    {
        private string rewardID;

        /// <summary>
        /// Unique string used to store and retrieve the draft reward data.
        /// Implicitly sets NameKey and DescriptionKey if null
        /// </summary>
        public string RewardID
        {
            get { return rewardID; }
            set
            {
                rewardID = value;
                if (NameKey == null)
                {
                    NameKey = RewardID + "_RelicRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardID + "_RelicRewardData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Specific relic to give.
        /// </summary>
        public CollectableRelicData RelicData { get; set; }

        public RewardData Build(bool register = true)
        {
            if (RewardID == null)
            {
                throw new BuilderException("RewardID is required");
            }

            var rewardData = ScriptableObject.CreateInstance<RelicRewardData>();
            rewardData.name = RewardID;

            Construct(rewardData);

            AccessTools.Field(typeof(RelicRewardData), "_relicData").SetValue(rewardData, RelicData);

            if (register)
            {
                CustomRewardManager.RegisterCustomReward(rewardData);
            }

            return rewardData;
        }

        public RewardData BuildAndRegister()
        {
            RewardData data = Build(false);
            CustomRewardManager.RegisterCustomReward(data);
            return data;
        }
    }
}
