using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Enums.MTTriggers;
using static BattleTriggeredEvents;

namespace Trainworks.Patches
{
    [HarmonyPatch(typeof(BattleTriggeredEvents), nameof(BattleTriggeredEvents.Init))]
    public class CustomBattleTriggeredEventsPatch
    {
        public static void Postfix(Dictionary<Team.Type, TriggerDictionaries> ___teamTriggerDictionaries)
        {
            Array team_types = Enum.GetValues(typeof(Team.Type));

            string[] character_triggers = CharacterTrigger.GetAllNames();

            foreach (Team.Type item in team_types)
            {
                foreach (string item2 in character_triggers)
                {
                    CharacterTriggerData.Trigger trigger = CharacterTrigger.GetValueOrDefault(item2).GetEnum();
                    ___teamTriggerDictionaries[item].triggerDictionary[trigger] = new BattleTriggers();
                }
            }
        }
    }
}
