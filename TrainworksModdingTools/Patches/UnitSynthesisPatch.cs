using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;

namespace Trainworks.Patches
{
    /// <summary>
    /// Supercedes the depreacated ResetUnitSynthesisMapping.
    /// </summary>
    [HarmonyPatch(typeof(UnitSynthesisMapping), nameof(UnitSynthesisMapping.GetUpgradeData))]
    public class UnitSynthesisPatch
    {
        public static void Postfix(CharacterData characterData, ref CardUpgradeData __result)
        {
            var upgrade = CustomCharacterManager.GetUnitSynthesis(characterData);
            if (upgrade != null)
            {
                __result = upgrade;
            }
        }
    }
}
