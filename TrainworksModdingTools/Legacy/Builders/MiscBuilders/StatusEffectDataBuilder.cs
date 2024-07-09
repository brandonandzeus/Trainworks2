﻿using HarmonyLib;
using System;
using System.Reflection;
using Trainworks.Managers;
using UnityEngine;
using static StatusEffectData;

namespace Trainworks.Builders
{
    [Obsolete("Trainworks.Builders.StatusEffectDataBuilder is deprecated and will no longer be unsupported. Please do not use if making a new mod. Trainworks.BuildersV2.StatusEffectDataBuilder should be used for newer mods. See: https://github.com/brandonandzeus/Trainworks2/wiki/Upgrade-Trainworks-Tutorial")]
    public class StatusEffectDataBuilder
    {
        /// <summary>
        /// Don't set directly; use StatusEffectStateType instead.
        /// Type of the status effect class to instantiate.
        /// </summary>
        public Type statusEffectStateType;

        /// <summary>
        /// Type of the status effect class to instantiate.
        /// Implicitly sets StatusEffectStateName.
        /// </summary>
        public Type StatusEffectStateType
        {
            get { return this.statusEffectStateType; }
            set
            {
                this.statusEffectStateType = value;
                this.StatusEffectStateName = this.statusEffectStateType.AssemblyQualifiedName;
            }
        }

        public string StatusEffectStateName { get; set; }
        public string StatusId { get; set; }
        public string IconPath { get; set; }
        public string AppliedSFXName { get; set; }
        public string TriggeredSFXName { get; set; }

        //[Tooltip("This category determines the color used for the icon.")]
        public StatusEffectData.DisplayCategory DisplayCategory { get; set; }

        //[Tooltip("The VFX to display on the character when the status effect is added.")]
        public VfxAtLoc AddedVFX { get; set; }
        public VfxAtLocList MoreAddedVFX { get; set; }

        //[Tooltip("The VFX to display on the character while this status effect is active")]
        public VfxAtLoc PersistentVFX { get; set; }
        public VfxAtLocList MorePersistentVFX { get; set; }

        //[Tooltip("The VFX to display on the character when the effect is triggered.")]
        public VfxAtLoc TriggeredVFX { get; set; }
        public VfxAtLocList MoreTriggeredVFX { get; set; }

        //[Tooltip("The VFX to display on the character when the status effect is removed.")]
        public VfxAtLoc RemovedVFX { get; set; }
        public VfxAtLocList MoreRemovedVFX { get; set; }

        //[Tooltip("The VFX to display on a character when it is damaged/affected by this effect.")]
        public VfxAtLoc AffectedVFX { get; set; }

        public TriggerStage TriggerStage { get; set; }
        public bool RemoveStackAtEndOfTurn { get; set; }
        public bool RemoveAtEndOfTurn { get; set; }
        public bool RemoveWhenTriggered { get; set; }

        //[Tooltip("This is the same as Remove When Triggered except it will be removed only after the card currently being played finishes playing\n\nNOTE: This should only be used for status effects that are triggered by a card being played.")]
        public bool RemoveWhenTriggeredAfterCardPlayed { get; set; }
        /// <summary>
        /// Whether or not the status effect is stackable. Defaults to true.
        /// </summary>
		public bool IsStackable { get; set; }
        /// <summary>
        /// Whether or not the status effect should show stacks in the card text. Defaults to true.
        /// </summary>
		public bool ShowStackCount { get; set; }
        /// <summary>
        /// Defaults to true.
        /// </summary>
		public bool ShowNotificationsOnRemoval { get; set; }
        public string ParamStr { get; set; }
        public int ParamInt { get; set; }
        public int ParamSecondaryInt { get; set; }
        public float ParamFloat { get; set; }
        public string BaseAssetPath { get; private set; }

