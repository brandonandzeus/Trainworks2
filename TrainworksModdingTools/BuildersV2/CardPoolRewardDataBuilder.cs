using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class CardPoolRewardDataBuilder : RewardDataBuilderBase, IRewardDataBuilder
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
                    NameKey = RewardID + "_CardPoolRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardID + "_CardPoolRewardData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Card Pool to use.
        /// </summary>
        public CardPool CardPool { get; set; }

        public RewardData Build(bool register = true)
        {
            if (RewardID == null)
            {
                throw new BuilderException("RewardID is required");
            }

            var rewardData = ScriptableObject.CreateInstance<CardPoolRewardData>();
            rewardData.name = RewardID;

            Construct(rewardData);

            AccessTools.Field(typeof(CardPoolRewardData), "_cardPool").SetValue(rewardData, CardPool);

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
