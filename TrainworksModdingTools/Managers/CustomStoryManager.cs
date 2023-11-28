using BepInEx.Logging;
using HarmonyLib;
using Malee;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.ConstantsV2;
using Trainworks.Utilities;

namespace Trainworks.Managers
{
    public static class CustomStoryManager
    {
        /// <summary>
        /// Maps StoryID to StoryEventData
        /// </summary>
        public static Dictionary<String, StoryEventData> CustomStoryData = new Dictionary<String, StoryEventData>();

        /// <summary>
        /// Registers a Custom Story.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cavernEvent"></param>
        public static void RegisterCustomStory(StoryEventData data, bool cavernEvent = true)
        {
            if (!CustomStoryData.ContainsKey(data.GetID()))
            {
                CustomStoryData.Add(data.GetID(), data);

                // Add story to AllGameData.
                AllGameData allGameData = ProviderManager.SaveManager.GetAllGameData();
                var list = (List<StoryEventData>)AccessTools.Field(typeof(AllGameData), "storyEventDatas").GetValue(allGameData);
                list.Add(data);

                // Add story to appear in cavern event pools if requested.
                if (cavernEvent)
                {
                    StoryEventPoolData pool = (StoryEventPoolData)allGameData.FindMapNodeData(VanillaMapNodeIDs.StoryEventPoolData);
                    var stories = (ReorderableArray<StoryEventData>)AccessTools.Field(typeof(StoryEventPoolData), "storyEvents").GetValue(pool);
                    stories.Add(data);
                }
            }
            else
            {
                Trainworks.Log(LogLevel.Warning, "Attempted to register duplicate story with name: " + data.name);
            }
        }

        /// <summary>
        /// Find a custom story (or vanilla) with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StoryEventData GetStoryDataByID(String id)
        {
            // Search for custom stories matching ID
            var guid = GUIDGenerator.GenerateDeterministicGUID(id);
            if (CustomStoryData.ContainsKey(guid))
            {
                return CustomStoryData[guid];
            }

            // No custom story found; search for vanilla stories matching ID
            var vanillaStory = ProviderManager.SaveManager.GetAllGameData().FindStoryEventData(id);
            if (vanillaStory == null)
            {
                Trainworks.Log(LogLevel.Warning, "Couldn't find story: " + id + " - This will cause crashes.");
            }
            return vanillaStory;
        }

        public static bool AddInkStory(String knotName, String json)
        {
            return false;
        }

        public static bool AddInkGlobalVariable(String name, int value)
        {
            return false;
        }
    }
}