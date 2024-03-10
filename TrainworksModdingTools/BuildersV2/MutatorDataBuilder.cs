using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.ManagersV2;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class MutatorDataBuilder
    {
        private string mutatorID;

        /// <summary>
        /// Unique string used to store and retrieve the mutator data.
        /// Implicitly sets NameKey and DescriptionKey.
        /// </summary>
        public string MutatorID
        {
            get { return mutatorID; }
            set
            {
                mutatorID = value;
                if (NameKey == null)
                {
                    NameKey = mutatorID + "_MutatorData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = mutatorID + "_MutatorData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Name displayed for the mutator.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Localization key for the mutator's name.
        /// Note that setting MutatorID sets this field to [mutatorID]_MutatorData_NameKey.
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Description displayed for the relic.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Localization key for the relic's description.
        /// Note that setting MutatorID sets this field to [mutatorID]_MutatorData_DescriptionKey.
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// Convenience Builders for Effects.
        /// </summary>
        public List<RelicEffectDataBuilder> EffectBuilders { get; set; }
        /// <summary>
        /// Relic Effect Data for the Mutator.
        /// </summary>
        public List<RelicEffectData> Effects { get; set; }
        public string RelicActivatedKey { get; set; }
        public List<string> RelicLoreTooltipKeys { get; set; }
        /// <summary>
        /// This typically ranges from -10 to +10.
        /// <br></br>
        /// Positive numbers indicate beneficial mutators, while negative numbers indicate harmful ones.
        /// <br></br>
        /// When a player clicks the 'randomize mutators' button, one positive, one negative, and one other mutator will be chosen. The total combined boon value will be -5 to -2.
        /// </summary>
        public int BoonValue { get; set; }
        /// <summary>
        /// Tags are metadata that can place the mutator into different categories.
        /// <br></br>
        /// They are mutually exclusive, and no two tags will ever be the same when mutators are selected randomly.
        /// <br></br>
        /// ex: 'friendlyeffect'
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// This determines whether the player needs the DLC to be active to use this mutator.
        /// <br></br>
        /// Defaults to 'DLC.None'.
        /// </summary>
        public ShinyShoe.DLC RequiredDLC { get; set; }
        /// <summary>
        /// When set to 'true', this mutator will never be picked randomly.
        /// </summary>
        public bool DisableInDailyChallenges { get; set; }
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + IconPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Custom asset path to load mutator art from. 76x76 image.
        /// </summary>
        public string IconPath { get; set; }

        public MutatorDataBuilder()
        {
            Effects = new List<RelicEffectData>();
            EffectBuilders = new List<RelicEffectDataBuilder>();
            RelicLoreTooltipKeys = new List<string>();
            BoonValue = 0;
            DisableInDailyChallenges = false;
            RequiredDLC = ShinyShoe.DLC.None;
            Tags = new List<string>();

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the RelicData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered MutatorData</returns>
        public MutatorData BuildAndRegister()
        {
            var mutatorData = Build();
            CustomMutatorManager.RegisterCustomMutator(mutatorData);
            return mutatorData;
        }

        /// <summary>
        /// Builds the RelicData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created MutatorData</returns>
        public MutatorData Build()
        {
            if (MutatorID == null)
            {
                throw new BuilderException("MutatorID is required");
            }

            var relicData = ScriptableObject.CreateInstance<MutatorData>();

            var guid = GUIDGenerator.GenerateDeterministicGUID(MutatorID);
            AccessTools.Field(typeof(GameData), "id").SetValue(relicData, guid);
            relicData.name = MutatorID;

            // RelicData fields
            // Object doesn't initialize effects so can't relicData.GetEffects() here.
            var effects = new List<RelicEffectData>();
            effects.AddRange(Effects);
            foreach (var builder in EffectBuilders)
            {
                effects.Add(builder.Build());
            }

            AccessTools.Field(typeof(RelicData), "descriptionKey").SetValue(relicData, DescriptionKey);
            AccessTools.Field(typeof(RelicData), "effects").SetValue(relicData, effects);
            AccessTools.Field(typeof(RelicData), "nameKey").SetValue(relicData, NameKey);
            AccessTools.Field(typeof(RelicData), "relicActivatedKey").SetValue(relicData, RelicActivatedKey);
            AccessTools.Field(typeof(RelicData), "relicLoreTooltipKeys").SetValue(relicData, RelicLoreTooltipKeys);
            if (IconPath != null)
            {
                Sprite iconSprite = CustomAssetManager.LoadSpriteFromPath(FullAssetPath);
                AccessTools.Field(typeof(RelicData), "icon").SetValue(relicData, iconSprite);
            }

            // MutatorData fields
            AccessTools.Field(typeof(MutatorData), "boonValue").SetValue(relicData, BoonValue);
            AccessTools.Field(typeof(MutatorData), "tags").SetValue(relicData, Tags);
            AccessTools.Field(typeof(MutatorData), "disableInDailyChallenges").SetValue(relicData, DisableInDailyChallenges);
            AccessTools.Field(typeof(MutatorData), "requiredDLC").SetValue(relicData, RequiredDLC);

            BuilderUtils.ImportStandardLocalization(NameKey, Name);
            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);
            return relicData;
        }
    }
}