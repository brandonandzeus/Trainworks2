using HarmonyLib;
using Malee;
using ShinyShoe;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Managers;
using Trainworks.Utilities;
using UnityEngine;

/// Adopted from ThreeFishes' Equestrian Clan.
namespace Trainworks.BuildersV2
{
    /// <summary>
    ///	Builder for StoryEventData objects. This defines the metadata for cavern events.
    ///	
    /// Note that this class isn't enough to build a cavern event.
    /// The text and choices in a cavern event are handled by Ink.
    /// 
    /// Inky editor for ink: https://github.com/inkle/inky/releases/tag/0.11.0
    /// </summary>
    public class StoryEventDataBuilder
    {
        private string storyID;

        public string StoryID
        {
            get
            {
                return storyID;
            }
            set
            {
                storyID = value;
                KnotName = value;
            }
        }
        /// <summary>
        /// If true add to the cavern event pool.
        /// You should set to false if this story is part of a followup event.
        /// (Defaults to true).
        /// </summary>
        public bool AddToCavernEventPool { get; set; }
        /// <summary>
        /// Knot name for the story. This corresponds with the Ink Story's Knot Name.
        /// Shouldn't need to set this as its automatically set to StoryID.
        /// </summary>
        public string KnotName { get; set; }
        /// <summary>
        /// For now, this defaults to GimmeGold.
        /// </summary>
        public StoryEventAssetFrame EventPrefab { get; set; }
        /// <summary>
        /// Followup Events that can occur after this StoryEvent.
        /// </summary>
        public List<FollowupEventData> FollowupEvents { get; set; }
        /// <summary>
        /// Convience Builder for FollowupEvents, will be appeneded to FollowupEvents if set.
        /// </summary>
        public List<FollowupEventDataBuilder> FollowupEventBuilders { get; set; }
        /// <summary>
        /// Number of runs completed before the event is able to be seen.
        /// </summary>
        public int NumRunsCompletedToSee { get; set; }
        /// <summary>
        /// Ranges from 0 to 100 with higher numbers appearing less frequently.
        /// </summary>
        public int PriorityTicketCount { get; set; }
        /// <summary>
        /// Number of unlocked clans needed for this event to be available.
        /// </summary>
        public int NumClassesNeededToShow { get; set; }
        /// <summary>
        /// Minimum Covenant level required to unlock the event.
        /// </summary>
        public int CovenantLevelRequired { get; set; }
        /// <summary>
        /// All clans needed to show the event
        /// </summary>
        public bool AllClassesNeededToShow { get; set; }
        /// <summary>
        /// The earliest the event can be seen.
        /// Defined as number of battles with (0 = limbo, 7 = seraph)
        /// Note that there are no Cavern events in the first two rings.
        /// </summary>
        public int MinDistanceAllowed { get; set; }
        /// <summary>
        /// The latest the event can be seen.
        /// Defined as number of battles with (0 = limbo, 7 = seraph)
        /// Note that there are no Cavern events in the first two rings.
        /// </summary>
        public int MaxDistanceAllowed { get; set; }
        /// <summary>
        /// If a certain mutator is present the event is excluded from showing up.
        /// </summary>
        public MutatorData ExcludedMutator { get; set; }
        /// <summary>
        /// DLC requirement.
        /// </summary>
        public DLC AssociatedDLC { get; set; }
        /// <summary>
        /// Require the DLC to be active.
        /// </summary>
        public bool RequireDlcModeActive { get; set; }
        /// <summary>
        /// Number of pact shards required.
        /// Set to 0 to skip the check.
        /// </summary>
        public int NumCrystalsRequired { get; set; }
        /// <summary>
        /// Prevent events from showing up if this one was taken.
        /// For instance if you get the Malicka Event in ring 3
        /// Then the Malicka event at the start of ring 4 (with the negative artifacts) won't appear.
        /// </summary>
        public List<StoryEventData> ExcludingEventData { get; set; }
        /// <summary>
        /// Possible Rewards
        /// </summary>
        public List<RewardData> PossibleRewards { get; set; }
        /// <summary>
        /// Convience Builder for Possible Rewards will be appeneded to PossibleRewards when built.
        /// </summary>
        public List<IRewardDataBuilder> PossibleRewardBuilders { get; set; }

