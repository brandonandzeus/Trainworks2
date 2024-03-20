using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class HealthRewardDataBuilder : GrantableRewardDataBuilderBase, IRewardDataBuilder
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
                    NameKey = RewardID + "_HealthRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardID + "_HealthRewardData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Amount of health to give. -1 for a full heal.
        /// </summary>
        public int Amount { get; set; }

        public RewardData Build(bool register = true)
        {
            var rewardData = ScriptableObject.CreateInstance<HealthRewardData>();
            rewardData.name = RewardID;

            Construct(rewardData);

            AccessTools.Field(typeof(HealthRewardData), "_amount").SetValue(rewardData, Amount);

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
