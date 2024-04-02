using System.Reflection;
using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System;
using System.Diagnostics;

namespace TouchGestures.Installer
{
    [PluginName(PLUGIN_NAME)]
    public class TouchGesturesInstaller : ITool
    {
        public const string PLUGIN_NAME = "Touch Gestures Installer";
#if NET6_0
        public const string OTD_VERSION = "0.6.x";
#elif NET5_0
        public const string OTD_VERSION = "0.5.x";
#else
        public const string OTD_VERSION = "";
#endif

        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();

        private static readonly FileInfo location = new(assembly.Location);
        private static readonly DirectoryInfo? pluginsDirectory = location.Directory?.Parent;
        private static readonly string AssemblySuffix = OTD_VERSION == "0.5.x" ? "" : $"-{OTD_VERSION}";

        private readonly DirectoryInfo OTDEnhancedOutputModeDirectory = null!;


        private readonly string dependenciesResourcePath = $"Touch-Gestures.Installer{AssemblySuffix}.Touch-Gestures-{OTD_VERSION}.zip";

        public TouchGesturesInstaller()
        {
            if (pluginsDirectory == null || !pluginsDirectory.Exists)
            {
                Log.Write(PLUGIN_NAME, "Failed to get plugins directory.", LogLevel.Error);
                return;
            }

            // Look for OTD.EnhancedOutputMode.dll within the plugins directory
            foreach (var pluginDirectory in pluginsDirectory.GetDirectories())
            {
                foreach (var file in pluginDirectory.GetFiles())
                {
                    if (file.Name == "OTD.EnhancedOutputMode.dll")
                    {
                        OTDEnhancedOutputModeDirectory = pluginDirectory;
                        return;
                    }
                }
            }
        }

        public bool Initialize()
        {
            _ = Task.Run(() => Install(assembly, PLUGIN_NAME, dependenciesResourcePath, OTDEnhancedOutputModeDirectory, ForceInstall));
            return true;
        }

        public bool Install(Assembly assembly, string group, string resourcePath, DirectoryInfo destinationDirectory, bool forceInstall = false)
        {
            if (pluginsDirectory == null || !pluginsDirectory.Exists)
            {
                Log.Write(group, "Failed to get plugins directory.", LogLevel.Error);
                return false;
            }

            if (!OTDEnhancedOutputModeDirectory.Exists)
            {
                Log.Write(group, "OTD.EnhancedOutputMode is not installed.", LogLevel.Error);
                return false;
            }

            var dependencies = assembly.GetManifestResourceStream(resourcePath);

            if (dependencies == null)
            {
                Log.Write(group, "Failed to open embedded dependencies.", LogLevel.Error);
                return false;
            }

            int entriesCount = 0;
            int installed = 0;

            using (ZipArchive archive = new(dependencies, ZipArchiveMode.Read))
            {
                var entries = archive.Entries;
                entriesCount = entries.Count;

                foreach (ZipArchiveEntry entry in entries)
                {
                    FileInfo destinationFile = new($"{destinationDirectory}/{entry.FullName}");

                    if (destinationFile.Exists && !forceInstall)
                        continue;

                    entry.ExtractToFile(destinationFile.FullName, true);
                    installed++;
                }
            }

            // last step: remove OTD.Backport.Parsers.dll in any other plugin directory
            /*foreach (var pluginDirectory in pluginsDirectory.GetDirectories())
            {
                if (pluginDirectory.Name == OTDEnhancedOutputModeDirectory.Name)
                    continue;

                var parserDll = new FileInfo($"{pluginDirectory.FullName}/OTD.Backport.Parsers.dll");

                if (parserDll.Exists)
                {
                    Log.Write(group, $"Unable to remove the duplicate dll '{parserDll.FullName}'.", LogLevel.Warning);
                    Log.Write(group, "It is required to remove this dll for this plugin to work.", LogLevel.Warning);
                }
            }*/

            if (installed > 0)
            {
                string successMessage = $"Successfully installed {installed} of {entriesCount} dependencies.";
                string spacer = new('-', successMessage.Length);
                
                Log.Write(group, spacer, LogLevel.Info);
                Log.Write(group, $"Installed {installed} of {entriesCount} dependencies.", LogLevel.Info);
                Log.Write(group, $"You may need to restart OpenTabletDriver before the plugin can be enabled.", LogLevel.Info);
                Log.Write(group, spacer, LogLevel.Info);
            }

            return true;
        }

        public void Dispose()
        {
            
        }

        [BooleanProperty("Force Install", ""),
         DefaultPropertyValue(false),
         ToolTip("Force install the dependencies even if they are already installed.")]
        public bool ForceInstall { get; set; }
    }
}
