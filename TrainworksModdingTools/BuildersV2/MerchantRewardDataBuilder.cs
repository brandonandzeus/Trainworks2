using HarmonyLib;

namespace Trainworks.BuildersV2
{
    public class MerchantRewardDataBuilder
    {
        public RewardData RewardData { get; set; }
        public IRewardDataBuilder RewardDataBuilder { get; set; }
        public MutatorData RequiredMutator { get; set; }
        public bool OncePerVisit { get; set; }

        public MerchantRewardDataBuilder()
        {
        }

        public MerchantRewardData Build()
        {
            var merchantReward = new MerchantRewardData();

            var rewardData = RewardData;
            if (RewardDataBuilder != null)
            {
                rewardData = RewardDataBuilder.Build();
            }

            AccessTools.Field(typeof(MerchantRewardData), "rewardData").SetValue(merchantReward, rewardData);
            AccessTools.Field(typeof(MerchantRewardData), "requiredMutator").SetValue(merchantReward, RequiredMutator);
            AccessTools.Field(typeof(MerchantRewardData), "oncePerVisit").SetValue(merchantReward, OncePerVisit);

            return merchantReward;
        }
    }
}
