using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Trainworks.BuildersV2
{
    public class CharacterDataBuilder
    {
        private string characterID;

        /// <summary>
        /// Unique string used to store and retrieve the character data.
        /// Implictly sets NameKey if null.
        /// </summary>
        public string CharacterID
        {
            get { return characterID; }
            set
            {
                characterID = value;
                if (NameKey == null)
                {
                    NameKey = characterID + "_CharacterData_NameKey";
                }
            }
        }
        /// <summary>
        /// Name displayed for the character.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Localization key for the character's name.
        /// Note that setting CharacterID sets this field to [CharacterID]_CharacterData_NameKey so you shouldn't need to set.
        /// </summary>
        public string NameKey { get; set; }
        /// <summary>
        /// The character's attack stat.
        /// </summary>
        public int AttackDamage { get; set; }
        /// <summary>
        /// The character's health stat.
        /// </summary>
        public int Health { get; set; }
        /// <summary>
        /// The amount of capacity the character uses up on the floor.
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Character triggers.
        /// </summary>
        public List<CharacterTriggerData> Triggers { get; set; }
        /// <summary>
        /// Convenience Builders for Triggers. Will be appeneded to Triggers.
        /// </summary>
        public List<CharacterTriggerDataBuilder> TriggerBuilders { get; set; }
        /// <summary>
        /// Status effects the character starts with when spawned on a floor.
        /// </summary>
        public List<StatusEffectStackData> StartingStatusEffects { get; set; }
        /// <summary>
        /// Status Effects this Character is immune to.
        /// </summary>
        public string[] StatusEffectImmunities { get; set; }
        /// <summary>
        /// The full, absolute path to the asset. Concatenates BaseAssetPath and AssetPath.
        /// </summary>
        public string FullAssetPath => BaseAssetPath + "/" + AssetPath;
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }
        /// <summary>
        /// Custom asset path to load from. Must be inside the BaseAssetPath.
        /// </summary>
        public string AssetPath { get; set; }
        /// <summary>
        /// Loading Info for loading a character's art
        /// </summary>
        public BundleAssetLoadingInfo BundleLoadingInfo { get; set; }
        /// <summary>
        /// Use an existing base game character's art by filling this in with the appropriate character's asset reference information.
        /// </summary>
        public AssetReferenceGameObject CharacterPrefabVariantRef { get; set; }
        /// <summary>
        /// Convenience builder for CharacterPrefabVariantRef.
        /// </summary>
        public Builders.AssetRefBuilder CharacterPrefabVariantRefBuilder { get; set; }
        /// <summary>
        /// Whether or not the character is able to attack.
        /// </summary>
        public bool CanAttack { get; set; }
        /// <summary>
        /// Whether or not the character is able to be healed.
        /// </summary>
        public bool CanBeHealed { get; set; }
        /// <summary>
        /// Whether or not the character is a miniboss (a non-flying boss).
        /// </summary>
        public bool IsMiniboss { get; set; }
        /// <summary>
        /// Whether or not the character is a flying boss.
        /// </summary>
        public bool IsOuterTrainBoss { get; set; }
        /// <summary>
        /// Death Slides Backwards. Defaults to true.
        /// Many units that spawn units on death or explode have this set to false.
        /// </summary>
        public bool DeathSlidesBackwards { get; set; }
        /// <summary>
        /// Only used for Flying Boss Characters controls their actions and how often its repeated.
        /// Note: No Builder currently exists for ActionGroupData so reflection has to be used.
        /// </summary>
        public List<ActionGroupData> BossActionGroups { get; set; }
        /// <summary>
        /// Lore tooltip keys. Note that these are localization keys so you will need to Add localization data.
        /// </summary>
        public List<string> CharacterLoreTooltipKeys { get; set; }
        /// <summary>
        /// Convenience Builders for RoomModifiers. Will be appeneded to RoomModifiers.
        /// </summary>
        public List<RoomModifierDataBuilder> RoomModifierBuilders { get; set; }
        /// <summary>
        /// RoomModifiers that the Character applies.
        /// </summary>
        public List<RoomModifierData> RoomModifiers { get; set; }
        /// <summary>
        /// Defaults to true, Set to false to prevent enemies from ascending the train.
        /// The uses of this are Characters such as the Collector, and Archus's darkshards.
        /// </summary>
        public bool AscendsTrainAutomatically { get; set; }
        /// <summary>
        /// When attacking, this character moves next to its target before hitting it.
        /// </summary>
        public bool AttackTeleportsToDefender { get; set; }
        /// <summary>
        /// List of Subtypes for the character.
        /// </summary>
        public List<string> SubtypeKeys { get; set; }
        /// <summary>
        /// Sound effects for the Character.
        /// Note no Builder currently exists for this object. So reflection must be used to create a CharacterSoundData.
        /// </summary>
        public CharacterSoundData CharacterSoundData { get; set; }
        /// <summary>
        /// Character Chatter Data.
        /// </summary>
        public CharacterChatterData CharacterChatterData { get; set; }
        /// <summary>
        /// Builder for CharacterChatterData if set overrides CharacterChatterData.
        /// </summary>
        public CharacterChatterDataBuilder CharacterChatterDataBuilder { get; set; }
        public RuntimeAnimatorController AnimationController { get; set; }
        public CharacterDeathVFX.Type DeathType { get; set; }
        public VfxAtLoc DeathVFX { get; set; }
        /// <summary>
        /// The VFX to put on the target when attacking to override CombatManager CombatReactVFXPrefab
        /// </summary>
        public VfxAtLoc ImpactVFX { get; set; }
        /// <summary>
        /// The projectile to fire when attacking - base game comment
        /// </summary>
        public VfxAtLoc ProjectilePrefab { get; set; }
        public VfxAtLoc BossRoomSpellCastVFX { get; set; }
        public VfxAtLoc BossSpellCastVFX { get; set; }
        /// <summary>
        /// A cache for the character's sprite so it doesn't have to be reloaded repeatedly.
        /// </summary>
        public Sprite CharacterSpriteCache { get; set; }
        /// <summary>
        /// Sets whether or not this unit should be considered for being automatically drawn as part of the priority unit draw system for hands.
        /// This sets the appropriate Subtype (Chosen) for the character.
        /// </summary>
        public bool PriorityDraw { get; set; }
        /// <summary>
        /// Holds the builder for a unit's synthesis ability.
        /// </summary>
        public CardUpgradeDataBuilder UnitSynthesisBuilder { get; set; }
        public bool BlockVisualSizeIncrease { get; set; }
        public CharacterData.ReorderableCharacterShardUpgradeList BypassPactCrystalsUpgradeDataList { get; set; }
        public int PactCrystalsRequiredCount { get; set; }
        public CharacterData PactCrystalsVariantData { get; set; }
        public bool RemoveTriggersOnRelentlessChange { get; set; }
        public BossState.AttackPhase ValidBossAttackPhase { get; set; }

        public CharacterDataBuilder()
        {
            AttackDamage = 10;
            Size = 2;
            AttackTeleportsToDefender = true;
            CanAttack = true;
            CanBeHealed = true;
            AscendsTrainAutomatically = true;
            DeathSlidesBackwards = true;
            Triggers = new List<CharacterTriggerData>();
            SubtypeKeys = new List<string>();
            BossActionGroups = new List<ActionGroupData>();
            RoomModifiers = new List<RoomModifierData>();
            CharacterLoreTooltipKeys = new List<string>();
            StartingStatusEffects = new List<StatusEffectStackData>();
            StatusEffectImmunities = Array.Empty<string>();
            ImpactVFX = (VfxAtLoc)FormatterServices.GetUninitializedObject(typeof(VfxAtLoc));
            TriggerBuilders = new List<CharacterTriggerDataBuilder>();
            RoomModifierBuilders = new List<RoomModifierDataBuilder>();
            PriorityDraw = true;
            ValidBossAttackPhase = BossState.AttackPhase.Relentless;
            PactCrystalsRequiredCount = -1;
            CharacterChatterDataBuilder = new CharacterChatterDataBuilder();

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        /// <summary>
        /// Builds the CharacterData represented by this builder's parameters
        /// and registers it and its components with the appropriate managers.
        /// </summary>
        /// <returns>The newly registered CharacterData</returns>
        public CharacterData BuildAndRegister()
        {
            var characterData = Build();
            CustomCharacterManager.RegisterCustomCharacter(characterData);

            if (!characterData.IsChampion())
            {
                if (UnitSynthesisBuilder == null)
                {
                    BuildDummyUnitSynthesis(characterData);
                }
                else
                {
                    UnitSynthesisBuilder.SourceSynthesisUnit = characterData;
                    UnitSynthesisBuilder.BuildAndRegister();
                }
            }

            return characterData;
        }

        private static void BuildDummyUnitSynthesis(CharacterData characterData)
        {
            Trainworks.Log(BepInEx.Logging.LogLevel.Warning, "Building Dummy Unit Synthesis for Character: " + characterData.name);
            new CardUpgradeDataBuilder()
            {
                UpgradeID = $"Dummy_synth_{characterData.name}",
                UpgradeDescription = "<DUMMY>",
                SourceSynthesisUnit = characterData,
                BonusDamage = 1
            }.BuildAndRegister();
        }

        /// <summary>
        /// Builds the CharacterData represented by this builder's parameters
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created CharacterData</returns>
        public CharacterData Build()
        {
            if (characterID == null)
            {
                throw new BuilderException("CharacterID is required");
            }

            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
            characterData.name = CharacterID;

            // Saving allocations by reusing the allocated lists at initialization.
            var triggers = characterData.GetTriggers();
            triggers.AddRange(Triggers);
            foreach (var builder in TriggerBuilders)
            {
                triggers.Add(builder.Build());
            }

            var roomModifiers = characterData.GetRoomModifiersData();
            roomModifiers.AddRange(RoomModifiers);
            foreach (var builder in RoomModifierBuilders)
            {
                roomModifiers.Add(builder.Build());
            }

            // No Getter for subtype keys.
            var subtypeKeys = (List<string>)AccessTools.Field(typeof(CharacterData), "subtypeKeys").GetValue(characterData);
            subtypeKeys.AddRange(SubtypeKeys);
            if (PriorityDraw)
            {
                subtypeKeys.Add(VanillaSubtypeIDs.Chosen);
            }

            var guid = GUIDGenerator.GenerateDeterministicGUID(CharacterID);
            AccessTools.Field(typeof(CharacterData), "id").SetValue(characterData, guid);
            AccessTools.Field(typeof(CharacterData), "animationController").SetValue(characterData, AnimationController);
            AccessTools.Field(typeof(CharacterData), "ascendsTrainAutomatically").SetValue(characterData, AscendsTrainAutomatically);
            AccessTools.Field(typeof(CharacterData), "attackDamage").SetValue(characterData, AttackDamage);
            AccessTools.Field(typeof(CharacterData), "attackTeleportsToDefender").SetValue(characterData, AttackTeleportsToDefender);
            AccessTools.Field(typeof(CharacterData), "blockVisualSizeIncrease").SetValue(characterData, BlockVisualSizeIncrease);
            AccessTools.Field(typeof(CharacterData), "bossActionGroups").SetValue(characterData, BossActionGroups);
            AccessTools.Field(typeof(CharacterData), "bossRoomSpellCastVFX").SetValue(characterData, BossRoomSpellCastVFX);
            AccessTools.Field(typeof(CharacterData), "bossSpellCastVFX").SetValue(characterData, BossSpellCastVFX);
            AccessTools.Field(typeof(CharacterData), "bypassPactCrystalsUpgradeDataList").SetValue(characterData, BypassPactCrystalsUpgradeDataList);
            AccessTools.Field(typeof(CharacterData), "canAttack").SetValue(characterData, CanAttack);
            AccessTools.Field(typeof(CharacterData), "canBeHealed").SetValue(characterData, CanBeHealed);
            AccessTools.Field(typeof(CharacterData), "characterLoreTooltipKeys").SetValue(characterData, CharacterLoreTooltipKeys);
            AccessTools.Field(typeof(CharacterData), "characterSoundData").SetValue(characterData, CharacterSoundData);
            AccessTools.Field(typeof(CharacterData), "characterSpriteCache").SetValue(characterData, CharacterSpriteCache);
            AccessTools.Field(typeof(CharacterData), "deathSlidesBackwards").SetValue(characterData, DeathSlidesBackwards);
            AccessTools.Field(typeof(CharacterData), "deathType").SetValue(characterData, DeathType);
            AccessTools.Field(typeof(CharacterData), "deathVFX").SetValue(characterData, DeathVFX);
            AccessTools.Field(typeof(CharacterData), "health").SetValue(characterData, Health);
            AccessTools.Field(typeof(CharacterData), "impactVFX").SetValue(characterData, ImpactVFX);
            AccessTools.Field(typeof(CharacterData), "isMiniboss").SetValue(characterData, IsMiniboss);
            AccessTools.Field(typeof(CharacterData), "isOuterTrainBoss").SetValue(characterData, IsOuterTrainBoss);
            AccessTools.Field(typeof(CharacterData), "nameKey").SetValue(characterData, NameKey);
            AccessTools.Field(typeof(CharacterData), "pactCrystalsRequiredCount").SetValue(characterData, PactCrystalsRequiredCount);
            AccessTools.Field(typeof(CharacterData), "pactCrystalsVariantData").SetValue(characterData, PactCrystalsVariantData);
            AccessTools.Field(typeof(CharacterData), "projectilePrefab").SetValue(characterData, ProjectilePrefab);
            AccessTools.Field(typeof(CharacterData), "removeTriggersOnRelentlessChange").SetValue(characterData, RemoveTriggersOnRelentlessChange);
            AccessTools.Field(typeof(CharacterData), "size").SetValue(characterData, Size);
            AccessTools.Field(typeof(CharacterData), "startingStatusEffects").SetValue(characterData, StartingStatusEffects.ToArray());
            AccessTools.Field(typeof(CharacterData), "statusEffectImmunities").SetValue(characterData, StatusEffectImmunities);
            AccessTools.Field(typeof(CharacterData), "validBossAttackPhase").SetValue(characterData, ValidBossAttackPhase);

            CharacterChatterData chatterData = CharacterChatterData;
            if (CharacterChatterDataBuilder == null)
            {
                chatterData = CharacterChatterDataBuilder.Build();
            }
            AccessTools.Field(typeof(CharacterData), "characterChatterData").SetValue(characterData, chatterData);

            if (CharacterPrefabVariantRef == null)
            {
                if (CharacterPrefabVariantRefBuilder == null)
                {
                    if (BundleLoadingInfo != null)
                    {
                        BundleLoadingInfo.PluginPath = BaseAssetPath;
                        CharacterPrefabVariantRefBuilder = new Builders.AssetRefBuilder
                        {
                            AssetLoadingInfo = BundleLoadingInfo
                        };
                    }
                    else
                    {
                        var assetLoadingInfo = new AssetLoadingInfo()
                        {
                            FilePath = AssetPath,
                            PluginPath = BaseAssetPath,
                            AssetType = Builders.AssetRefBuilder.AssetTypeEnum.Character
                        };
                        CharacterPrefabVariantRefBuilder = new Builders.AssetRefBuilder
                        {
                            AssetLoadingInfo = assetLoadingInfo
                        };
                    }
                }
                CharacterPrefabVariantRef = CharacterPrefabVariantRefBuilder.BuildAndRegister();
            }
            AccessTools.Field(typeof(CharacterData), "characterPrefabVariantRef").SetValue(characterData, CharacterPrefabVariantRef);

            BuilderUtils.ImportStandardLocalization(NameKey, Name);
            return characterData;
        }

        /// <summary>
        /// Creates an asset reference to an existing game file.
        /// Primarily useful for reusing base game art.
        /// Cards with custom art should not use this method.
        /// </summary>
        /// <param name="m_debugName">The asset's debug name (usually the path to it)</param>
        /// <param name="m_AssetGUID">The asset's GUID</param>
        public void CreateAndSetCharacterArtPrefabVariantRef(string m_debugName, string m_AssetGUID)
        {
            var assetReferenceGameObject = new AssetReferenceGameObject();
            AccessTools.Field(typeof(AssetReferenceGameObject), "m_debugName")
                    .SetValue(assetReferenceGameObject, m_debugName);
            AccessTools.Field(typeof(AssetReferenceGameObject), "m_AssetGUID")
                .SetValue(assetReferenceGameObject, m_AssetGUID);
            CharacterPrefabVariantRef = assetReferenceGameObject;

            AssetPath = m_AssetGUID;
        }
    }
}