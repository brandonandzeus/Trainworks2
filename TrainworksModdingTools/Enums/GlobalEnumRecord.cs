using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RimLight;

namespace Trainworks.Enums
{
    /// <summary>
    /// Global dictionary used in ExtendedEnum.
    /// Necessary since unfortunately ExtendedEnum wasn't marked private anyone can extend the existing enum types.
    /// 
    /// The issue with this is if Trainworks extends an Enum and another mod extends that same Enum in a different class
    /// that the ID tracking isn't global across these ExtendedEnum classes. I.E.
    /// 
    /// Mod A creates an enum with value 33.
    /// Mod B creates an enum with value 33.
    /// 
    /// The ExtendedEnum class won't warn about a collision since the previous internal mapping is unique to each class.
    /// 
    /// This class implements a Global dictionary of enum values created and handles ID generation.
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    internal sealed class GlobalEnumRecord<TEnum> 
        where TEnum : Enum
    {
        private static readonly GlobalEnumRecord<TEnum> instance = new GlobalEnumRecord<TEnum>();

        private int NextID = (from int x in Enum.GetValues(typeof(TEnum)).AsQueryable() select x).Max() + 1;
        private readonly IDictionary<string, TEnum> NameToEnumMap = new Dictionary<string, TEnum>();
        private readonly IDictionary<TEnum, string> EnumToNameMap = new Dictionary<TEnum, string>();

        static GlobalEnumRecord()
        {
        }

        private GlobalEnumRecord()
        {
        }

        public static GlobalEnumRecord<TEnum> Instance
        {
            get
            {
                return instance;
            }
        }

        public int Add(string name)
        {
            int value = NextID;
            TEnum key = (TEnum) Enum.ToObject(typeof(TEnum), value);
            EnumToNameMap[key] = name;
            NameToEnumMap[name] = key;
            NextID++;
            return value;
        }
         
        public bool ContainsName(string name)
        {
            return NameToEnumMap.ContainsKey(name);
        }
    }

    internal sealed class GlobalByteEnumRecord<TEnum>
        where TEnum : Enum
    {
        private static readonly GlobalByteEnumRecord<TEnum> instance = new GlobalByteEnumRecord<TEnum>();

        private byte NextID = (byte)((byte) (from byte x in Enum.GetValues(typeof(TEnum)).AsQueryable() select x).Max() + 1);
        private readonly IDictionary<string, TEnum> NameToEnumMap = new Dictionary<string, TEnum>();
        private readonly IDictionary<TEnum, string> EnumToNameMap = new Dictionary<TEnum, string>();

        static GlobalByteEnumRecord()
        {
        }

        private GlobalByteEnumRecord()
        {
        }

        public static GlobalByteEnumRecord<TEnum> Instance
        {
            get
            {
                return instance;
            }
        }

        public byte Add(string name)
        {
            byte value = NextID;
            TEnum key = (TEnum)Enum.ToObject(typeof(TEnum), value);
            EnumToNameMap[key] = name;
            NameToEnumMap[name] = key;
            NextID++;
            if (NextID == 0)
            {
                throw new InvalidOperationException("Too many Enums have been created");
            }
            return value;
        }

        public bool ContainsName(string name)
        {
            return NameToEnumMap.ContainsKey(name);
        }
    }
}
