using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using Trainworks.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Trainworks.Managers
{
    /// <summary>
    /// Handles loading of custom localized strings.
    /// </summary>
    public class CustomLocalizationManager
    {
        // Separator to CSV File Path.
        private static Dictionary<char, HashSet<String>> SeparatorsToCsvFiles = new Dictionary<char, HashSet<String>>();
        // Replacement strings currently in use
        internal static Dictionary<string, ReplacementStringData> ReplacementStrings;
        // Single localization lines added.
        private static readonly Dictionary<string, string> CSVLineStrings = new Dictionary<string, string>();

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
                                data = data.Replace(separator, ',');
                            }
                            miniCSVBuilder.Append(data);
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
        }


        public static string ExportCSV(int language=0)
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
        /// <returns></returns>
        public static bool AddReplacementString(string keyword, string replacement)
        {
            if (ReplacementStrings.ContainsKey(keyword))
            {
                Trainworks.Log("Attempt to add duplicate Replacement String Keyword: " + keyword);
                return false;
            }
            ReplacementStringData replacementStringData = new ReplacementStringData();
            AccessTools.Field(typeof(ReplacementStringData), "_keyword").SetValue(replacementStringData, keyword);
            AccessTools.Field(typeof(ReplacementStringData), "_replacement").SetValue(replacementStringData, replacement);
            ReplacementStrings.Add(keyword, replacementStringData);
            return true;
        }

        /// <summary>
        /// Marks a Custom Card Trait class as one that has a tooltip
        /// </summary>
        /// <param name="cardTraitClass"></param>
        public static void AllowCustomCardTraitTooltips(Type cardTraitClass)
        {
            var traits = (List<string>) AccessTools.Field(typeof(TooltipContainer), "TraitsSupportedInTooltips").GetValue(null);
            // Have to add both the name and qualified name as the checks for this could be from CardTraitData.TraitStateName or CardTraitState.GetType().Name
            // The first will always be the fully qualified assembly name the second since it is direct access will be the class name.
            traits.Add(cardTraitClass.Name);
            traits.Add(cardTraitClass.AssemblyQualifiedName);
        }
    }
}
