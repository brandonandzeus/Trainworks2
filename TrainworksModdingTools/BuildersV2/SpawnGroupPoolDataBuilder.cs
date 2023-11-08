using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.BuildersV2
{
    public class SpawnGroupPoolDataBuilder
    {
        /// <summary>
        /// Possible Groups of enemies to spawn.
        /// </summary>
        public List<SpawnGroupDataBuilder> PossibleGroups {  get; set; }

        public SpawnGroupPoolDataBuilder()
        {
            PossibleGroups = new List<SpawnGroupDataBuilder>();
        }

        public SpawnGroupPoolData Build()
        {
            var spawnGroupPoolData = new SpawnGroupPoolData();

            SpawnGroupPoolData.SpawnGroupDataList list = (SpawnGroupPoolData.SpawnGroupDataList) AccessTools.Field(typeof(SpawnGroupPoolData), "possibleGroups").GetValue(spawnGroupPoolData);
            list.Clear();
            foreach (var builder in PossibleGroups)
            {
                list.Add(builder.Build());
            }

            return spawnGroupPoolData;
        }
    }
}
