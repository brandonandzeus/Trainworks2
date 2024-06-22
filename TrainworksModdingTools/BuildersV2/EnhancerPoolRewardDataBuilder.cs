using HarmonyLib;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class EnhancerPoolRewardDataBuilder : RewardDataBuilderBase, IRewardDataBuilder
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
                    NameKey = RewardID + "_EnhancerPoolRewardData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = RewardID + "_EnhancerPoolRewardData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Card Pool to use.
        /// </summary>
        public EnhancerPool EnhancerPool { get; set; }

        public RewardData Build(bool register = true)
        {
            if (RewardID == null)
            {
                throw new BuilderException("RewardID is required");
            }

            var rewardData = ScriptableObject.CreateInstance<EnhancerPoolRewardData>();
            rewardData.name = RewardID;

            Construct(rewardData);

            AccessTools.Field(typeof(EnhancerPoolRewardData), "_enhancerPool").SetValue(rewardData, EnhancerPool);

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
