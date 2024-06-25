using System.Collections.Generic;
using System.IO;
using Trainworks.Builders;
using UnityEngine;

namespace Trainworks.Utilities
{
    public class AssetLoadingInfo
    {
        /// <summary>
        /// Path relative to your plugin
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// This will get set for you. Don't set it manually.
        /// </summary>
        public string PluginPath { get; set; }

        /// <summary>
        /// The asset type to load
        /// </summary>
        public AssetRefBuilder.AssetTypeEnum AssetType { get; set; }

        /// <summary>
        /// Concatenates plugin path and bundle path
        /// </summary>
        public string FullPath
        {
            get
            {
                return Path.Combine(PluginPath, FilePath);
            }
        }
    }

    /// <summary>
    /// Helper class to aid in loading asset bundles.
    /// </summary>
    public class BundleAssetLoadingInfo : AssetLoadingInfo
    {
        public ISettings ImportSettings { get; set; }

        /// <summary>
        /// Base name of the asset. Defaults to the SpriteName's filename without extension.
        /// This should be set for easier debugging with the Runtime Unity editor
        /// </summary>
        public string BaseName { get; set; }

        /// <summary>
        /// Path to the asset sprite in the bundle.
        /// Even if you're making a spine character, you *must* set this for the preview sprite.
        /// </summary>
        public string SpriteName { get; set; }

        /// <summary>
        /// Optional path to a singular spine skeleton data in the bundle.
        /// Use if all of your Spine Animations is in a single SkeletonAnimation.
        /// 
        /// This should be used if used as a ClassSelectCharacterDisplay Character.
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Optional paths mapping the Animation to the Spine Skeleton Data in the bundle.
        /// This is preferred over ObjectName as you can specify all 6 SkeletonAnimations for your character with this property.
        /// </summary>
        public IDictionary<CharacterUI.Anim, string> SpineAnimationDict { get; set; }

        /// <summary>
        /// For Custom Spine Objects, adjust the position of the animation. May be needed if the animation needs adjust from its center.
        /// </summary>
        public Vector3? OverridePosition { get; set; }
        /// <summary>
        /// For Custom Spine Objects, adjust the position of the animation. May be needed if the animation is higher resolution.
        /// </summary>
        public Vector3? OverrideScale { get; set; }
        /// <summary>
        /// For Custom Spine Objects, adjust the position of the animation. May be needed if the animation needs adjust from its center.
        /// </summary>
        public Vector3? OverrideCharacterSelectPosition { get; set; }
        /// <summary>
        /// For Custom Spine Objects, adjust the position of the animation. May be needed if the animation is higher resolution.
        /// </summary>
        public Vector3? OverrideCharacterSelectScale { get; set; }
    }
}
