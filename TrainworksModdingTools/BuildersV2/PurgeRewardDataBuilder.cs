using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class PurgeRewardDataBuilder : GrantableRewardDataBuilderBase, IRewardDataBuilder
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
                    NameKey = RewardID + "_PurgeRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardID + "_PurgeRewardData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Number of Card Purges.
        /// </summary>
        public int NumPurges { get; set; }

        /// <summary>
        /// Are the purges mandatory.
        /// </summary>
        public bool IsCompulsory { get; set; }

        /// <summary>
        /// Filter for cards
        /// </summary>
        public CardUpgradeMaskData CardUpgradeMaskData { get; set; }

        /// <summary>
        /// Convienence Builder for CardUpgradeMaskData if set overrides the former.
        /// </summary>
        public CardUpgradeMaskDataBuilder CardUpgradeMaskBuilder { get; set; }

        public RewardData Build(bool register = true)
        {
            var rewardData = ScriptableObject.CreateInstance<PurgeRewardData>();
            rewardData.name = RewardID;

            Construct(rewardData);

            var cardUpgradeMaskData = CardUpgradeMaskData;
            if (CardUpgradeMaskBuilder  != null)
            {
                cardUpgradeMaskData = CardUpgradeMaskBuilder.Build();
            }

            AccessTools.Field(typeof(PurgeRewardData), "isCompulsory").SetValue(rewardData, IsCompulsory);
            AccessTools.Field(typeof(PurgeRewardData), "numPurges").SetValue(rewardData, NumPurges);
            AccessTools.Field(typeof(PurgeRewardData), "cardUpgradeMaskData").SetValue(rewardData, cardUpgradeMaskData);

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
