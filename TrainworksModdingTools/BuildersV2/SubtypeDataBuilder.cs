using HarmonyLib;
using Trainworks.Managers;

namespace Trainworks.BuildersV2
{
    // Note changed properties to not include the leading underscore because it felt tacky and serves no purpose.
    public class SubtypeDataBuilder
    {
        /// <summary>
        /// Subtype string. This should be a localization key.
        /// </summary>
        public string Subtype { get; set; }
        public bool IsChampion { get; set; }
        public bool IsNone { get; set; }
        public bool IsTreasureCollector { get; set; }
        public bool IsImp { get; set; }
        public bool IsPyre { get; set; }

        public SubtypeData BuildAndRegister()
        {
            SubtypeData subtypeData = Build();
            CustomCharacterManager.CustomSubtypeData[subtypeData.Key] = subtypeData;
            return subtypeData;
        }

        /// <summary>
        /// Builds the SubtypeData represented by this builders's parameters;
        /// </summary>
        /// <returns>The newly created SubtypeData</returns>
        public SubtypeData Build()
        {
            // TODO this probably could be registered with SubtypeManager, However the original code patches
            // GetSubtypeList which I don't believe is necessary.
            SubtypeData subtypeData = new SubtypeData();
            AccessTools.Field(typeof(SubtypeData), "_subtype").SetValue(subtypeData, Subtype);
            AccessTools.Field(typeof(SubtypeData), "_isChampion").SetValue(subtypeData, IsChampion);
            AccessTools.Field(typeof(SubtypeData), "_isNone").SetValue(subtypeData, IsNone);
            AccessTools.Field(typeof(SubtypeData), "_isTreasureCollector").SetValue(subtypeData, IsTreasureCollector);
            AccessTools.Field(typeof(SubtypeData), "_isImp").SetValue(subtypeData, IsImp);
            AccessTools.Field(typeof(SubtypeData), "_isPyre").SetValue(subtypeData, IsPyre);
            return subtypeData;
        }
    }
}