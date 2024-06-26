﻿using HarmonyLib;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class CrystalRewardDataBuilder : GrantableRewardDataBuilderBase, IRewardDataBuilder
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
                    NameKey = RewardID + "_CrystalRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardID + "_CrystalRewardData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Amount of pact shards to give.
        /// </summary>
        public int Amount { get; set; }

        public RewardData Build(bool register = true)
        {
            var rewardData = ScriptableObject.CreateInstance<CrystalRewardData>();
            rewardData.name = RewardID;

            Construct(rewardData);

            AccessTools.Field(typeof(CrystalRewardData), "_amount").SetValue(rewardData, Amount);

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
