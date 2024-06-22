using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trainworks.Custom.CardEffects
{
    /// <summary>
    /// A more generic version of CardEffectHealAndDamageRelative. The issues with the stock version is that it was very hardcoded.
    /// 1) In the TargetMode only the first character within the list of targets got the heal.
    /// 2) The CardEffect hardcodes the damage (5 * healed_amount) to go to the front unit of the opposite team.
    /// 
    /// This version aims to support more features.
    /// 
    /// 1) All targets in TargetMode are healed. (Note that behaviour is not defined if TargetTeamType is Team.Type.Heroes | Team.Type.Monsters).
    /// 2) A second TargetMode is passed in through AdditionalParamInt
    /// 3) The TargetMode is passed with the opposite team as 1)
    /// 4) The amount healed is multiplied by ParamMultiplier
    /// 5) If ParamBool is true the damage is split equally among damaged units. Otherwise each unit takes healed_amount * multiplier damage.
    /// 
    /// Params:
    ///   ParamInt: Amount to heal.
    ///   ParamMultiplier: Damage multiplier.
    ///   AdditionalParmaInt: Second target mode for the damage step
    ///   ParamBool: True to split the damage, false to apply the same amount of damage
    /// </summary>
    public sealed class CardEffectHealAndDamageRelativeCustomTargets : CardEffectHeal
    {
        private List<CharacterState> toProcessCharacters = new List<CharacterState>();

        private TargetHelper.CollectTargetsData collectTargetsData;

        public override bool CanRandomizeTargetMode => false;

        public override bool TestEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            return cardEffectParams.targets.Count > 0;
        }

        public override IEnumerator ApplyEffect(CardEffectState cardEffectState, CardEffectParams cardEffectParams)
        {
            int num = 0;
            foreach (var target in cardEffectParams.targets)
            {
                int healAmount = GetHealAmount(cardEffectState);
                int oldHp = target.GetHP();
                yield return target.ApplyHeal(healAmount, triggerOnHeal: true, cardEffectParams.playedCard);
                num += Mathf.Max(Mathf.RoundToInt((float)(target.GetHP() - oldHp) * cardEffectState.GetParamMultiplier()), 0);
            }

            if (num > 0)
            {
                var testTarget = cardEffectParams.targets[0];
                CollectTargets(cardEffectState, cardEffectParams, testTarget.GetTeamType(), testTarget.GetCurrentRoomIndex());
                if (toProcessCharacters.Count <= 0)
                {
                    yield break;
                }
                int damage = cardEffectState.GetParamBool() ? num / toProcessCharacters.Count : num;
                foreach (var target2 in toProcessCharacters)
                {
                    yield return cardEffectParams.combatManager.ApplyDamageToTarget(damage, target2, new CombatManager.ApplyDamageToTargetParameters
                    {
                        playedCard = cardEffectParams.playedCard,
                        finalEffectInSequence = cardEffectParams.finalEffectInSequence,
                        vfxAtLoc = cardEffectState.GetAppliedVFX(),
                        showDamageVfx = cardEffectParams.allowPlayingDamageVfx
                    });
                }
            }
        }

        private void CollectTargets(CardEffectState cardEffectState, CardEffectParams cardEffectParams, Team.Type team, int roomIndex, bool isTesting = false)
        {
            toProcessCharacters.Clear();
            collectTargetsData.Reset((TargetMode)cardEffectState.GetAdditionalParamInt());
            collectTargetsData.targetTeamType = team.GetOppositeTeam();
            collectTargetsData.roomIndex = roomIndex;
            collectTargetsData.inCombat = false;
            collectTargetsData.heroManager = cardEffectParams.heroManager;
            collectTargetsData.monsterManager = cardEffectParams.monsterManager;
            collectTargetsData.cardManager = cardEffectParams.cardManager;
            collectTargetsData.roomManager = cardEffectParams.roomManager;
            collectTargetsData.combatManager = cardEffectParams.combatManager;
            collectTargetsData.isTesting = isTesting;
            TargetHelper.CollectTargets(collectTargetsData, ref toProcessCharacters);
            collectTargetsData.Reset(TargetMode.FrontInRoom);
        }
    }

}
