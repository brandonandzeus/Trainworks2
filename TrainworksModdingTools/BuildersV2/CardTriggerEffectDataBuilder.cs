using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Trainworks.BuildersV2
{
    public class CardTriggerEffectDataBuilder
    {
        private string triggerID;

        /// <summary>
        /// Unique ID for CardTriggerEffect.
        /// Implicitly sets DescriptionKey if null.
        /// </summary>
        public string TriggerID
        {
            get { return triggerID; }
            set
            {
                triggerID = value;
                if (DescriptionKey == null)
                {
                    DescriptionKey = triggerID + "_CardTriggerEffectData_DescriptionKey";
                }
            }
        }

        /// <summary>
        /// Card Trigger Type.
        /// </summary>
        public CardTriggerType Trigger { get; set; }
        /// <summary>
        /// Custom description for the trigger effect.
        /// Note that setting this property will set the localization for all languages.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Use an existing base game trigger's description key to copy the format of its description.
        /// *HIGHLY* Recommended to set this property if using a custom card trigger effect as the localization key is not unique.
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// List of CardTrigger Effects.
        /// Note no builder since all fields in CardTriggerData are public.
        /// These CardTriggerEffects are ran first.
        /// </summary>
        public List<CardTriggerData> CardTriggerEffects { get; set; }
        /// <summary>
        /// List of normal CardEffects to run. 
        /// The CardEffects are ran AFTER CardTriggerEffects.
        /// </summary>
        public List<CardEffectData> CardEffects { get; set; }
        /// <summary>
        /// Convenience Builders to append to CardEffects.
        /// </summary>
        public List<CardEffectDataBuilder> CardEffectBuilders { get; set; }

        public CardTriggerEffectDataBuilder()
        {
            CardTriggerEffects = new List<CardTriggerData>();
            CardEffectBuilders = new List<CardEffectDataBuilder>();
            CardEffects = new List<CardEffectData>();
        }

        /// <summary>
        /// Builds the CardTriggerEffectData represented by this builder's parameters.
        /// all Builders represented in this class's various fields will also be built.
        /// </summary>
        /// <returns>The newly created CardTriggerEffectData</returns>
        public CardTriggerEffectData Build()
        {
            // Not catastrophic enough to throw an Exception, this should be provided though.
            if (TriggerID == null)
            {
                Trainworks.Log(LogLevel.Error, "Warning should provide a TriggerID.");
                Trainworks.Log(LogLevel.Debug, "Stacktrace: " + Environment.StackTrace);
            }

            // Doesn't inherit from ScriptableObject
            CardTriggerEffectData cardTriggerEffectData = new CardTriggerEffectData();

            AccessTools.Field(typeof(CardTriggerEffectData), "descriptionKey").SetValue(cardTriggerEffectData, DescriptionKey);
            AccessTools.Field(typeof(CardTriggerEffectData), "trigger").SetValue(cardTriggerEffectData, Trigger);

            // Saving allocations by adding to the list that was already allocated.
            var cardEffects = cardTriggerEffectData.GetCardEffects();
            cardEffects.AddRange(CardEffects);
            foreach (var builder in CardEffectBuilders)
            {
                cardEffects.Add(builder.Build());
            }

            var cardTriggerEffects = cardTriggerEffectData.GetTriggerEffects();
            cardTriggerEffects.AddRange(CardTriggerEffects);

            BuilderUtils.ImportStandardLocalization(DescriptionKey, Description);

            return cardTriggerEffectData;
        }

        /// <summary>
        /// Adds a new CardTrigger to the list.
        /// </summary>
        /// <param name="persistenceMode">SingleRun, or SingleBattle</param>
        /// <param name="cardTriggerEffect">ICardTriggerEffect subclass this is the CardTriggerEffect to play and will be instantiated</param>
        /// <param name="buffEffectType">Used by CardTriggerBuffEffect.
        /// ICardEffect subclass this should match one of the CardEffects within CardData.Effects (Not CardTriggerEffectData's CardEffects). 
        /// If given will reset the CardEffectState that has EffectStateName equal to this class.
        /// However this is bugged if you have multiple CardEffects since it reorders the effects of your card since it removes the effect pointed to by buffEffectTytpe
        /// and readds it at the end of the effects list at the end of battle if the PersistenceMode is SingleBattle.
        /// 
        /// </param>
        /// <param name="paramInt">ParamInt</param>
        /// <param name="upgrade">ParamUpgrade</param>
        /// <returns></returns>
        public CardTriggerData AddCardTrigger(PersistenceMode persistenceMode, Type cardTriggerEffect, Type buffEffectType, int paramInt, CardUpgradeData upgrade)
        {
            var trigger = MakeCardTrigger(persistenceMode, cardTriggerEffect, buffEffectType, paramInt, upgrade);
            CardTriggerEffects.Add(trigger);
            return trigger;
        }

        /// <summary>
        /// Makes a CardTriggerData.
        /// </summary>
        /// <param name="persistenceMode">SingleRun, or SingleBattle</param>
        /// <param name="cardTriggerEffect">ICardTriggerEffect subclass this is the CardTriggerEffect to play and will be instantiated</param>
        /// <param name="buffEffectType">Used by CardTriggerBuffEffect.
        /// ICardEffect subclass this should match one of the CardEffects within CardData.Effects (Not CardTriggerEffectData's CardEffects).
        /// If given will reset the CardEffectState that has EffectStateName equal to this class.
        /// However this is bugged if you have multiple CardEffects since it reorders the effects of your card since it removes the effect pointed to by buffEffectTytpe
        /// and readds it at the end of the effects list at the end of battle if the PersistenceMode is SingleBattle.
        /// </param>
        /// <param name="paramInt">ParamInt</param>
        /// <param name="upgrade">ParamUpgrade</param>
        /// <returns></returns>
        public static CardTriggerData MakeCardTrigger(PersistenceMode persistenceMode, Type cardTriggerEffect, Type buffEffectType, int paramInt, CardUpgradeData upgrade)
        {
            CardTriggerData trigger = new CardTriggerData
            {
                persistenceMode = persistenceMode,
                cardTriggerEffect = BuilderUtils.GetEffectClassName(cardTriggerEffect),
                buffEffectType = buffEffectType == null ? "None" : BuilderUtils.GetEffectClassName(buffEffectType),
                paramInt = paramInt,
                paramUpgrade = upgrade
            };

            return trigger;
        }

        // Unsafe An incorrect Class type can be passed in.
        [Obsolete("AddCardTrigger(PersistenceMode, string, string, int) is deprecated. Please switch to the overload AddCardTrigger(PersistenceMode, Type, Type, int, CardUpgradeData).")]
        public CardTriggerData AddCardTrigger(PersistenceMode persistenceMode, string cardTriggerEffect, string buffEffectType, int paramInt)
        {
            CardTriggerData trigger = new CardTriggerData
            {
                persistenceMode = persistenceMode,
                cardTriggerEffect = cardTriggerEffect,
                buffEffectType = buffEffectType,
                paramInt = paramInt
            };

            CardTriggerEffects.Add(trigger);
            return trigger;
        }

        // Unsafe An incorrect Class type can be passed in.
        [Obsolete("MakeCardTrigger(PersistenceMode, string, string, int) is deprecated. Please switch to the overload MakeCardTrigger(PersistenceMode, Type, Type, int, CardUpgradeData).")]
        public static CardTriggerData MakeCardTrigger(PersistenceMode persistenceMode, string cardTriggerEffect, string buffEffectType, int paramInt)
        {
            CardTriggerData trigger = new CardTriggerData
            {
                persistenceMode = persistenceMode,
                cardTriggerEffect = cardTriggerEffect,
                buffEffectType = buffEffectType,
                paramInt = paramInt
            };

            return trigger;
        }
    }
}