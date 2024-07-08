using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trainworks.Enums
{
    /// <summary>
    /// An abstract class used to create classes that can safely extend onto existing enumerators
    /// </summary>
    /// <typeparam name="TExtendedEnum"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    public abstract class ExtendedEnum<TExtendedEnum, TEnum>
        where TExtendedEnum : ExtendedEnum<TExtendedEnum, TEnum>
        where TEnum : Enum
    {
        protected static Dictionary<int, TExtendedEnum> IntToExtendedEnumMap = new Dictionary<int, TExtendedEnum>();
        protected static Dictionary<string, TExtendedEnum> NameToExtendedEnumMap = new Dictionary<string, TExtendedEnum>();
        protected static List<int> ReservedIDs = ((int[])Enum.GetValues(typeof(TEnum))).ToList();
        protected int ID { get; private set; }
        protected string name;
        public string Name
        {
            get { return this.name; }
            private set { this.name = value; }
        }

        /// <summary>
        /// Base Constructor for creating an Extended Enumerator.
        /// The ID is determined by the Backing Enum type to avoid collisions with the base game and other mods.
        /// Do not depend on the ID being a specific value, since it changes based on what other Enums have been added
        /// by this class.
        /// 
        /// THe ID assignment is global meaning you can have as many ExtendedEnum sublasses (but why would you...?)
        /// </summary>
        /// <param name="name">Name of the Enum, it is shared across the Enum type.</param>
        public ExtendedEnum(string name)
        {
            Name = name;
            var GlobalEnumMap = GlobalEnumRecord<TEnum>.Instance;
            if (GlobalEnumMap.ContainsName(Name))
            {
                Trainworks.Log(LogLevel.Warning, $"Name: {Name} Conflict in domain, {typeof(TEnum).Name}");
            }
            this.ID = GlobalEnumMap.Add(Name);
            NameToExtendedEnumMap[Name] = (TExtendedEnum)this;
            IntToExtendedEnumMap[ID] = (TExtendedEnum)this;
        }

        /// <summary>
        /// Base Constructor for creating an Extended Enumerator
        /// </summary>
        /// <param name="Name">Name of new Enum Value</param>
        /// <param name="ID">ID of new Enum Value</param>
        [Obsolete("ExtendedEnum(string, int) is deprecated, The ID will be ignored. Please use the ExtendedEnum(string) overload.")]
        public ExtendedEnum(string Name, int ID)
        {
            this.Name = Name;
            var GlobalEnumMap = GlobalEnumRecord<TEnum>.Instance;

            if (GlobalEnumMap.ContainsName(this.Name))
            {
                Trainworks.Log(LogLevel.Warning, $"Name: {this.Name} Conflict in domain, {typeof(TEnum).Name}");
            }
            this.ID = GlobalEnumMap.Add(Name);
            NameToExtendedEnumMap[Name] = (TExtendedEnum)this;
            IntToExtendedEnumMap[this.ID] = (TExtendedEnum)this;
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
        public static int[] GetAllIDs() => IntToExtendedEnumMap.Keys.ToArray();

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
        public static TExtendedEnum GetValueOrDefault(int Key) => IntToExtendedEnumMap.GetValueOrDefault(Key);

        /// <summary>
        /// Returns a generated variant of TEnum that can be used for Trainworks functions
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        [Obsolete("Usage of this function is unnecessary (you should never need to convert a BaseGame Enum Type to an ExtendedEnum instance), and calls can be removed. (Also this function has a bug)")]
        public static TExtendedEnum Convert(TEnum @enum)
        {
            int id = System.Convert.ToInt32((Enum)@enum);
            if (IntToExtendedEnumMap.ContainsKey(id))
            {
                TExtendedEnum @extendedEnum = (TExtendedEnum)Activator.CreateInstance(typeof(TExtendedEnum));
                @extendedEnum.ID = id;
                @extendedEnum.Name = "Generated_" + Enum.GetName(typeof(TEnum), @enum);
                NameToExtendedEnumMap[@extendedEnum.Name] = @extendedEnum;
                IntToExtendedEnumMap[@extendedEnum.ID] = @extendedEnum;
                return @extendedEnum;
            }
            else
            {
                return IntToExtendedEnumMap[id];
            }
        }
    }
}
