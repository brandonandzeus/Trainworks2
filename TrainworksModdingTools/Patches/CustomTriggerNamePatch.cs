using HarmonyLib;
using System;
using System.Text.RegularExpressions;

namespace Trainworks.Patches
{
    // Patch to prevent a rare incorrect localization key being formed for a tooltip.
    // Fixes Trigger tooltips for CardEffectPlayUnitTrigger and CardEffectReplayTriggers.
    [HarmonyPatch(typeof(TooltipUI), nameof(TooltipUI.InitCardExplicitTooltip))]
    public class CustomTriggerNamePatch
    {
        private static Regex pattern = new Regex(@"Trigger_(?<triggerid>\d+)_");

        public static void Prefix(ref string titleKey, ref string descriptionKey)
        {
            if (titleKey == null || descriptionKey == null) return;

            var match = pattern.Match(titleKey);
            if (!match.Success) return;


            var triggerId = (CharacterTriggerData.Trigger)Int32.Parse(match.Groups["triggerid"].Value);
            var baseKey = CharacterTriggerData.TriggerToLocalizationExpression[triggerId];

            titleKey = baseKey + "_CardText";
            descriptionKey = baseKey + "_TooltipText";
        }
    }
}
