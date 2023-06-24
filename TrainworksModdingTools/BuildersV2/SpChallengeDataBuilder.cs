using HarmonyLib;
using ShinyShoe;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.ManagersV2;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class SpChallengeDataBuilder
    {
        private string challengeID;

        /// <summary>
        /// Unique string used to store and retrieve the challenge data.
        /// Implicitly sets NameKey and DescriptionKey.
        /// </summary>
        public string ChallengeID
        {
            get { return challengeID; }
            set
            {
                challengeID = value;
                if (NameKey == null)
                {
                    NameKey = challengeID + "_SpChallengeData_NameKey";
                }
                if (DescriptionKey == null)
                {
                    DescriptionKey = challengeID + "_SpChallengeData_DescriptionKey";
                }
            }
        }
        /// <summary>
        /// Name displayed for the challenge.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Localization key for the challenge's name.
        /// Note that setting challengeID sets this field to [ChallengeID]_SpChallengeData_NameKey.
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// Description displayed for the challenge.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Localization key for the challenge's description.
        /// Note that setting challengeID sets this field to [ChallengeID]_SpChallengeData_DescriptionKey.
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// List of Mutators to add to the ExpertChallenge to.
        /// </summary>
        public List<MutatorData> Mutators { get; set; }
        /// <summary>
        /// Convenience Builders for Mutators. Will be appended to Mutators.
        /// </summary>
        public List<MutatorDataBuilder> MutatorBuilders { get; set; }
        /// <summary>
        /// Require DLC.
        /// </summary>
        public DLC RequiredDLC { get; set; }
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + IconPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Not required to be set as it appears to be unused, but here for future-proofing.
        /// </summary>
        public string IconPath { get; set; }

        public SpChallengeDataBuilder()
        {
            Mutators = new List<MutatorData>();
            MutatorBuilders = new List<MutatorDataBuilder>();
            RequiredDLC = DLC.None;
            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the RelicData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered SpChallengeData</returns>
        public SpChallengeData BuildAndRegister()
        {
            var challengeData = Build();
            CustomChallengeManager.RegisterCustomChallenge(challengeData);
            return challengeData;
        }

        /// <summary>
        /// Builds the RelicData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created RelicData</returns>
        public SpChallengeData Build()
        {
            if (ChallengeID == null)
            {
                throw new BuilderException("ChallengeID is required");
            }

            var challengeData = ScriptableObject.CreateInstance<SpChallengeData>();
            var guid = GUIDGenerator.GenerateDeterministicGUID(ChallengeID);
            AccessTools.Field(typeof(GameData), "id").SetValue(challengeData, guid);
            challengeData.name = ChallengeID;

            // Object doesn't initialize mutators so can't challengeData.GetMutators() here.
            var mutators = new List<MutatorData>();
            mutators.AddRange(Mutators);
            foreach (var builder in MutatorBuilders)
            {
                mutators.Add(builder.Build());
            }

            // RelicData fields
            AccessTools.Field(typeof(SpChallengeData), "nameKey").SetValue(challengeData, NameKey);
            AccessTools.Field(typeof(SpChallengeData), "descriptionKey").SetValue(challengeData, DescriptionKey);
            AccessTools.Field(typeof(SpChallengeData), "requiredDLC").SetValue(challengeData, RequiredDLC);
            AccessTools.Field(typeof(SpChallengeData), "mutators").SetValue(challengeData, mutators);
            if (IconPath != null)
            {
                Sprite iconSprite = CustomAssetManager.LoadSpriteFromPath(FullAssetPath);
                AccessTools.Field(typeof(SpChallengeData), "icon").SetValue(challengeData, iconSprite);
            }

            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);
            BuilderUtils.ImportStandardLocalization(NameKey, Name);

            return challengeData;
        }
    }
}
