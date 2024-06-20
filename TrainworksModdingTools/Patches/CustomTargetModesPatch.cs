using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Trainworks.Enums;
using Trainworks.Custom;
using static TargetHelper;
using static UnityEngine.GraphicsBuffer;

namespace Trainworks.Patches
{
    /// <summary>
    /// Main patch implementing Custom Target Modes and the Invert flag.
    /// </summary>
    [HarmonyPatch()]
    public class CustomCharacterTargetModesPatch
    {
        private static readonly List<CharacterState> temp = new List<CharacterState>();

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(TargetHelper), nameof(TargetHelper.CollectTargets), new Type[] { typeof(TargetHelper.CollectTargetsData), typeof(List<CharacterState>).MakeByRefType() });
        }

        // The idea is to run the original with the Original Target Mode (non inverted) and the Postfix patch does the inversion.
        public static void Prefix(ref TargetHelper.CollectTargetsData data, ref bool __state)
        {
            // state == invert.
            __state = TargetModeType.IsInverted(data.targetMode) && (TargetModeType.GetOriginalTargetMode(data.targetMode) != TargetMode.DropTargetCharacter);
            data.targetMode = TargetModeType.GetOriginalTargetMode(data.targetMode);
        }

        public static void Postfix(TargetHelper.CollectTargetsData data, ref List<CharacterState> targets, List<CharacterState> ___allTargets, ref List<CharacterState> ___lastTargetedCharacters, ref bool __state)
        {
            bool flag = data.firstEffectInPlayedCard.HasValue && data.firstEffectInPlayedCard.Value;
            bool handled = false;

            // Shouldn't happen
            if (targets == null)
                return;

            handled = TargetModeType.CharacterTargetModeHandlers.TryGetValue(data.targetMode, out Action<TargetHelper.CollectTargetsData, List<CharacterState>, List<CharacterState>> func);
            if (handled)
            {
                func.Invoke(data, ___allTargets, targets);
            }

            // Need to invert.
            if (__state)
            {
                temp.Clear();
                if (data.targetTeamType.HasFlag(Team.Type.Heroes))
                {
                    data.heroManager.AddCharactersInRoomToList(temp, data.roomIndex);
                }
                if (data.targetTeamType.HasFlag(Team.Type.Monsters))
                {
                    data.monsterManager.AddCharactersInRoomToList(temp, data.roomIndex);
                }

                foreach (var target in targets)
                {
                    temp.Remove(target);
                }

                targets.Clear();
                targets.AddRange(temp);
            }


            if (flag && (handled || __state))
            {
                ___lastTargetedCharacters.Clear();
                ___lastTargetedCharacters.AddRange(targets);
            }
        }
    }

    /// <summary>
    /// Patch to enable the Card Version of the Target Mode patch to work along with implementations of Hand, PlayedCard, LastDrawnCard, and their inverted forms.
    /// </summary>
    [HarmonyPatch()]
    class CustomCardTragetModesPatch
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(TargetHelper), nameof(TargetHelper.CollectTargets), new Type[] { typeof(CardEffectState), typeof(CardEffectParams), typeof(bool) } );
        }

        public static void Postfix(CardEffectState effectState, CardEffectParams cardEffectParams, bool isTesting, ref List<CharacterState> ___lastTargetedCharacters)
        {
            var targetMode = effectState.GetTargetMode();

            // Have to handle these Card Target Modes here.
            if (targetMode == TargetModes.PlayedCard)
            {
                cardEffectParams.targetCards.Clear();
                cardEffectParams.targetCards.Add(cardEffectParams.playedCard);
            }
            else if (targetMode == TargetModes.PlayedCard.Invert())
            {
                cardEffectParams.targetCards.Clear();
                cardEffectParams.targetCards.AddRange(cardEffectParams.cardManager.GetHand());
                cardEffectParams.targetCards.Remove(cardEffectParams.playedCard);   
            }
            else if (targetMode == TargetMode.LastDrawnCard.Invert())
            {
                cardEffectParams.targetCards.Clear();
                cardEffectParams.targetCards.AddRange(cardEffectParams.cardManager.GetHand());
                cardEffectParams.targetCards.Remove(cardEffectParams.cardManager.GetLastDrawnCard());
            }
            else if (targetMode == TargetMode.Hand)
            {
                cardEffectParams.targetCards.Clear();
                cardEffectParams.targetCards.AddRange(cardEffectParams.cardManager.GetHand());
            }
            else if (TargetModeType.CardTargetModeHandlers.ContainsKey(targetMode))
            {
                // ugh have to go through this again.
                bool? firstEffectInPlayedCard = null;
                bool isLimitedRangeCard = false;
                if (cardEffectParams.playedCard != null)
                {
                    if (effectState != null)
                    {
                        List<CardEffectState> effectStates = cardEffectParams.playedCard.GetEffectStates();
                        if (effectStates != null && effectStates.Count > 0)
                        {
                            firstEffectInPlayedCard = effectStates.IndexOf(effectState) == 0;
                            if (effectStates[0].GetCardEffect() is CardEffectNULL && ((CardEffectNULL)effectStates[0].GetCardEffect()).ShouldIgnoreLastTarget())
                            {
                                firstEffectInPlayedCard = effectStates.IndexOf(effectState) == 1;
                            }
                        }
                    }
                    isLimitedRangeCard = cardEffectParams.playedCard.HasTrait(typeof(CardTraitLimitedRange));
                }
                CollectTargetsData collectTargetsData = default(CollectTargetsData);
                collectTargetsData.targetMode = effectState.GetTargetMode();
                collectTargetsData.targetModeStatusEffectsFilter = effectState.GetTargetModeStatusEffectsFilter();
                collectTargetsData.targetModeHealthFilter = effectState.GetTargetModeHealthFilter();
                collectTargetsData.targetIgnoreBosses = effectState.GetTargetIgnoreBosses();
                collectTargetsData.targetIgnorePyre = effectState.GetParentCardState() != null;
                collectTargetsData.targetTeamType = effectState.GetTargetTeamType();
                collectTargetsData.targetSubtype = effectState.GetTargetCharacterSubtype();
                collectTargetsData.roomIndex = cardEffectParams.selectedRoom;
                collectTargetsData.statusId = effectState.GetParamStr();
                collectTargetsData.selfTarget = cardEffectParams.selfTarget;
                collectTargetsData.heroManager = cardEffectParams.heroManager;
                collectTargetsData.monsterManager = cardEffectParams.monsterManager;
                collectTargetsData.roomManager = cardEffectParams.roomManager;
                collectTargetsData.cardManager = cardEffectParams.cardManager;
                collectTargetsData.combatManager = cardEffectParams.combatManager;
                collectTargetsData.inCombat = false;
                collectTargetsData.ignoreDead = cardEffectParams.ignoreDeadInTargeting;
                collectTargetsData.firstEffectInPlayedCard = firstEffectInPlayedCard;
                collectTargetsData.isLimitedRangeCard = isLimitedRangeCard;
                collectTargetsData.overrideTargetCharacter = cardEffectParams.overrideTargetCharacter;
                collectTargetsData.isTesting = isTesting;
                CollectTargetsData data = collectTargetsData;

                TargetHelper.CollectCardTargets(data, ref cardEffectParams.targetCards);
            }
        }
    }


    /// <summary>
    /// Patch for the Card target selection version.
    /// </summary>
    [HarmonyPatch(typeof(TargetHelper), nameof(TargetHelper.CollectCardTargets))]
    public class CustomCardTargetModesPatch2
    {
        public static void Postfix(TargetHelper.CollectTargetsData data, ref List<CardState> targetCards)
        {
            if (targetCards == null)
                return;

            if (TargetModeType.CardTargetModeHandlers.TryGetValue(data.targetMode, out Action<TargetHelper.CollectTargetsData, List<CardState>> func))
            {
                func.Invoke(data, targetCards);
            }
        }
    }

    /// <summary>
    /// Patch to get DropTargetCharacter.Invert() working.
    /// </summary>
    [HarmonyPatch(typeof(TargetHelper), nameof(TargetHelper.CheckTargetsOverride))]
    class FixInvertDropTargetCharacterPatch
    {
        public static void Postfix(CardEffectState effectState, ref List<CharacterState> targets, SpawnPoint dropLocation, ref List<CharacterState> ___lastTargetedCharacters)
        {
            if (effectState.GetTargetMode() == TargetModeType.MakeInverted(TargetMode.DropTargetCharacter) && dropLocation != null)
            {
                CharacterState characterState = dropLocation.GetCharacterState();
                if (characterState != null)
                {
                    // Targets here should be the full list of valid targets, as gathered in CollectTargets.
                    targets.Remove(characterState);
                    ___lastTargetedCharacters.Clear();
                    ___lastTargetedCharacters.AddRange(targets);
                }
                else
                {
                    targets.Clear();
                }
            }
        }
    }

    /// <summary>
    /// Patch to enable the above patch to run.
    /// </summary>
    [HarmonyPatch(typeof(TargetHelper), nameof(TargetHelper.IsCardDropTargetRoom))]
    class FixInvertDropTargetCharacterPatch2
    {
        public static void Postfix(CardState card, ref bool __result)
        {
            // Only need to handle DropTargetCharacter Inverted. Self is already handled in CollectTargets
            if (card != null && card.DoesAnyEffectTarget(TargetMode.DropTargetCharacter.Invert()))
            {
                __result = false;
            }
        }
    }
}
