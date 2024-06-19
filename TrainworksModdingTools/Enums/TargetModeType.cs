using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Trainworks.Enums;
using Trainworks.Patches;

namespace Trainworks.Enums
{
    using CharacterTargetSelectionFunc = Action<TargetHelper.CollectTargetsData, List<CharacterState>, List<CharacterState>>;
    using CardTargetSelectionFunc = Action<TargetHelper.CollectTargetsData, List<CardState>>;

    public class TargetModeType : ExtendedByteEnum<TargetModeType, TargetMode>
    {
        private static byte InitialID = 48;
        internal static readonly Dictionary<TargetMode, CharacterTargetSelectionFunc> CharacterTargetModeHandlers = new Dictionary<TargetMode, CharacterTargetSelectionFunc>();
        internal static readonly Dictionary<TargetMode, CardTargetSelectionFunc> CardTargetModeHandlers = new Dictionary<TargetMode, CardTargetSelectionFunc>();
        internal static readonly List<TargetMode> InvalidInvertedCardTargetModes = new List<TargetMode>
        {
            TargetMode.Discard,
            TargetMode.DrawPile,
            TargetMode.Exhaust,
            TargetMode.Eaten,
            TargetMode.Hand,
        };
        internal static readonly List<TargetMode> InvalidInvertedTargetModes = new List<TargetMode>
        {
            TargetMode.Tower,
            TargetMode.Pyre,
        };
#pragma warning disable CS0618 // Type or member is obsolete
        internal static TargetMode InvertTargets = new TargetModeType("Invert", 128).GetEnum();
#pragma warning restore CS0618 // Type or member is obsolete

        [Obsolete("Please use the overload accepting (string, Action, byte?)")]
        public TargetModeType(string Name, byte? ID = null) : base(Name, ID ?? GetNewID())
        {
        }

        /// <summary>
        /// Creates a new Target Mode type with its own handler
        /// </summary>
        /// <param name="Name">Name of the TargetMode</param>
        /// <param name="func">A function responsible for selecting the targets. parameters (CollectTargetsData. AllTargets, out Targets)
        /// The first parameter being a CollectTargetsData which contains all of the parameters.
        /// The second parameter being all available targets in the room (no need for filtering out Phased, subtypes, etc that's already done). **DO NOT** iterate over this list with a foreach loop.
        /// The third parameter is to be filled with the selected targets.
        /// </param>
        /// <param name="ID">Optional assigned ID.</param>
        public TargetModeType(string Name, CharacterTargetSelectionFunc func, byte? ID = null) : base(Name, ID ?? GetNewID())
        {
            CharacterTargetModeHandlers.Add(this.GetEnum(), func);
        }

        /// <summary>
        /// Creates a new Target Mode type with its own handler
        /// </summary>
        /// <param name="Name">Name of the TargetMode</param>
        /// <param name="func">A function responsible for selecting the cards. parameters (CollectTargetsData, out TargetCards)
        /// The first parameter being a CollectTargetsData which contains all of the parameters.
        /// The second parameter is to be filled with the selected target cards.
        /// </param>
        /// <param name="ID">Optional assigned ID.</param>
        public TargetModeType(string Name, CardTargetSelectionFunc func, byte? ID = null) : base(Name, ID ?? GetNewID())
        {
            CardTargetModeHandlers.Add(this.GetEnum(), func);
        }

        /// <summary>
        /// Given a TargetMode (including custom TargetMode) returns the TargetMode that selects the targets not matching the original condition.
        /// </summary>
        public static TargetMode MakeInverted(TargetMode targetMode)
        {
            if (CardTargetModeHandlers.ContainsKey(targetMode) || InvalidInvertedCardTargetModes.Contains(targetMode))
            {
                Trainworks.Log(LogLevel.Fatal, "Can't invert a TargetMode specifying Cards other than PlayedCard or LastDrawnCard.");
            }
            if (InvalidInvertedTargetModes.Contains(targetMode))
            {
                Trainworks.Log(LogLevel.Fatal, "Can't invert TargetMode.Tower or TargetMode.Pyre");
            }
            return (TargetMode)((byte)InvertTargets | (byte)targetMode);
        }

        public static TargetMode GetOriginalTargetMode(TargetMode targetMode)
        {
            return (TargetMode)(~(byte)InvertTargets & (byte)targetMode);
        }

        public static bool IsInverted(TargetMode targetMode)
        {
            return ((int)targetMode & (int)TargetModeType.InvertTargets) != 0;
        }

        public static byte GetNewID()
        {
            InitialID++;
            if (InitialID >= 128)
            {
                Trainworks.Log(LogLevel.Error, "Too many TargetModeTypes have been created");
            }
            return InitialID;
        }
    }

    public static class TargetModeExtensions
    {
        public static TargetMode Invert(this TargetMode targetmode)
        {
            return TargetModeType.MakeInverted(targetmode);
        }
    }
}