        public StatusEffectDataBuilder()
        {
            IsStackable = true;
            ShowNotificationsOnRemoval = true;
            ShowStackCount = true;

            var assembly = Assembly.GetCallingAssembly();
            this.BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        public StatusEffectData Build()
        {
            StatusEffectData statusEffect = new StatusEffectData();

            AccessTools.Field(typeof(StatusEffectData), "addedVFX").SetValue(statusEffect, AddedVFX);
            AccessTools.Field(typeof(StatusEffectData), "affectedVFX").SetValue(statusEffect, AffectedVFX);
            AccessTools.Field(typeof(StatusEffectData), "appliedSFXName").SetValue(statusEffect, AppliedSFXName);
            AccessTools.Field(typeof(StatusEffectData), "displayCategory").SetValue(statusEffect, DisplayCategory);
            AccessTools.Field(typeof(StatusEffectData), "isStackable").SetValue(statusEffect, IsStackable);
            AccessTools.Field(typeof(StatusEffectData), "moreAddedVFX").SetValue(statusEffect, MoreAddedVFX);
            AccessTools.Field(typeof(StatusEffectData), "morePersistentVFX").SetValue(statusEffect, MorePersistentVFX);
            AccessTools.Field(typeof(StatusEffectData), "moreRemovedVFX").SetValue(statusEffect, MoreRemovedVFX);
            AccessTools.Field(typeof(StatusEffectData), "moreTriggeredVFX").SetValue(statusEffect, MoreTriggeredVFX);
            AccessTools.Field(typeof(StatusEffectData), "paramFloat").SetValue(statusEffect, ParamFloat);
            AccessTools.Field(typeof(StatusEffectData), "paramInt").SetValue(statusEffect, ParamInt);
            AccessTools.Field(typeof(StatusEffectData), "paramSecondaryInt").SetValue(statusEffect, ParamSecondaryInt);
            AccessTools.Field(typeof(StatusEffectData), "paramStr").SetValue(statusEffect, ParamStr);
            AccessTools.Field(typeof(StatusEffectData), "persistentVFX").SetValue(statusEffect, PersistentVFX);
            AccessTools.Field(typeof(StatusEffectData), "removeAtEndOfTurn").SetValue(statusEffect, RemoveAtEndOfTurn);
            AccessTools.Field(typeof(StatusEffectData), "removedVFX").SetValue(statusEffect, RemovedVFX);
            AccessTools.Field(typeof(StatusEffectData), "removeStackAtEndOfTurn").SetValue(statusEffect, RemoveStackAtEndOfTurn);
            AccessTools.Field(typeof(StatusEffectData), "removeWhenTriggered").SetValue(statusEffect, RemoveWhenTriggered);
            AccessTools.Field(typeof(StatusEffectData), "removeWhenTriggeredAfterCardPlayed").SetValue(statusEffect, RemoveWhenTriggeredAfterCardPlayed);
            AccessTools.Field(typeof(StatusEffectData), "showNotificationsOnRemoval").SetValue(statusEffect, ShowNotificationsOnRemoval);
            AccessTools.Field(typeof(StatusEffectData), "showStackCount").SetValue(statusEffect, ShowStackCount);
            AccessTools.Field(typeof(StatusEffectData), "statusEffectStateName").SetValue(statusEffect, StatusEffectStateName);
            AccessTools.Field(typeof(StatusEffectData), "statusId").SetValue(statusEffect, StatusId);
            AccessTools.Field(typeof(StatusEffectData), "triggeredSFXName").SetValue(statusEffect, TriggeredSFXName);
            AccessTools.Field(typeof(StatusEffectData), "triggeredVFX").SetValue(statusEffect, TriggeredVFX);
            AccessTools.Field(typeof(StatusEffectData), "triggerStage").SetValue(statusEffect, TriggerStage);

            if (this.IconPath != null)
            {
                Sprite draftIconSprite = CustomAssetManager.LoadSpriteFromPath(this.BaseAssetPath + "/" + this.IconPath);
                AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(statusEffect, draftIconSprite);
            }

            var manager = StatusEffectManager.Instance;
            manager.GetAllStatusEffectsData().GetStatusEffectData().Add(statusEffect);
            // To keep parity with old API, don't capitalize the status.
            string idkey = "StatusEffect_" + StatusId;
            StatusEffectManager.StatusIdToLocalizationExpression.Add(StatusId, idkey);

            return statusEffect;
        }
    }
}
