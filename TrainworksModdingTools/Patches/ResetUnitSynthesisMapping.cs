using HarmonyLib;
using Trainworks.Managers;
using System;

namespace Trainworks.Patches
{
    public class AccessUnitSynthesisMapping
    {
        public static void FindUnitSynthesisMappingInstanceToStub()
        {
            Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "This function is deprecated and calls should be removed.");
        }
    }


    class RecallingCollectMappingData
    {
        public static void CollectMappingDataStub(object instance)
        {
            // It's a stub so it has no initial content
        }
    }
}
