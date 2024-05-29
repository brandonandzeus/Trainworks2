using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using Trainworks.Managers;
using Trainworks.Utilities;

namespace Trainworks.ManagersV2
{
    public static class CustomChallengeManager
    {
        private static readonly Dictionary<string, SpChallengeData> CustomChallengeData = new Dictionary<string, SpChallengeData>();

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

        /// <summary>
        /// Given a GUID or ChallengeID (from SpChallengeDataBuilder) determines if the id is a custom challenge.
        /// </summary>
        /// <param name="id">GUID, note this is the SpChallengeData's id not the ChallengeID passed to SpChallengeDataBuilder.</param>
        public static bool IsCustomChallenge(string id)
        {
            return CustomChallengeData.ContainsKey(id) || CustomChallengeData.ContainsKey(GUIDGenerator.GenerateDeterministicGUID(id));
        }
    }
}
