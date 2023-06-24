using HarmonyLib;
using ShinyShoe;
using System;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    /// <summary>
    /// Builder class to aid in creating custom clans.
    /// </summary>
    public class ClassDataBuilder
    {
        private string classID;
        /// <summary>
        /// Unique string used to store and retrieve the clan data.
        /// Implictly sets TitleLoc, DescriptionLoc, and SubclassDescriptionLoc if those values are null.
        /// </summary>
        public string ClassID
        {
            get { return classID; }
            set
            {
                classID = value;
                if (TitleLoc == null)
                {
                    TitleLoc = classID + "_ClassData_TitleLoc";
                }
                if (DescriptionLoc == null)
                {
                    DescriptionLoc = classID + "_ClassData_DescriptionLoc";
                }
                if (SubclassDescriptionLoc == null)
                {
                    SubclassDescriptionLoc = classID + "_ClassData_SubclassDescriptionLoc";
                }
            }
        }
        /// <summary>
        /// Name of the clan.
        /// Note if set will set the localization across all languages.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the clan.
        /// Note if set will set the localization across all languages.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Description of the clan when selected as the allied clan.
        /// Note if set will set the localization across all languages.
        /// </summary>
        public string SubclassDescription { get; set; }
        /// <summary>
        /// Please set storyChampionData and championCharacterArt
        /// </summary>
        public List<ChampionData> Champions { get; set; }
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Must contain 4 sprite paths; in order:
        /// small icon (32x32)
        /// medium icon (64x64)
        /// large icon (96x96)
        /// silhouette icon (43x43)
        /// Note that these exact sizes don't have to be followed, Unity will downsize as appropriate.
        /// </summary>
        public List<string> IconAssetPaths { get; set; }
        /// <summary>
        /// Existing clans CardStyle to use. If you wish to use a custom CardFrame for this clan use CardFrameUnit/Spell Path instead.
        /// </summary>
        public ClassCardStyle CardStyle { get; set; }
        /// <summary>
        /// Add a custom CardFrame as a sprite for unit cards.
        /// </summary>
        public string CardFrameUnitPath { get; set; }
        /// <summary>
        /// Add a custom CardFrame as a sprite for spell cards.
        /// </summary>
        public string CardFrameSpellPath { get; set; }
        /// <summary>
        /// Custom icon for the card draft on battle victory.
        /// </summary>
        public string DraftIconPath { get; set; }
        /// <summary>
        /// Clan Ui Color used in some places.
        /// </summary>
        public Color UiColor { get; set; }
        /// <summary>
        /// Clan Ui Dark Color used in some places.
        /// </summary>
        public Color UiColorDark { get; set; }
        /// <summary>
        /// Condition to check to unlock the clan.
        /// </summary>
        public MetagameSaveData.TrackedValue ClassUnlockCondition { get; set; }
        /// <summary>
        /// Parameter for Class Unlock Condition.
        /// </summary>
        public int ClassUnlockParam { get; set; }
        /// <summary>
        /// Unlock Preivew Texts
        /// Note that this should be a list of size 10 of localization keys for the unlock preview text.
        /// </summary>
        public List<string> ClassUnlockPreviewTexts { get; set; }
        /// <summary>
        /// Gives the clan a starter relic.
        /// Important note in terms of processing effects, the starter relics will always be executed first.
        /// Not important for many RelicEffect Interfaces, but do note that the Covenants are implemented
        /// as Relics. So things like applying additional effects the starter deck can't be done easily without
        /// a patch, Since Covenant 1 removes all starter cards, and adds them back with a CardSet.
        /// </summary>
        public List<RelicData> StarterRelics { get; set; }
        /// <summary>
        /// Require DLC, Currently Hellforged is The Last Divinity.
        /// </summary>
        public DLC RequiredDLC { get; set; }
        /// <summary>
        /// Clan Select SFX Cue.
        /// </summary>
        public string ClanSelectSfxCue { get; set; }
        /// <summary>
        /// Enables charged echo floor effects for clan.
        /// Note that if you want a wurmkin clan clone StarterCardUpgrade and
        /// RandomDraftEnhancerPool also need to be set to apply CardTraitCorruption to cards.
        /// </summary>
        public bool CorruptionEnabled { get; set; }
        /// <summary>
        /// Upgrade applied to all starter cards.
        /// </summary>
        public CardUpgradeData StarterCardUpgrade { get; set; }
        /// <summary>
        /// Convenience Builder object for StarterCardUpgrade. If set overrides StarterCardUpgrade.
        /// </summary>
        public CardUpgradeDataBuilder StarterCardUpgradeBuilder { get; set; }
        /// <summary>
        /// A enhancer from this pool is applied to 1 random card in each draft.
        /// </summary>
        public EnhancerPool RandomDraftEnhancerPool { get; set; }
        /// <summary>
        /// Convenience Builder object for RandomDraftEnhancerPool. If set overrides RandomDraftEnhancerPool.
        /// </summary>
        public EnhancerPoolBuilder RandomDraftEnhancerPoolBuilder { get; set; }
        /// <summary>
        /// Localization key for Clan Title.
        /// You shouldn't need to set this as its automatically set by ClassID
        /// </summary>
        public string TitleLoc { get; set; }
        /// <summary>
        /// Localization key for Clan Description.
        /// You shouldn't need to set this as its automatically set by ClassID
        /// </summary>
        public string DescriptionLoc { get; set; }
        /// <summary>
        /// Localization key for Clan Subclass Description.
        /// You shouldn't need to set this as its automatically set by ClassID
        /// </summary>}
        public string SubclassDescriptionLoc { get; set; }
        /// <summary>
        /// Unused currently. Sets the Character ID to display in the Clan Select Screen
        /// when the clan is selected as main.
        /// </summary>
        public string[] ClassSelectScreenCharacterIDsMain { get; set; }
        /// <summary>
        /// Unused currently. Sets the Character ID to display in the Clan Select Screen
        /// when the clan is selected as secondary.
        /// </summary>
        public string[] ClassSelectScreenCharacterIDsSub { get; set; }

        public ClassDataBuilder()
        {
            IconAssetPaths = new List<string>();
            ClassUnlockPreviewTexts = new List<string>();
            Champions = new List<ChampionData> {
                ScriptableObject.CreateInstance<ChampionData>(),
                ScriptableObject.CreateInstance<ChampionData>()
            };
            RequiredDLC = DLC.None;
            StarterRelics = new List<RelicData>();
            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the ClassData represented by this builder's parameters recursively
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered ClassData</returns>
        public ClassData BuildAndRegister()
        {
            var classData = Build();
            CustomClassManager.RegisterCustomClass(classData);
            return classData;
        }

        /// <summary>
        /// Builds the ClassData represented by this builder's parameters recursively;
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created ClassData</returns>
        public ClassData Build()
        {
            if (ClassID == null)
            {
                throw new BuilderException("ClassID is required");
            }

            ClassData classData = ScriptableObject.CreateInstance<ClassData>();
            classData.name = ClassID;

            AccessTools.Field(typeof(ClassData), "id").SetValue(classData, GUIDGenerator.GenerateDeterministicGUID(ClassID));
            AccessTools.Field(typeof(ClassData), "cardStyle").SetValue(classData, CardStyle);
            AccessTools.Field(typeof(ClassData), "champions").SetValue(classData, Champions);
            AccessTools.Field(typeof(ClassData), "classUnlockCondition").SetValue(classData, ClassUnlockCondition);
            AccessTools.Field(typeof(ClassData), "classUnlockParam").SetValue(classData, ClassUnlockParam);
            AccessTools.Field(typeof(ClassData), "classUnlockPreviewTexts").SetValue(classData, ClassUnlockPreviewTexts);
            AccessTools.Field(typeof(ClassData), "corruptionEnabled").SetValue(classData, CorruptionEnabled);
            AccessTools.Field(typeof(ClassData), "descriptionLoc").SetValue(classData, DescriptionLoc);
            AccessTools.Field(typeof(ClassData), "randomDraftEnhancerPool").SetValue(classData, RandomDraftEnhancerPool);
            AccessTools.Field(typeof(ClassData), "requiredDlc").SetValue(classData, RequiredDLC);
            AccessTools.Field(typeof(ClassData), "starterCardUpgrade").SetValue(classData, StarterCardUpgrade);
            AccessTools.Field(typeof(ClassData), "starterRelics").SetValue(classData, StarterRelics);
            AccessTools.Field(typeof(ClassData), "subclassDescriptionLoc").SetValue(classData, SubclassDescriptionLoc);
            AccessTools.Field(typeof(ClassData), "titleLoc").SetValue(classData, TitleLoc);
            AccessTools.Field(typeof(ClassData), "uiColor").SetValue(classData, UiColor);
            AccessTools.Field(typeof(ClassData), "uiColorDark").SetValue(classData, UiColorDark);

            // Clan Icons
            var icons = new List<Sprite>();
            foreach (string iconPath in IconAssetPaths)
            {
                icons.Add(CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + iconPath));
            }
            Type iconSetType = AccessTools.Inner(typeof(ClassData), "IconSet");
            var iconSet = Activator.CreateInstance(iconSetType);
            AccessTools.Field(iconSetType, "small").SetValue(iconSet, icons[0]);
            AccessTools.Field(iconSetType, "medium").SetValue(iconSet, icons[1]);
            AccessTools.Field(iconSetType, "large").SetValue(iconSet, icons[2]);
            AccessTools.Field(iconSetType, "silhouette").SetValue(iconSet, icons[3]);
            AccessTools.Field(typeof(ClassData), "icons").SetValue(classData, iconSet);

            // Card Frame
            if (CardFrameSpellPath != null && CardFrameUnitPath != null)
            {
                Sprite cardFrameSpellSprite = CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + CardFrameSpellPath);
                Sprite cardFrameUnitSprite = CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + CardFrameUnitPath);
                CustomClassManager.CustomClassFrame.Add(GUIDGenerator.GenerateDeterministicGUID(ClassID), new List<Sprite>() { cardFrameUnitSprite, cardFrameSpellSprite });
            }

            // Draft Icon
            if (DraftIconPath != null)
            {
                Sprite draftIconSprite = CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + DraftIconPath);
                CustomClassManager.CustomClassDraftIcons.Add(GUIDGenerator.GenerateDeterministicGUID(ClassID), draftIconSprite);
            }
            // Class select character IDs
            CustomClassManager.CustomClassSelectScreenCharacterIDsMain.Add(GUIDGenerator.GenerateDeterministicGUID(ClassID), ClassSelectScreenCharacterIDsMain);
            CustomClassManager.CustomClassSelectScreenCharacterIDsSub.Add(GUIDGenerator.GenerateDeterministicGUID(ClassID), ClassSelectScreenCharacterIDsSub);

            BuilderUtils.ImportStandardLocalization(DescriptionLoc, Description);
            BuilderUtils.ImportStandardLocalization(SubclassDescriptionLoc, SubclassDescription);
            BuilderUtils.ImportStandardLocalization(TitleLoc, Name);

            return classData;
        }
    }
}