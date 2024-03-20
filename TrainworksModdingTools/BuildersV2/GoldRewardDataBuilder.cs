using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class GoldRewardDataBuilder : GrantableRewardDataBuilderBase, IRewardDataBuilder
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
                    NameKey = RewardID + "_GoldRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardID + "_GoldRewardData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Amount of gold to give.
        /// </summary>
        public int Amount { get; set; }

        public RewardData Build(bool register = true)
        {
            var rewardData = ScriptableObject.CreateInstance<GoldRewardData>();
            rewardData.name = RewardID;

            Construct(rewardData);

            AccessTools.Field(typeof(GoldRewardData), "_amount").SetValue(rewardData, Amount);

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
