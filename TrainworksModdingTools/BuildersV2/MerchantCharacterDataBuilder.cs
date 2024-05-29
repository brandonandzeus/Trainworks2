using HarmonyLib;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;
using static MerchantCharacterData;

namespace Trainworks.BuildersV2
{
    public class MerchantCharacterDataBuilder
    {
        public string MerchantCharacterID { get; set; }

        /// <summary>
        /// Loading Info for Custom Skeleton Data Asset
        /// 
        /// Required to set ObjectName here.
        /// </summary>
        public BundleAssetLoadingInfo BundleLoadingInfo { get; set; }
        /// <summary>
        /// Reuse a prexisting MerchantCharacters Skeleton Animation.
        /// </summary>
        public SkeletonDataAsset CharacterAnimationAsset { get; set; }

        public bool ShowInHR { get; set; }
        public string LookAtThisAnimationName { get; set; }
        public float SFXTimeoutTime { get; set; }
        public List<MerchantChatter> Greetings { get; set; }
        public List<MerchantChatter> Thanks { get; set; }
        public List<MerchantChatter> Chatter { get; set; }
        public MerchantChatter Bothered { get; set; }
        public bool CanGiveBotherAchievement { get; set; }
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }

        public MerchantCharacterDataBuilder()
        {
            ShowInHR = true;
            LookAtThisAnimationName = "Look at this";
            SFXTimeoutTime = 3f;
            CanGiveBotherAchievement = false;

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        public MerchantCharacterData Build()
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (MerchantCharacterID == null)
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Warning should provide a MerchantCharacterID.");
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Stacktrace: " + Environment.StackTrace);
            }

            var merchant = ScriptableObject.CreateInstance<MerchantCharacterData>();

            var guid = GUIDGenerator.GenerateDeterministicGUID(MerchantCharacterID);
            AccessTools.Field(typeof(GameData), "id").SetValue(merchant, guid);
            merchant.name = MerchantCharacterID;

            var skeletonAsset = CharacterAnimationAsset;
            if (BundleLoadingInfo != null)
            {
                BundleLoadingInfo.PluginPath = BaseAssetPath;
                BundleManager.RegisterBundle(BundleLoadingInfo);
                var obj = BundleManager.LoadAssetFromBundle(BundleLoadingInfo, BundleLoadingInfo.ObjectName) as GameObject;
                skeletonAsset = obj.GetComponent<SkeletonAnimation>().skeletonDataAsset;
            }

            AccessTools.Field(typeof(MerchantCharacterData), "_characterAnimationAsset").SetValue(merchant, skeletonAsset);
            AccessTools.Field(typeof(MerchantCharacterData), "showInHR").SetValue(merchant, ShowInHR);
            AccessTools.Field(typeof(MerchantCharacterData), "lookAtThisAnimationName").SetValue(merchant, LookAtThisAnimationName);
            AccessTools.Field(typeof(MerchantCharacterData), "sfxTimeoutTime").SetValue(merchant, SFXTimeoutTime);
            merchant.greetings = Greetings;
            merchant.thanks = Thanks;
            merchant.chatter = Chatter;
            merchant.bothered = Bothered;
            merchant.canGiveBotherAchievement = CanGiveBotherAchievement;

            return merchant;
        }
    }
}