        public StoryEventDataBuilder()
        {
            EventPrefab = ProviderManager.SaveManager.GetAllGameData().FindStoryEventDataByName("GimmeGold").EventPrefab;
            FollowupEvents = new List<FollowupEventData>();
            FollowupEventBuilders = new List<FollowupEventDataBuilder>();
            NumRunsCompletedToSee = 1;
            PriorityTicketCount = 1;
            NumClassesNeededToShow = 1;
            ExcludingEventData = new List<StoryEventData>();
            PossibleRewards = new List<RewardData>();
            PossibleRewardBuilders = new List<IRewardDataBuilder>();
            AddToCavernEventPool = true;
        }

        public StoryEventData Build()
        {
            StoryEventData newStory = ScriptableObject.CreateInstance<StoryEventData>();
            newStory.name = StoryID;

            AccessTools.Field(typeof(StoryEventData), "id").SetValue(newStory, GUIDGenerator.GenerateDeterministicGUID(StoryID));

            AccessTools.Field(typeof(StoryEventData), "associatedDLC").SetValue(newStory, AssociatedDLC);
            AccessTools.Field(typeof(StoryEventData), "storyId").SetValue(newStory, StoryID);
            AccessTools.Field(typeof(StoryEventData), "knotName").SetValue(newStory, KnotName);
            AccessTools.Field(typeof(StoryEventData), "eventPrefab").SetValue(newStory, EventPrefab);
            AccessTools.Field(typeof(StoryEventData), "numRunsCompletedToSee").SetValue(newStory, NumRunsCompletedToSee);
            AccessTools.Field(typeof(StoryEventData), "priorityTicketCount").SetValue(newStory, PriorityTicketCount);
            AccessTools.Field(typeof(StoryEventData), "numClassesNeededToShow").SetValue(newStory, NumClassesNeededToShow);
            AccessTools.Field(typeof(StoryEventData), "covenantLevelRequired").SetValue(newStory, CovenantLevelRequired);
            AccessTools.Field(typeof(StoryEventData), "allClassesNeededToShow").SetValue(newStory, AllClassesNeededToShow);
            AccessTools.Field(typeof(StoryEventData), "minDistanceAllowed").SetValue(newStory, MinDistanceAllowed);
            AccessTools.Field(typeof(StoryEventData), "maxDistanceAllowed").SetValue(newStory, MaxDistanceAllowed);
            AccessTools.Field(typeof(StoryEventData), "excludedMutator").SetValue(newStory, ExcludedMutator);
            AccessTools.Field(typeof(StoryEventData), "requireDlcModeActive").SetValue(newStory, RequireDlcModeActive);
            AccessTools.Field(typeof(StoryEventData), "numCrystalsRequired").SetValue(newStory, NumCrystalsRequired);
            AccessTools.Field(typeof(StoryEventData), "excludingEventData").SetValue(newStory, ExcludingEventData);

            var followupEvents = new List<FollowupEventData>();
            followupEvents.AddRange(FollowupEvents);
            foreach (var evt in FollowupEventBuilders)
                followupEvents.Add(evt.Build());
            AccessTools.Field(typeof(StoryEventData), "followupEvents").SetValue(newStory, followupEvents);

            var possibleRewards = new List<RewardData>();
            possibleRewards.AddRange(PossibleRewards);
            foreach (var reward in PossibleRewardBuilders)
                possibleRewards.Add(reward.Build());

            AccessTools.Field(typeof(StoryEventData), "possibleRewards").SetValue(newStory, possibleRewards);

            return newStory;
        }

        public StoryEventData BuildAndRegister()
        {
            StoryEventData newStory = Build();

            CustomStoryManager.RegisterCustomStory(newStory, AddToCavernEventPool);

            return newStory;
        }
    }
}