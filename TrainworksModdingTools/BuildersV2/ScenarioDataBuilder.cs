﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Trainworks.ConstantsV2;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;

namespace Trainworks.BuildersV2
{
    public class ScenarioDataBuilder
    {
        private String scenarioID;

        /// <summary>
        /// Required Scenario ID.
        /// </summary>
        public String ScenarioID
        {
            get
            {
                return scenarioID;
            }
            set
            {
                scenarioID = value;
                BattleNameKey = scenarioID + "_ScenarioData_BattleNameKey";
                BattleDescriptionKey = scenarioID + "_ScenarioData_BattleDescriptionKey";
            }
        }

        /// <summary>
        /// Required. Distance where the Scenario appears.
        /// Must be a number 0-8 where 0 is the firet battle.
        /// and 8 is the divnity fight.
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// Localization key for the Battle's name.
        /// Note that setting ScenarioID sets this field automatically.
        /// </summary>
        public String BattleNameKey { get; set; }
        /// <summary>
        /// Name for this Scenario.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public String BattleName { get; set; }
        /// <summary>
        /// Localization key for the battle's description.
        /// Note that setting ScenarioID sets this field automatically.
        /// </summary>
        public String BattleDescriptionKey { get; set; }
        /// <summary>
        /// Description for this Scenario.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public String BattleDescription { get; set; }

        /// <summary>
        /// Defines what eneemies are spawned per wave.
        /// </summary>
        public SpawnPatternData SpawnPattern { get; set; }
        /// <summary>
        /// Convienence Builder for SpawnPattern. if set overrides SpawnPattern
        /// </summary>
        public SpawnPatternDataBuilder SpawnPatternBuilder { get; set; }
        /// <summary>
        /// List of Boss characters to use.
        /// </summary>
        public List<CharacterData> BossVariants { get; set; }
        /// <summary>
        /// Energy (Ember) to give the player at the start of the scenario.
        /// </summary>
        public int StartingEnergy { get; set; }
        /// <summary>
        /// Scenario Difficulty.
        /// </summary>
        public ScenarioDifficulty Difficulty { get; set; }
        public string BossIcon { get; set; }
        public string BossPortrait { get; set; }
        /// <summary>
        /// Custom Map Node for display on the map.
        /// Seems to be used by Major Bosses (but not divinity).
        /// </summary>
        public GameObject MapNodePrefab { get; set; }
        /// <summary>
        /// Beneficial effects given to enemies in this scenario.
        /// These should technically be a SinsData, but the MT Codebase
        /// allows specifying any type of relic here.
        /// </summary>
        public List<RelicData> EnemyBlessingData { get; set; }
        public string BattleTrackNameData { get; set; }
        /// <summary>
        /// Minimum number of Treasure Units to spawn (Collector).
        /// </summary>
        public int MinTreasureUnits { get; set; }
        /// <summary>
        /// Maximum number of Treasure Units to spawn (Collector).
        /// </summary>
        public int MaxTreasureUnits { get; set; }
        /// <summary>
        /// Pool of Characters to use as a Treasure Unit.
        /// </summary>
        public List<CharacterData> TreasureCharacterPool { get; set; }
        /// <summary>
        /// Unused parameter by the MT Codebase, left here for futureproofing.
        /// </summary>
        public int MinTraitorUnits { get; set; }
        /// <summary>
        /// Unused parameter by the MT Codebase, left here for futureproofing.
        /// </summary>
        public int MaxTraitorUnits { get; set; }
        /// <summary>
        /// Unused parameter by the MT Codebase, left here for futureproofing.
        /// </summary>
        public List<CharacterData> TraitorUnitsPool { get; set; }
        /// <summary>
        /// Trials to pick from for this Scenario.
        /// </summary>
        public List<TrialData> Trials { get; set; }
        /// <summary>
        /// First Time User Experience Cards to draw if this is a FTUE scenario.
        /// </summary>
        public List<CardData> FTUECardDrawData { get; set; }
        /// <summary>
        /// Enemies to display on the Battle Intro Screen.
        /// </summary>
        public List<CharacterData> DisplayedEnemies { get; set; }
        /// <summary>
        /// Customizable offsets for the enemies in the Battle Intro Screen.
        /// </summary>
        public List<Vector2> DisplayedEnemyOffsets { get; set; }
        /// <summary>
        /// Boss Spawn SFX cue.
        /// </summary>
        public String BossSpawnSFXCue { get; set; }
        /// <summary>
        /// Background to use for the Scenario.
        /// </summary>
        public BackgroundData BackgroundData { get; set; }
        /// <summary>
        /// Set automatically in the constructor. Base asset path, usually the plugin directory.
        /// </summary>
        public string BaseAssetPath { get; set; }

        public ScenarioDataBuilder()
        {
            BossVariants = new List<CharacterData>();
            EnemyBlessingData = new List<RelicData>();
            TreasureCharacterPool = new List<CharacterData>();
            TraitorUnitsPool = new List<CharacterData>();
            Trials = new List<TrialData>();
            FTUECardDrawData = new List<CardData>();
            DisplayedEnemies = new List<CharacterData>();
            DisplayedEnemyOffsets = new List<Vector2>();
            Distance = -1;

            var assembly = Assembly.GetCallingAssembly();
            BaseAssetPath = PluginManager.PluginGUIDToPath[PluginManager.AssemblyNameToPluginGUID[assembly.FullName]];
        }

