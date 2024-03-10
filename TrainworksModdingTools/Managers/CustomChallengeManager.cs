using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using Trainworks.Utilities;

namespace Trainworks.ManagersV2
{
    public static class CustomChallengeManager
    {
        private static Dictionary<string, SpChallengeData> CustomChallengeData = new Dictionary<string, SpChallengeData>();

        public static void RegisterCustomChallenge(SpChallengeData challengeData)
        {
            if (!CustomChallengeData.ContainsKey(challengeData.GetID()))
            {
                CustomChallengeData.Add(challengeData.GetID(), challengeData);
                ProviderManager.SaveManager.GetAllGameData().GetBalanceData().GetSpChallenges().Add(challengeData);
                // For some strange reason AllGameData has a private spChallengeData variable
                var challengeDatas = (List<SpChallengeData>)AccessTools.Field(typeof(AllGameData), "spChallengeDatas").GetValue(ProviderManager.SaveManager.GetAllGameData());
                challengeDatas.Add(challengeData);
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate challenge data with name: " + challengeData.name);
            }
        }

        public static SpChallengeData FindChallengeByID(string id)
        {
            var guid = GUIDGenerator.GenerateDeterministicGUID(id);
            if (CustomChallengeData.ContainsKey(guid))
            {
                return CustomChallengeData[guid];
            }

            var challenge = ProviderManager.SaveManager.GetAllGameData().FindSpChallengeData(id);
            if (challenge != null)
            {
                return challenge;
            }

            Trainworks.Log(LogLevel.Warning, "Couldn't find challenge: " + id + " - This will cause crashes.");
            return null;

        }
    }
}
