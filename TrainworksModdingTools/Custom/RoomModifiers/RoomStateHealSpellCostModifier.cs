using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.Custom.RoomModifiers
{
    /// <summary>
    /// Simplified version of the Stock RoomStateHealCostModifier with improvements.
    /// The original has extraneous code that was checking for a condition (A monster with a spawn effect that applies regen) that doesn't exist.
    /// This version checks for various Healing card effects and can be extended across all mods, by mods with custom heal effects that don't
    /// inherit from CardEffectHeal adding to the OtherHealingCardEffects list.
    /// 
    /// The original only checked for CardEffectHeal (and subclasses).
    /// </summary>
    public class RoomStateHealSpellCostModifier : RoomStateCostModifierBase
    {
        public static List<Type> OtherHealingCardEffects = new List<Type>();

        public override int GetModifiedCost(CardState cardState, CardStatistics cardStatistics, MonsterManager monsterManager)
        {
            if (cardState.GetCardType() != CardType.Spell)
                return 0;

            foreach (CardEffectState effectState in cardState.GetEffectStates())
            {
                if (typeof(CardEffectHeal).IsAssignableFrom(effectState.GetCardEffect().GetType()))
                {
                    return modifiedCost;
                }
                if (OtherHealingCardEffects.Contains(effectState.GetCardEffect().GetType()))
                {
                    return modifiedCost;
                }
            }
            foreach (CardTriggerEffectState effectTrigger in cardState.GetTriggers())
            {
                if (effectTrigger.GetTrigger() != CardTriggerType.OnCast)
                {
                    continue;
                }
                foreach (CardEffectState effectState in effectTrigger.GetCardEffectParams())
                {
                    if (typeof(CardEffectHeal).IsAssignableFrom(effectState.GetCardEffect().GetType()))
                    {
                        return modifiedCost;
                    }
                    if (OtherHealingCardEffects.Contains(effectState.GetCardEffect().GetType()))
                    {
                        return modifiedCost;
                    }
                }
            }
            return 0;
        }
    }
}