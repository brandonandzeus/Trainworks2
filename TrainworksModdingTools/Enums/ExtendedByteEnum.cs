using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainworks.Enums
{
    /// <summary>
    /// An abstract class used to create classes that can safely extend onto existing enumerators
    /// </summary>
    /// <typeparam name="TExtendedEnum"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    public abstract class ExtendedByteEnum<TExtendedEnum, TEnum>
        where TExtendedEnum : ExtendedByteEnum<TExtendedEnum, TEnum>
        where TEnum : Enum
    {
        protected static Dictionary<byte, TExtendedEnum> ByteToExtendedEnumMap = new Dictionary<byte, TExtendedEnum>();
        protected static Dictionary<string, TExtendedEnum> NameToExtendedEnumMap = new Dictionary<string, TExtendedEnum>();
        protected static List<byte> ReservedIDs = ((byte[])Enum.GetValues(typeof(TEnum))).ToList();
        protected byte ID { get; private set; }
        protected string name;
        public string Name
        {
            get { return this.name; }
            private set { this.name = value; }
        }

        /// <summary>
        /// Base Constructor for creating an Extended Enumerator
        /// </summary>
        /// <param name="Name">Name of new Enum Value</param>
        /// <param name="ID">ID of new Enum Value</param>
        public ExtendedByteEnum(string Name, byte ID)
        {
            this.ID = ID;
            this.Name = Name;
            if (NameToExtendedEnumMap.ContainsKey(this.Name))
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, $"Name: {this.Name} Conflict in domain, {typeof(TExtendedEnum).Name}");
            }
            if (ByteToExtendedEnumMap.ContainsKey(this.ID))
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, $"ID#{this.ID} Conflict between {Name} and {ByteToExtendedEnumMap[this.ID].Name} in domain, {typeof(TExtendedEnum).Name}");
            }
            if (ReservedIDs.Contains(this.ID))
            {
                Trainworks.Log(BepInEx.Logging.LogLevel.Warning, $"ID#{this.ID} is Reserved and can't be set for {Name}");
            }
            NameToExtendedEnumMap[Name] = (TExtendedEnum)this;
            ByteToExtendedEnumMap[ID] = (TExtendedEnum)this;
        }

        /// <summary>
        /// Returns the Enum equivalent of the new ExtendedEnum
        /// </summary>
        /// <returns>the Enum equivalent of the new ExtendedEnum</returns>
        public virtual TEnum GetEnum() => (TEnum)Enum.ToObject(typeof(TEnum), ID);

        /// <summary>
        /// Returns all IDs of all ExtendedEnum classes
        /// </summary>
        /// <returns>all IDs of all ExtendedEnum classes</returns>
        public static byte[] GetAllIDs() => ByteToExtendedEnumMap.Keys.ToArray();

        /// <summary>
        /// Returns all names of all ExtendedEnum classes
        /// </summary>
        /// <returns>all names of all ExtendedEnum classes</returns>
        public static string[] GetAllNames() => NameToExtendedEnumMap.Keys.ToArray();

        /// <summary>
        /// Returns the value given a key or default
        /// </summary>
        /// <param name="Key">string key to get value</param>
        /// <returns></returns>
        public static TExtendedEnum GetValueOrDefault(string Key) => NameToExtendedEnumMap.GetValueOrDefault(Key);

        /// <summary>
        /// Returns the value given a key or default
        /// </summary>
        /// <param name="Key">int key to get value</param>
        /// <returns></returns>
        public static TExtendedEnum GetValueOrDefault(byte Key) => ByteToExtendedEnumMap.GetValueOrDefault(Key);

        /// <summary>
        /// Returns a generated variant of TEnum that can be used for Trainworks functions
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static TExtendedEnum Convert(TEnum @enum)
        {
            byte id = System.Convert.ToByte((Enum)@enum);
            if (ByteToExtendedEnumMap.ContainsKey(id))
            {
                TExtendedEnum @extendedEnum = (TExtendedEnum)Activator.CreateInstance(typeof(TExtendedEnum));
                @extendedEnum.ID = id;
                @extendedEnum.Name = "Generated_" + Enum.GetName(typeof(TEnum), @enum);
                NameToExtendedEnumMap[@extendedEnum.Name] = @extendedEnum;
                ByteToExtendedEnumMap[@extendedEnum.ID] = @extendedEnum;
                return @extendedEnum;
            }
            else
            {
                return ByteToExtendedEnumMap[id];
            }
        }
    }

}
