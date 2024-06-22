using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    // Changed Icon -> IconPath to be consistent across DataBuilders.
    public class RoomModifierDataBuilder
    {
        private string roomModifierID;

        /// <summary>
        /// Unique ID
        /// Implicitly sets DescriptionKey and DescriptionKeyInPlay if null.
        /// </summary>
        public string RoomModifierID
        {
            get { return roomModifierID; }
            set
            {
                roomModifierID = value;
                if (DescriptionKey == null)
                {
                    DescriptionKey = roomModifierID + "_RoomModifierData_DescriptionKey";
                }
                if (DescriptionKeyInPlay == null)
                {
                    DescriptionKeyInPlay = roomModifierID + "_RoomModifierData_DescriptionKeyInPlay";
                }
            }
        }

        /// <summary>
        /// Type of the room modifier class to instantiate.
        /// This should be a class inheriting from RoomStateModifierBase.
        /// </summary>
        public Type RoomModifierClassType { get; set; }
        /// <summary>
        /// RoomModifier class to instantiate.
        /// Note that this isn't a simple string name of the class it is the class name plus the Assembly info if necessary.
        /// </summary>
        public string RoomModifierClassName => BuilderUtils.GetEffectClassName(RoomModifierClassType);
        /// <summary>
        /// The description. Used in card text.
        /// Note if this is set this will set the localized text across all languages.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The description in play. Used in character text if set.
        /// Note if this is set this will set the localized text across all languages.
        /// </summary>
        public string DescriptionInPlay { get; set; }
        /// <summary>
        /// Extra Tooltip Body text.
        /// Note if this is set this will set the localized text across all languages.
        /// You must set ExtraTooltipTitleKey if this is set.
        /// </summary>
        public string ExtraTooltipBody { get; set; }
        /// <summary>
        /// Extra Tooltip Title.
        /// Note if this is set this will set the localized text across all languages.
        /// You must set ExtraTooltipTitleKey if this is set.
        /// </summary>
        public string ExtraTooltipTitle { get; set; }
        /// <summary>
        /// Localization key for description. Default value is [RoomModifierID]_RoomModifierData_DescriptionKey.
        /// Note you shouldn't need to set this as its pre-set when setting RoomModifierID.
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// Localization key for descriptionInPlay. Default value is [RoomModifierID]_RoomModifierData_DescriptionKeyInPlay.
        /// Note you shouldn't need to set this as its pre-set when setting RoomModifierID.
        /// </summary>
        public string DescriptionKeyInPlay { get; set; }
        /// <summary>
        /// Localization key for extra tooltip title.
        /// Note this isn't set automatically.
        /// </summary>
        public string ExtraTooltipTitleKey { get; set; }
        /// <summary>
        /// Localization key for extra tooltip boy.
        /// Note this isn't set automatically.
        /// </summary>
        public string ExtraTooltipBodyKey { get; set; }

        public int ParamInt { get; set; }
        public string ParamSubtype { get; set; }
        public CardUpgradeData ParamCardUpgradeData { get; set; }
        /// <summary>
        /// Convenience Builder for CardUpgradeData Parameter.
        /// if set overrides ParamCardUpgradeData.
        /// </summary>
        public CardUpgradeDataBuilder ParamCardUpgradeDataBuilder { get; set; }
        public List<StatusEffectStackData> ParamStatusEffects { get; set; }
        /// <summary>
        /// The full, absolute path to the asset.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + IconPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Do not use. Setting this will not allow setting a custom icon if used as part of a Character's abilities.
        /// To set a custom icon call CustomCharacterManager.AddRoomModifierIcon which sets the Icon.
        /// RoomModifier icons are controlled by RoomStateModifiersData and can't be overriden easily.
        /// </summary>
        public string IconPath { get; set; }

        public RoomModifierDataBuilder()
        {
            ParamSubtype = VanillaSubtypeIDs.None;
            ParamStatusEffects = new List<StatusEffectStackData>();
            ExtraTooltipBodyKey = "";
            ExtraTooltipTitleKey = "";

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the RoomModifierData represented by this builders's parameters
        /// </summary>
        /// <returns>The newly created RoomModifierData</returns>
        public RoomModifierData Build()
        {
            if (RoomModifierClassType == null)
            {
                throw new BuilderException("RoomModifierClassType is required");
            }
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (RoomModifierID == null)
            {
                Trainworks.Log(LogLevel.Error, "Warning should provide a RoomModifierID.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            }

            RoomModifierData roomModifierData = new RoomModifierData();

            AccessTools.Field(typeof(RoomModifierData), "descriptionKey").SetValue(roomModifierData, DescriptionKey);
            AccessTools.Field(typeof(RoomModifierData), "descriptionKeyInPlay").SetValue(roomModifierData, DescriptionKeyInPlay);
            AccessTools.Field(typeof(RoomModifierData), "extraTooltipBodyKey").SetValue(roomModifierData, ExtraTooltipBodyKey);
            AccessTools.Field(typeof(RoomModifierData), "extraTooltipTitleKey").SetValue(roomModifierData, ExtraTooltipTitleKey);
            AccessTools.Field(typeof(RoomModifierData), "paramInt").SetValue(roomModifierData, ParamInt);
            AccessTools.Field(typeof(RoomModifierData), "paramStatusEffects").SetValue(roomModifierData, ParamStatusEffects.ToArray());
            AccessTools.Field(typeof(RoomModifierData), "paramSubtype").SetValue(roomModifierData, ParamSubtype);
            AccessTools.Field(typeof(RoomModifierData), "roomStateModifierClassName").SetValue(roomModifierData, RoomModifierClassName);

            var upgrade = ParamCardUpgradeData;
            if (ParamCardUpgradeDataBuilder != null)
            {
                upgrade = ParamCardUpgradeDataBuilder.Build();
            }

            AccessTools.Field(typeof(RoomModifierData), "paramCardUpgardeData" /* sic */).SetValue(roomModifierData, upgrade);

            if (IconPath != null)
            {
                Sprite sprite = CustomAssetManager.LoadSpriteFromPath(FullAssetPath);
                AccessTools.Field(typeof(RoomModifierData), "icon").SetValue(roomModifierData, sprite);
            }

            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);
            BuilderUtils.ImportStandardLocalization(DescriptionKeyInPlay, DescriptionInPlay);
            BuilderUtils.ImportStandardLocalization(ExtraTooltipBodyKey, ExtraTooltipBody);
            BuilderUtils.ImportStandardLocalization(ExtraTooltipTitleKey, ExtraTooltipTitle);

            return roomModifierData;
        }
    }
}
