using HarmonyLib;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.Patches
{
    [HarmonyPatch(typeof(RoomStateModifierBase), nameof(RoomStateModifierBase.Initialize))]
    public class RoomModifierIconPatch
    {
        public static void Postfix(RoomModifierData roomModifierData, ref Sprite ___icon)
        {
            // Nothing to do.
            if (___icon != null)
                return;

            foreach (var class_name_sprite in CustomCharacterManager.CustomRoomModifierIcons)
            {
                if (class_name_sprite.Key == roomModifierData.GetRoomStateModifierClassName())
                {
                    ___icon = class_name_sprite.Value;
                    break;
                }
            }
        }
    }
}
