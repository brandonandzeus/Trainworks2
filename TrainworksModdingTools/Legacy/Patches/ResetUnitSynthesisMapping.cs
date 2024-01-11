using HarmonyLib;
using Trainworks.Managers;
using System;

// Only here for backwards compatibility. This used to recompute the internal unit synthesis mapping dictionary.
// But it was an inefficient solution requiring it to be called per Plugin which recomputes them all every time.
namespace Trainworks.Patches
{
    public class AccessUnitSynthesisMapping
    {
        [Obsolete("AccessUnitSynthesisMapping.FindUnitSynthesisMappingInstanceToStub is deprecated and calls should be removed.")]
        public static void FindUnitSynthesisMappingInstanceToStub()
        {
            Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "This function is deprecated and calls should be removed.");
        }
    }


    class RecallingCollectMappingData
    {
        [Obsolete("RecallingCollectMappingData.CollectMappingDataStub is deprecated and calls should be removed.")]
        public static void CollectMappingDataStub(object instance)
        {
            // It's a stub so it has no initial content
        }
    }
}
