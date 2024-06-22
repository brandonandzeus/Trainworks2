using System.Collections.Generic;
using Trainworks.Enums;

namespace Trainworks.Custom
{
    /// <summary>
    /// Example Custom TargetModes for use in mods of course!
    /// 
    /// In writing these **DO NOT** use TargetHelper.CollectTargetsData.statusId as this field is linked with ParamStr in CardEffectData,
    /// this will make the TargetMode less usuable since that field is used by certain CardEffect classes. Instead status effect filtering
    /// is already done for you via TargetModeStatusEffectsFilter.
    ///
    /// Custom TargetMode functions don't need to handle these parameters from CardEffectData: 
    /// IgnoreDead, TargetModeStatusEffectsFilter, TargetModeHealthFilter, TargetIgnoreBosses, TargetIgnorePyre, TargetSubtype
    /// These parameters are already handled before the CharacterTargetSelectionFunction is invoked.
    /// 
    /// Do not iterate over the second paramater of the CharacterTargetSelectionFunction with a foreach loop. There appears to be an issue with
    /// invalidating the Iterator. Instead use the standard for loop.
    /// 
    /// Lastly do not write a TargetMode that is just an inverts version of an existing one. That one is already given to you for free in the Framework. Just use TargetMode.Invert().
    /// 
    /// To reiterate the parameters CharacterTargetSelectionFunction is a function (Action) that takes in
    /// TargetHelper.CollectTargetsData data, List of CharacterState allValidTargets, List of CharacterState chosenTargets
    ///   data: The CollectTargetsData passed in.
    ///   allValidTargets: A list of all valid CharacterData matching the target filtering parameters from CardEffectData. Do not iterate over with a foreach loop.
    ///   chosenTargets: Output the characters your TargetMode selects by adding to this list.
    ///
    /// </summary>
    public static class TargetModes
    {
        /// <summary>
        /// Target the unit with the most attack. If multiple do then the frontmost unit will be selected.
        /// </summary>
        public static readonly TargetMode Strongest = new TargetModeType("Strongest", HandleStrongest).GetEnum();
        /// <summary>
        /// Same as Strongest, but it can't be selfTarget. This must be used within CharacterTriggerData.Effects otherwise it will be the same as Strongest.
        /// </summary>
        public static readonly TargetMode StrongestExcludingSelf = new TargetModeType("StrongestExcludingSelf", HandleStrongestExcludingSelf).GetEnum();
        /// <summary>
        /// Special character target type, Simply specifies OverrideTargetCharacter.
        /// This is useful for Custom Triggers as you can fire a trigger setting overrideTargetCharacter and not have to write Custom Card Effects to get the override target.
        /// (Added for readability, LastAttackedCharacter works however selfTarget needs to be specified as well).
        /// </summary>
        public static readonly TargetMode OverrideTarget = new TargetModeType("OverrideTarget", HandleOverrideTarget).GetEnum();

#pragma warning disable CS0618 // Type or member is obsolete
        /// <summary>
        /// Special card target mode, Adds the card itself to targetCards. Its implementation is in a patch.
        /// </summary>
        public static readonly TargetMode PlayedCard = new TargetModeType("PlayedCard").GetEnum();

#pragma warning restore CS0618 // Type or member is obsolete



        public static void HandleStrongest(TargetHelper.CollectTargetsData data, List<CharacterState> allValidTargets, List<CharacterState> chosenTargets)
        {
            chosenTargets.Clear();
            CharacterState strongest = null;
            for (int i = 0; i < allValidTargets.Count; i++)
            {
                var current = allValidTargets[i];
                if (strongest == null || current.GetAttackDamage() > strongest.GetAttackDamage())
                {
                    strongest = current;
                }
            }
            if (strongest != null)
            {
                chosenTargets.Add(strongest);
            }
        }

        public static void HandleStrongestExcludingSelf(TargetHelper.CollectTargetsData data, List<CharacterState> allValidTargets, List<CharacterState> chosenTargets)
        {
            chosenTargets.Clear();

            CharacterState self = data.selfTarget;

            CharacterState strongest = null;
            for (int i = 0; i < allValidTargets.Count; i++)
            {
                var current = allValidTargets[i];
                if (current == self)
                {
                    continue;
                }

                if (strongest == null || current.GetAttackDamage() > strongest.GetAttackDamage())
                {
                    strongest = current;
                }
            }
            if (strongest != null)
            {
                chosenTargets.Add(strongest);
            }
        }

        public static void HandleOverrideTarget(TargetHelper.CollectTargetsData data, List<CharacterState> allValidTargets, List<CharacterState> chosenTargets)
        {
            chosenTargets.Clear();

            if (data.overrideTargetCharacter != null)
            {
                chosenTargets.Add(data.overrideTargetCharacter);
            }
        }
    }
}
