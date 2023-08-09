using BepInEx;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace ModSettings
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            string path = Path.Combine(Application.persistentDataPath, "ModSettings/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Patches.SettingsUIHelperPatch.Apply();
        }
    }
}
