using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using Trainworks.Managers;

namespace Trainworks
{
    /// <summary>
    /// The entry point for the framework.
    /// </summary>
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("MonsterTrain.exe")]
    [BepInProcess("MtLinkHandler.exe")]
    public class Trainworks : BaseUnityPlugin
    {
        public const string GUID = "tools.modding.trainworks";
        public const string NAME = "Trainworks Modding Tools";
        public const string VERSION = "2.5.0";

        /// <summary>
        /// The framework's logging source.
        /// </summary>
        private static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("Trainworks");

        public static string APIBasePath { get; private set; }

        /// <summary>
        /// Logs a message into the BepInEx console.
        /// </summary>
        /// <param name="lvl">The severity of the message</param>
        /// <param name="msg">The message to log</param>
        public static void Log(LogLevel lvl, string msg)
        {
            /*Assembly assembly = Assembly.GetCallingAssembly();
            string guid = PluginManager.AssemblyNameToPluginGUID[assembly.FullName];
            if (guid != GUID || lvl != LogLevel.All)
                lvl = LogLevel.None;*/

            logger.Log(lvl, string.Format("[{0}] {1}", DateTime.UtcNow.ToString("HH:mm:ss.ffffff"), msg));
        }

        /// <summary>
        /// Logs a message into the BepInEx console with LogLevel "Debug"
        /// </summary>
        /// <param name="msg">The message to log</param>
        public static void Log(string msg)
        {
            Log(LogLevel.Debug, msg);
        }

        /// <summary>
        /// Called on startup. Executes all Harmony patches anywhere in the framework's assembly.
        /// </summary>
        private void Awake()
        {
            DepInjector.AddClient(new ProviderManager());

            var assembly = this.GetType().Assembly;
            APIBasePath = Path.GetDirectoryName(assembly.Location);

            // Register in order to provide dummy synthesis data for compatibility reasons
            PluginManager.RegisterPlugin(this);

            var harmony = new Harmony(GUID);
            harmony.PatchAll();
        }
    }
}
