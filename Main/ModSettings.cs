using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ModSettings
{
    public static class ModSettings
    {
        public static void CreateSelector(string name, string modId, string displayName, List<string> options, string defaultValue = "")
        {
            Selector newSetting = (Selector)CreateSetting(name, modId, SettingTypes.Selector, displayName);
            newSetting.selectorOptions = options;
            if (defaultValue == "")
            {
                defaultValue = options[0];
            }
            newSetting.defaultValue = defaultValue;
        }

        public static void CreateCheckbox(string name, string modId, string displayName, bool defaultValue = false)
        {
            Tick newSetting = (Tick)CreateSetting(name, modId, SettingTypes.Tick, displayName);
            newSetting.defaultValue = defaultValue;
        }

        public static void CreateSlider(string name, string modId, string displayName, float minValue = 0f, float maxValue = 1f, float increments = 0.1f, float defaultValue = 1f)
        {
            Slider newSetting = (Slider)CreateSetting(name, modId, SettingTypes.Slider, displayName);
            newSetting.minValue = minValue;
            newSetting.maxValue = maxValue;
            newSetting.defaultValue = defaultValue;
            newSetting.valueIncrement = increments;
        }

        public static string GetSelectorValue(string name, string modId)
        {
            foreach (ModSetting setting in settings)
            {
                if (setting.settingID == name && setting.modID == modId)
                {
                    switch (setting.settingType)
                    {
                        case SettingTypes.Selector:
                            return ((Selector)setting).GetValue();
                        default:
                            return null;
                    }
                }
            }
            return null;
        }

        public static bool GetCheckboxValue(string name, string modId)
        {
            foreach (ModSetting setting in settings)
            {
                if (setting.settingID == name && setting.modID == modId)
                {
                    switch (setting.settingType)
                    {
                        case SettingTypes.Tick:
                            return ((Tick)setting).GetValue();
                        default:
                            return false;
                    }
                }
            }
            return false;
        }
        public static float GetSliderValue(string name, string modId)
        {
            foreach (ModSetting setting in settings)
            {
                if (setting.settingID == name && setting.modID == modId)
                {
                    switch (setting.settingType)
                    {
                        case SettingTypes.Slider:
                            return ((Slider)setting).GetValue();
                        default:
                            return 0f;
                    }
                }
            }
            return 0f;
        }

        public static void SaveSetting(string name, string modId, object value)
        {
            string path = Path.Combine(Application.persistentDataPath, "ModSettings/", modId + ".cfg");
            List<string> lines = new List<string>();
            bool FoundLine = false;
            if (File.Exists(path))
            {
                foreach (string line in File.ReadAllLines(path))
                {
                    lines.Add(line);
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith(name + "="))
                    {
                        lines[i] = name + "=" + value.ToString();
                        FoundLine = true;
                        break;
                    }
                }
                if (!FoundLine)
                {
                    lines.Add(name + "=" + value.ToString());
                }
            }
            else
            {
                 lines.Add(name + "=" + value.ToString());
            }
            File.WriteAllLines(path, lines);
        }

        public static object LoadSetting(string name, string modId, SettingTypes type)
        {
            string path = Path.Combine(Application.persistentDataPath, "ModSettings/", modId + ".cfg");
            List<string> lines = new List<string>();
            if (!File.Exists(path))
            {
                return null;
            }

            foreach (string line in File.ReadAllLines(path))
            {
                lines.Add(line);
            }
            
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(name + "="))
                {
                    switch (type)
                    {
                        case SettingTypes.Selector:
                            return lines[i].Split("=")[1];
                        case SettingTypes.Tick:
                            return bool.Parse(lines[i].Split("=")[1]);
                        case SettingTypes.Slider:
                            return float.Parse(lines[i].Split("=")[1]);
                    }
                    break;
                }
            }
            return null;
        }

        private static ModSetting CreateSetting(string name, string modId, SettingTypes type, string displayName)
        {
            ModSetting newSetting = null;
            switch (type)
            {
                case SettingTypes.Selector:
                    newSetting = new Selector();
                    break;
                case SettingTypes.Tick:
                    newSetting = new Tick();
                    break;
                case SettingTypes.Slider:
                    newSetting = new Slider();
                    break;
            }

            if (newSetting == null)
            {
                return null;
            }

            newSetting.settingID = name;
            newSetting.modID = modId;
            newSetting.dispayName = displayName;
            newSetting.settingType = type;

            settings.Add(newSetting);
            return newSetting;
        }

        public abstract class ModSetting
        {
            public abstract void OnChange();

            public string modID;      
            public string settingID;
            public string dispayName;
            public SettingTypes settingType;
            public object defaultValue;
        }

        public class Selector : ModSetting
        {
            public string GetValue()
            {
                return selectedValue;
            }
            public override void OnChange()
            {
                selectedValue = selector.options[selector.Index];
                SaveSetting(settingID, modID, selectedValue);
            }

            public List<string> selectorOptions;
            public string selectedValue;
            public EnumSelector selector;
        }

        public class Tick : ModSetting
        {
            public bool GetValue()
            {
                return value;
            }
            public override void OnChange()
            {
                value = checkbox.state;
                SaveSetting(settingID, modID, value);
            }

            public bool value;
            public Checkbox checkbox;
        }

        public class Slider : ModSetting
        {
            public float GetValue()
            {
                return value;
            }
            public override void OnChange()
            {
                value = slider.value;
                SaveSetting(settingID, modID, value);
            }

            public float value;
            public float minValue;
            public float maxValue;
            public float valueIncrement;
            public TFUISlider slider;
        }

        public static List<ModSetting> settings = new List<ModSetting>();
    }

    public enum SettingTypes
    {
        Selector = 0,
        Tick = 1,
        Slider = 2
    }
}
