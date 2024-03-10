using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;

namespace Trainworks.Managers
{
    public static class CustomRewardManager
    {
        public static readonly IDictionary<string, GrantableRewardData> CustomGrantableRewardData = new Dictionary<string, GrantableRewardData>();

        public static void RegisterCustomReward(RewardData reward)
        {
            if (reward is GrantableRewardData)
            {
                RegisterCustomGrantableReward(reward as GrantableRewardData);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register unknown reward type with name: " + reward.name);
            }
        }

        public static void RegisterCustomGrantableReward(GrantableRewardData reward)
        {
            if (!CustomGrantableRewardData.ContainsKey(reward.GetID()))
            {
                CustomGrantableRewardData.Add(reward.GetID(), reward);
                var allGameData = ProviderManager.SaveManager.GetAllGameData();
                var rewardDatas = (List<GrantableRewardData>)AccessTools.Field(typeof(AllGameData), "rewardDatas").GetValue(allGameData);
                rewardDatas.Add(reward);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate upgrade data with name: " + reward.name);
            }
        }
    }
}
