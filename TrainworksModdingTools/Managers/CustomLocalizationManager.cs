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
        private static readonly HashSet<String> CSVFilesLoaded = new HashSet<String>();
        // Replacement strings currently in use
        internal static Dictionary<string, ReplacementStringData> ReplacementStrings;

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
        public static void ImportCSV(string path, char Separator = ',')
        {
            if (CSVFilesLoaded.Contains(path))
            {
                Trainworks.Log(LogLevel.Error, "CSV localization file: " + path + " has already been imported. Ignoring...");
                return;
            }

            CSVFilesLoaded.Add(path);

            string CSVstring = "";

            var localPath = Path.GetDirectoryName(new Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath);
            Trainworks.Log(BepInEx.Logging.LogLevel.Debug, "Loading Localization CSV File: " + Path.Combine(localPath, path));

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(Path.Combine(localPath, path)))
                {
                    // Read the stream to a string, and write the string to the console.
                    CSVstring = sr.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                Trainworks.Log(LogLevel.Error, "Couldn't read file: " + Path.Combine(localPath, path));
                Trainworks.Log(LogLevel.Error, e.Message);
            }

            List<string> categories = LocalizationManager.Sources[0].GetCategories(true, (List<string>)null);
            foreach (string Category in categories)
                LocalizationManager.Sources[0].Import_CSV(Category, CSVstring, eSpreadsheetUpdateMode.AddNewTerms, Separator);
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
            LanguageSourceData I2Languages = I2.Loc.LocalizationManager.Sources[0];
            if (I2Languages.ContainsTerm($"Default\\{key}"))
                return;
            if (!key.HasTranslation())
            {
                if (french == null) french = english;
                if (german == null) german = english;
                if (russian == null) russian = english;
                if (portuguese == null) portuguese = english;
                if (chinese == null) chinese = english;

                TermData data = I2Languages.AddTerm($"Default\\{key}", eTermType.Text, false);
                data.Languages[I2Languages.GetLanguageIndex("English")] = english;
                data.Languages[I2Languages.GetLanguageIndex("French")] = french;
                data.Languages[I2Languages.GetLanguageIndex("German")] = german;
                data.Languages[I2Languages.GetLanguageIndex("Russian")] = russian;
                data.Languages[I2Languages.GetLanguageIndex("Portuguese (Brazil)")] = portuguese;
                data.Languages[I2Languages.GetLanguageIndex("Chinese")] = chinese;
                I2Languages.UpdateDictionary(true);
            }
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
    }
}
