using HarmonyLib;

namespace Trainworks.CustomRelicEffects
{
    /// <summary>
    /// Interface for a RelicEffect that runs its effects after a new run has been setup.
    /// 
    /// Very useful for the StarterRelics param for Custom clans that want to modify the starter deck.
    /// A good example is a Wurmkin-like Relic that setups the deck and gives Starter Cards Infused.
    /// 
    /// If you use StarterRelics the artifacts specified will be given *AT THE VERY START* of a run
    /// before the Convenants are setup. This is an issue since the Covenants effects will run and  will
    /// remove all cards and then add the appropriate starter deck afterward thus negating any modifications 
    /// you did to the starter deck.
    /// </summary>
    interface IPostStartOfRunRelicEffect : IRelicEffect
    {
        void ApplyEffect(RelicEffectParams param);
    }
}
