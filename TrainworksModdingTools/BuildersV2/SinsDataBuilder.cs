using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Trainworks.Managers;
using Trainworks.ManagersV2;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    /// <summary>
    /// Builder for SinsData. SinsData represents the beneficial effects given to enemies in a battle.
    /// </summary>
    public class SinsDataBuilder
    {
        private string sinsID;

        /// <summary>
        /// Unique string used to store and retrieve the sins data.
        /// Implicitly sets NameKey and DescriptionKey.
        /// </summary>
        public string SinsID
        {
            get { return sinsID; }
            set
            {
                sinsID = value;
                if (NameKey == null)
                {
                    NameKey = sinsID + "_SinsData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = sinsID + "_SinsData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Name displayed for the sin.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Localization key for the sin's name.
        /// Note that setting SinsID sets this field to [sinsID]_SinsData_NameKey.
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Description displayed for the sin.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Localization key for the sin's description.
        /// Note that setting SinsID sets this field to [sinsID]_SinsData_DescriptionKey.
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// Convenience Builders for Effects.
        /// </summary>
        public List<RelicEffectDataBuilder> EffectBuilders { get; set; }
        /// <summary>
        /// Relic Effect Data for the Sin.
        /// </summary>
        public List<RelicEffectData> Effects { get; set; }
        public string RelicActivatedKey { get; set; }
        public List<string> RelicLoreTooltipKeys { get; set; }
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + IconPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Custom asset path to load sin art from.
        /// </summary>
        public string IconPath { get; set; }

        public SinsDataBuilder()
        {
            Effects = new List<RelicEffectData>();
            EffectBuilders = new List<RelicEffectDataBuilder>();
            RelicLoreTooltipKeys = new List<string>();

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the RelicData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered SinsData</returns>
        public SinsData BuildAndRegister()
        {
            var sinsData = Build();
            CustomScenarioManager.RegisterCustomSin(sinsData);
            return sinsData;
        }

        /// <summary>
        /// Builds the RelicData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created SinsData</returns>
        public SinsData Build()
        {
            if (SinsID == null)
            {
                throw new BuilderException("SinsID is required");
            }

            var relicData = ScriptableObject.CreateInstance<SinsData>();

            var guid = GUIDGenerator.GenerateDeterministicGUID(SinsID);
            AccessTools.Field(typeof(GameData), "id").SetValue(relicData, guid);
            relicData.name = SinsID;

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

            BuilderUtils.ImportStandardLocalization(NameKey, Name);
            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);
            return relicData;
        }
    }
}
