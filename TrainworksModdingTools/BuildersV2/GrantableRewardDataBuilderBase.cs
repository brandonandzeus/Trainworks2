using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.BuildersV2
{
    public abstract class GrantableRewardDataBuilderBase : RewardDataBuilderBase
    {
        public bool CanBeSkippedOverride { get; set; }
        public bool ForceContentUnlocked { get; set; }
        public bool IsServiceMerchantReward { get; set; }
        public int MerchantServiceIndex { get; set; }

        public void Construct(GrantableRewardData rewardData)
        {
            base.Construct(rewardData);

            rewardData.CanBeSkippedOverride = CanBeSkippedOverride;
            rewardData.ForceContentUnlocked = ForceContentUnlocked;

            AccessTools.Field(typeof(GrantableRewardData), "_isServiceMerchantReward").SetValue(rewardData, IsServiceMerchantReward);
            AccessTools.Field(typeof(GrantableRewardData), "_merchantServiceIndex").SetValue(rewardData, MerchantServiceIndex);
        }
    }
}
