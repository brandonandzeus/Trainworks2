using HarmonyLib;
using Trainworks.Custom.RelicEffects;

namespace Trainworks.Patches
{
    /// <summary>
    /// Implementation of IPostStartOfRunRelicEffect.
    /// </summary>
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.ApplyStartOfRunRelicEffects))]
    class ApplyStartOfRunRelicEffectsPatch
    {
        public static void Postfix(SaveManager ___saveManager, RelicEffectParams ___relicEffectParams)
        {
            foreach (RelicState currentRelic in ___saveManager.GetAllRelics())
            {
                foreach (IRelicEffect effect in currentRelic.GetEffects())
                {
                    if (effect is IPostStartOfRunRelicEffect postStartOfRunRelicEffect)
                    {
                        postStartOfRunRelicEffect.ApplyEffect(___relicEffectParams);
                    }
                }
            }
        }
    }
}