        public ScenarioData Build()
        {
            if (ScenarioID == null)
            {
                throw new BuilderException("ScenarioID is required.");
            }
            if (Distance < 0 || Distance > 8)
            {
                throw new BuilderException("Distance is required and must be non negative and less than 9");
            }
            ScenarioData scenarioData = ScriptableObject.CreateInstance<ScenarioData>();
            scenarioData.name = ScenarioID;

            AccessTools.Field(typeof(ScenarioData), "id").SetValue(scenarioData, GUIDGenerator.GenerateDeterministicGUID(ScenarioID));


            var spawnPattern = SpawnPattern;
            if (SpawnPatternBuilder != null)
                spawnPattern = SpawnPatternBuilder.Build();

            AccessTools.Field(typeof(ScenarioData), "spawnPattern").SetValue(scenarioData, spawnPattern);
            AccessTools.Field(typeof(ScenarioData), "startingEnergy").SetValue(scenarioData, StartingEnergy);
            AccessTools.Field(typeof(ScenarioData), "difficulty").SetValue(scenarioData, Difficulty);
            AccessTools.Field(typeof(ScenarioData), "enemyBlessingData").SetValue(scenarioData, EnemyBlessingData.ToArray());
            AccessTools.Field(typeof(ScenarioData), "battleTrackNameData").SetValue(scenarioData, BattleTrackNameData);
            AccessTools.Field(typeof(ScenarioData), "minTreasureUnits").SetValue(scenarioData, MinTreasureUnits);
            AccessTools.Field(typeof(ScenarioData), "maxTreasureUnits").SetValue(scenarioData, MaxTreasureUnits);
            AccessTools.Field(typeof(ScenarioData), "treasureCharacterPool").SetValue(scenarioData, TreasureCharacterPool.ToArray());
            AccessTools.Field(typeof(ScenarioData), "minTraitorUnits").SetValue(scenarioData, MinTraitorUnits);
            AccessTools.Field(typeof(ScenarioData), "maxTraitorUnits").SetValue(scenarioData, MaxTraitorUnits);
            AccessTools.Field(typeof(ScenarioData), "traitorUnitsPool").SetValue(scenarioData, TraitorUnitsPool.ToArray());
            AccessTools.Field(typeof(ScenarioData), "battleNameKey").SetValue(scenarioData, BattleNameKey);
            AccessTools.Field(typeof(ScenarioData), "battleDescriptionKey").SetValue(scenarioData, BattleDescriptionKey);
            AccessTools.Field(typeof(ScenarioData), "displayedEnemies").SetValue(scenarioData, DisplayedEnemies);
            AccessTools.Field(typeof(ScenarioData), "displayedEnemyOffsets").SetValue(scenarioData, DisplayedEnemyOffsets);
            AccessTools.Field(typeof(ScenarioData), "bossSpawnSFXCue").SetValue(scenarioData, BossSpawnSFXCue);
            AccessTools.Field(typeof(ScenarioData), "backgroundData").SetValue(scenarioData, BackgroundData);

            if (BossIcon != null)
            {
                Sprite iconSprite = CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + BossIcon);
                AccessTools.Field(typeof(ScenarioData), "bossIcon").SetValue(scenarioData, iconSprite);
            }

            if (BossPortrait != null)
            {
                Sprite iconSprite = CustomAssetManager.LoadSpriteFromPath(BaseAssetPath + "/" + BossIcon);
                AccessTools.Field(typeof(ScenarioData), "bossPortrait").SetValue(scenarioData, iconSprite);
            }

            // Trial Data setup
            TrialDataList trialDataList = ScriptableObject.CreateInstance<TrialDataList>();
            AccessTools.Field(typeof(TrialDataList), "listName").SetValue(trialDataList, ScenarioID + "Trials");
            AccessTools.Field(typeof(TrialDataList), "trialDatas").SetValue(trialDataList, Trials.ToArray());
            AccessTools.Field(typeof(ScenarioData), "trials").SetValue(scenarioData, trialDataList);

            // FTUE card draw data.
            ScenarioData.FtueCardDrawDataList ftueCardDraws = new ScenarioData.FtueCardDrawDataList();
            ftueCardDraws.CopyFrom(FTUECardDrawData);
            AccessTools.Field(typeof(ScenarioData), "ftueCardDrawData").SetValue(scenarioData, ftueCardDraws);

            BossVariantSpawnData bossVariants = ScriptableObject.CreateInstance<BossVariantSpawnData>();
            Malee.ReorderableArray<CharacterData> list = (Malee.ReorderableArray<CharacterData>)AccessTools.Field(typeof(BossVariantSpawnData), "bossVariants").GetValue(bossVariants);
            list.Clear();
            list.CopyFrom(BossVariants);
            AccessTools.Field(typeof(ScenarioData), "bossVariant").SetValue(scenarioData, bossVariants);

            BuilderUtils.ImportStandardLocalization(BattleNameKey, BattleName);
            BuilderUtils.ImportStandardLocalization(BattleDescriptionKey, BattleDescription);

            return scenarioData;
        }

        public ScenarioData BuildAndRegister()
        {
            ScenarioData data = Build();
            CustomScenarioManager.RegisterCustomScenario(data, Distance);
            return data;
        }
    }
}
