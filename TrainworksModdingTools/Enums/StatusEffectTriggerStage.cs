using System;
using System.Collections.Generic;
using System.Text;

namespace Trainworks.Enums
{
    public class StatusEffectTriggerStage : ExtendedByteEnum<StatusEffectTriggerStage, StatusEffectData.TriggerStage>
    {
        private static byte InitialID = 128;

        public StatusEffectTriggerStage(string Name, byte? ID = null) : base(Name, ID ?? GetNewID())
        {

        }

        public static byte GetNewID()
        {
            InitialID++;
            return InitialID;
        }
    }

}
