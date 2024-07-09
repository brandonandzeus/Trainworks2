using HarmonyLib;
using System;

namespace Trainworks.Builders
{
    [Obsolete("Trainworks.Builders.SubtypeDataBuilder is deprecated and will no longer be unsupported. Please do not use if making a new mod. Trainworks.BuildersV2.SubtypeDataBuilder should be used for newer mods. See: https://github.com/brandonandzeus/Trainworks2/wiki/Upgrade-Trainworks-Tutorial")]
    public class SubtypeDataBuilder
    {
        public string _Subtype { get; set; }
        public bool _IsChampion { get; set; }
        public bool _IsNone { get; set; }
        public bool _IsTreasureCollector { get; set; }
        public bool _IsImp { get; set; }

        /// <summary>
        /// Builds the SubtypeData represented by this builders's parameters;
        /// </summary>
        /// <returns>The newly created SubtypeData</returns>
        public SubtypeData Build()
        {
            SubtypeData subtypeData = new SubtypeData();
            AccessTools.Field(typeof(SubtypeData), "_subtype").SetValue(subtypeData, this._Subtype);
            AccessTools.Field(typeof(SubtypeData), "_isChampion").SetValue(subtypeData, this._IsChampion);
            AccessTools.Field(typeof(SubtypeData), "_isNone").SetValue(subtypeData, this._IsNone);
            AccessTools.Field(typeof(SubtypeData), "_isTreasureCollector").SetValue(subtypeData, this._IsTreasureCollector);
            AccessTools.Field(typeof(SubtypeData), "_isImp").SetValue(subtypeData, this._IsImp);
            return subtypeData;
        }
    }
}
