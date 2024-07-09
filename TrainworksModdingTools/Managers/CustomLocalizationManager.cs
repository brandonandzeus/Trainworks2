using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Trainworks.Managers
{
    using DynamicLocalizationParameterFunction = Func<string, ILocalizationParameterContext, string>;

    /// <summary>
    /// Handles loading of custom localized strings.
    /// </summary>
    public class CustomLocalizationManager : ILocalizationParamsManager
    {
        // Separator to CSV File Path.
        private static Dictionary<char, HashSet<String>> SeparatorsToCsvFiles = new Dictionary<char, HashSet<String>>();
        // Replacement strings currently in use
        internal static Dictionary<string, ReplacementStringData> ReplacementStrings;
        // Dynamic localization paremeter functions.
        internal static Dictionary<string, DynamicLocalizationParameterFunction> DynamicLocalizationParameters = new Dictionary<string, DynamicLocalizationParameterFunction>();
        // Static localization parameters
        internal static Dictionary<string, string> LocalizationParameters = new Dictionary<string, string>();
        // Single localization lines added.
        private static readonly Dictionary<string, string> CSVLineStrings = new Dictionary<string, string>();
        // Flag to specify that Localization Data has already been loaded and further changes are ignored.
        public static bool HasBeenLoaded { get; private set; }

        internal static CustomLocalizationManager instance = new CustomLocalizationManager();

        static CustomLocalizationManager()
        {

        }

        private CustomLocalizationManager()
        {

        }

        internal static CustomLocalizationManager Instance()
        {
            return instance;
        }

        // Required because the library just chokes. When we need plural support, we can reimplement this.
        // The existing issue was that LocalizationUtil.GetPluralsUsedByLanguages() returns one less than mTerm.Languages.Length
        // It needs to return the same amount. Changing the number is also a problem, too lazy to debug now.
        // This function is only in use by csv imports, so we affect nothing else by skipping it.
        [HarmonyPatch(typeof(LanguageSourceData), "SimplifyPlurals")]
        class SkipBrokenLibraryFunction
        {
            static bool Prefix(ref LanguageSourceData __instance)
            {
                return true;
            }
        }

        /// <summary>
        /// Imports new data to the English Localization from a CSV string with separators ';'.
        /// Required columns are 'Keys', 'Type', 'Plural', 'Group', 'Desc', 'Descriptions'
        /// </summary>
        public static void ImportCSV(string path, char separator = ',')
        {
            if (separator != ',')
            {
                Trainworks.Log(LogLevel.Warning, "CSV file with a different separator found, please convert to a CSV file that is comma separated. Importing may cause issues.");
            }
            if (!SeparatorsToCsvFiles.ContainsKey(separator))
            {
                SeparatorsToCsvFiles.Add(separator, new HashSet<String>());
            }
            var localPath = Path.GetDirectoryName(new Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath);
            var fullPath = Path.Combine(localPath, path);
            if (SeparatorsToCsvFiles[separator].Contains(fullPath))
            {
                Trainworks.Log(LogLevel.Error, "Attempt to Import CSV file: " + fullPath + " multiple times. Please only call this function once in your Plugin's Initialize");
            }
            if (HasBeenLoaded)
            {
                Trainworks.Log(LogLevel.Error, "Localization data has already been uploaded to the I2.Loc library, any additions will not be reflected. Localization data should only be added in your Plugin's Initialize method");
            }
            SeparatorsToCsvFiles[separator].Add(fullPath);
        }

        /// <summary>
        /// Appends a Single Localization to the Localization Manager
        /// </summary>
        /// <param name="key">a string that is not null or empty that will act as a key for other strings</param>
        /// <param name="type">type for the string to be converted to, typically is Text</param>
        /// <param name="desc">description of what key represents, typically unused</param>
        /// <param name="plural">The plural of the string, currently broken</param>
        /// <param name="group">the name of the group that the key is apart of</param>
        /// <param name="descriptions">an extra description</param>
        /// <param name="english">The English Translation</param>
        /// <param name="french">The French Translation</param>
        /// <param name="german">The German Translation</param>
        /// <param name="russian">The Russian Translation</param>
        /// <param name="portuguese">The Portuguese Translation</param>
        /// <param name="chinese">The Chinese Translation</param>
        public static void ImportSingleLocalization(string key, string type, string desc, string plural, string group, string descriptions, string english, string french = null, string german = null, string russian = null, string portuguese = null, string chinese = null)
        {
            if (HasBeenLoaded)
            {
                Trainworks.Log(LogLevel.Error, "Localization data has already been uploaded to the I2.Loc library, any additions will not be reflected. Localization data should only be added in your Plugin's Initialize method");
                return;
            }
            if (string.IsNullOrEmpty(key)) return;
            if (!key.HasTranslation() && !CSVLineStrings.ContainsKey(key))
            {
                if (!english.StartsWith("\"") && english.Contains(","))
                    english = string.Format("\"{0}\"", english);

                if (french == null) french = english;
                if (german == null) german = english;
                if (russian == null) russian = english;
                if (portuguese == null) portuguese = english;
                if (chinese == null) chinese = english;

                var miniCSVBuilder = new System.Text.StringBuilder();
                miniCSVBuilder.Append(key + ",");
                miniCSVBuilder.Append(type + ",");
                miniCSVBuilder.Append(desc + ",");
                miniCSVBuilder.Append(plural + ",");
                miniCSVBuilder.Append(group + ",");
                miniCSVBuilder.Append(descriptions + ",");
                miniCSVBuilder.Append(english + ",");
                miniCSVBuilder.Append(french + ",");
                miniCSVBuilder.Append(german + ",");
                miniCSVBuilder.Append(russian + ",");
                miniCSVBuilder.Append(portuguese + ",");
                miniCSVBuilder.Append(chinese);

                CSVLineStrings.Add(key, miniCSVBuilder.ToString());
            }
        }

        /// <summary>
        /// Do not call this function. It is called once all Plugin's Initialize function has been called.
        /// Calling this function incurs a huge overhead.
        /// </summary>
        internal static void ImportLocalizationData()
        {
            var miniCSVBuilder = new System.Text.StringBuilder();
            String header = "Key,Type,Desc,Plural,Group,Descriptions,English [en-US],French [fr-FR],German [de-DE],Russian,Portuguese (Brazil),Chinese [zh-CN]";
            miniCSVBuilder.AppendLine(header);

            foreach (var sep_path in SeparatorsToCsvFiles)
            {
                var separator = sep_path.Key;
                var files = sep_path.Value;

                // Ensure a valid header is present.
                String valid_header = header;
                if (separator != ',')
                    valid_header = header.Replace(',', separator);

                foreach (var file in files)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(file))
                        {
                            String file_header = sr.ReadLine();
                            if (file_header != valid_header)
                            {
                                Trainworks.Log(LogLevel.Error, "Incorrect header format found for csv file: " + file);
                                Trainworks.Log(LogLevel.Error, "Found header: " + file_header);
                                Trainworks.Log(LogLevel.Error, "Expected header: " + header);
                                continue;
                            }

                            String data = sr.ReadToEnd();
                            // This is safe, the only non CSV csv file is Arcadian and works when the ;'s are replaced.
                            if (separator != ',')
                            {
                                var builder = new StringBuilder();
                                var lines = data.Split(separator);
                                for (int i = 0; i < lines.Length; i++)
                                {
                                    if (lines[i].Contains(","))
                                    {
                                        builder.Append(string.Format("\"{0}\"", lines[i]));
                                    }
                                    else
                                    {
                                        builder.Append(lines[i]);
                                    }
                                    if (i != lines.Length - 1)
                                    {
                                        builder.Append(',');
                                    }
                                }
                                data = builder.ToString();
                            }
                            miniCSVBuilder.AppendLine(data);
                        }
                    }
                    catch (IOException e)
                    {
                        Trainworks.Log(LogLevel.Error, "Couldn't read CSV file: " + file);
                        Trainworks.Log(LogLevel.Error, e.Message);
                    }
                }
            }

            foreach (var line in CSVLineStrings.Values)
            {
                miniCSVBuilder.AppendLine(line);
            }

            List<string> categories = LocalizationManager.Sources[0].GetCategories(true, (List<string>)null);
            foreach (string Category in categories)
                LocalizationManager.Sources[0].Import_CSV(Category, miniCSVBuilder.ToString(), eSpreadsheetUpdateMode.AddNewTerms, ',');

            HasBeenLoaded = true;
        }


        public static string ExportCSV(int language = 0)
        {
            string ret = "";
            List<string> categories = LocalizationManager.Sources[0].GetCategories(true, (List<string>)null);
            foreach (string Category in categories)
                ret += LocalizationManager.Sources[0].Export_CSV(Category);

            return ret;
        }

        /// <summary>
        /// Adds a ReplacementString with key and replacement.
        /// When used in tooltips and CardText will replace [keyword] with replacement.Localize().
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="replacement">Localized text key for replacement</param>
        /// <returns>True if a Replacement String was added</returns>
        public static bool AddReplacementString(string keyword, string replacement)
        {
            if (ReplacementStrings.ContainsKey(keyword))
            {
                Trainworks.Log(LogLevel.Warning, "Attempt to add duplicate Replacement String Keyword: " + keyword);
                return false;
            }
            ReplacementStringData replacementStringData = new ReplacementStringData();
            AccessTools.Field(typeof(ReplacementStringData), "_keyword").SetValue(replacementStringData, keyword);
            AccessTools.Field(typeof(ReplacementStringData), "_replacement").SetValue(replacementStringData, replacement);
            ReplacementStrings.Add(keyword, replacementStringData);
            return true;
        }

        /// <summary>
        /// Registers a Dynamic Localization Parameter.
        /// 
        /// This allows for a parameter to be passed when text is localized which allows for changing a part of a translation
        /// with some runtime values from in game.
        /// 
        /// To use in your CSV file include the text {[PARAMETER]}, calling this function passing "PARAMETER" and a function
        /// that takes in a string and returns a string. The string passed in will be the paramter(s) registered with the function
        /// and you are to return the replacement text.
        /// 
        /// Note that if your text resolves to something within the base game that needs to be processed normally i.e. [effect0.status0.power]
        /// Then the function `LocalizationManager.ApplyLocalizationParams(ref text)` needs to be called within your function before the text is returned.
        /// 
        /// </summary>
        /// <param name="param">Localization Param</param>
        /// <param name="func">Function that handles returning the runtime text to replace the param with</param>
        /// <returns></returns>
        public static bool AddDynamicParameter(string param, DynamicLocalizationParameterFunction func)
        {
            if (DynamicLocalizationParameters.ContainsKey(param))
            {
                Trainworks.Log(LogLevel.Warning, "Attempt to add duplicate Dynamic Parameter: " + param);
                return false;
            }
            if (LocalizationParameters.ContainsKey(param))
            {
                Trainworks.Log(LogLevel.Warning, "Can't assign parameter to be both dynamic and static: " + param);
                return false;
            }

            DynamicLocalizationParameters.Add(param, func);
            return true;
        }

        /// <summary>
        /// Sets a Parameter to be a static string.
        /// 
        /// This function can be called multiple times if you want to change the Parameter's value.
        /// 
        /// To use in your CSV file include the text {[PARAMETER]}, calling this function passing "PARAMETER" with a string
        /// will replace the {[PARAMETER]} text with the value passed in.
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetParameterValue(string param, string value)
        {
            if (DynamicLocalizationParameters.ContainsKey(param))
            {
                Trainworks.Log(LogLevel.Warning, "Can't assign parameter to be both dynamic and static: " + param);
                return false;
            }

            LocalizationParameters[param] = value;
            return true;
        }

        /// <summary>
        /// Marks a Custom Card Trait class as one that has a tooltip
        /// </summary>
        /// <param name="cardTraitClass"></param>
        public static void AllowCustomCardTraitTooltips(Type cardTraitClass)
        {
            var traits = (List<string>)AccessTools.Field(typeof(TooltipContainer), "TraitsSupportedInTooltips").GetValue(null);
            // Have to add both the name and qualified name as the checks for this could be from CardTraitData.TraitStateName or CardTraitState.GetType().Name
            // The first will always be the fully qualified assembly name the second since it is direct access will be the class name.
            traits.Add(cardTraitClass.Name);
            traits.Add(cardTraitClass.AssemblyQualifiedName);
        }

        public string GetParameterValue(string Param, ILocalizationParameterContext context)
        {
            if (DynamicLocalizationParameters.ContainsKey(Param))
            {
                return DynamicLocalizationParameters[Param].Invoke(Param, context);
            }
            else if (LocalizationParameters.ContainsKey(Param))
            {
                return LocalizationParameters[Param];
            }
            return null;
        }

        public int? GetParameterPluralAmount(string param, ILocalizationParameterContext context)
        {
            return null;
        }
    }
}
