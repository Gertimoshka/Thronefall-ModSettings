using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HarmonyLib;
using UnityEngine.Events;

namespace ModSettings.Patches
{
    public static class SettingsUIHelperPatch
    {
        public static void Apply()
        {
            On.SettingsUIHelper.Awake += Awake;
        }

        private static void Awake(On.SettingsUIHelper.orig_Awake orig, SettingsUIHelper self)
        {
            orig(self);
            //Innitialize new Elements
            var Tabs = Traverse.Create(self).Field<Dictionary<ThronefallUIElement, SettingsUIHelper.SettingsTab>>("allTabs").Value;

            SettingsUIHelper.SettingsTab newTab = new SettingsUIHelper.SettingsTab();
            GameObject tabParent = GameObject.Instantiate(GameObject.Find("UI Canvas/Settings Frame/Category Tabs/Video"), GameObject.Find("UI Canvas/Settings Frame/Category Tabs").transform);
            tabParent.name = "Mods";
            modTab = tabParent;
            GameObject tabChild = GameObject.Instantiate(GameObject.Find("UI Canvas/Settings Frame/Video Settings"), GameObject.Find("UI Canvas/Settings Frame").transform);
            tabChild.name = "Mods Settings";

            newTab.parentElement = tabParent.GetComponent<TFUITextButton>();
            newTab.childContainer = tabChild.GetComponent<VerticalLayoutGroup>();

            Tabs.Add(newTab.parentElement, newTab);
            newTab.childContainer.gameObject.SetActive(false);
            newTab.ComputeNavigationForSettingsTab();

            Traverse.Create(self).Field("allTabs").SetValue(Tabs);

            //Change Title
            GameObject.DestroyImmediate(tabParent.GetComponent<I2.Loc.Localize>());
            tabParent.GetComponent<TextMeshProUGUI>().text = "Mods";
            Traverse.Create(tabParent.GetComponent<TFUITextButton>()).Field("originalString").SetValue("Mods");

            //Clear Sub
            for (int i = 0; i < tabChild.transform.childCount; i++)
            {
                GameObject.Destroy(tabChild.transform.GetChild(i).gameObject);
            }

            //Create Mod Select
            modSelect = GameObject.Instantiate(GameObject.Find("UI Canvas/Settings Frame/Video Settings/Resolution"), tabChild.transform);
            modSelect.name = "ModSelect";
            GameObject.DestroyImmediate(modSelect.GetComponent<I2.Loc.Localize>());
            GameObject.DestroyImmediate(modSelect.GetComponent<SettingsResolution>());
            modSelect.GetComponent<TextMeshProUGUI>().text = "Selected Mod Settings";
            var enumSelector = modSelect.GetComponent<EnumSelector>();
            var Options = enumSelector.options;
            Options.Clear();
            Options.Add("No Mod Settings Found");
            enumSelector.options = Options;
            enumSelector.SetIndex(0);
            enumSelector.delayedApply = false;

            //Copy and Cache Setting Types
            settingTypes.Add(GameObject.Instantiate(GameObject.Find("UI Canvas/Settings Frame/Video Settings/Resolution"), tabChild.transform));
            GameObject.DestroyImmediate(settingTypes[0].GetComponent<I2.Loc.Localize>());
            GameObject.DestroyImmediate(settingTypes[0].GetComponent<SettingsResolution>());
            settingTypes[0].GetComponent<TextMeshProUGUI>().text = "Preset_Selector";
            settingTypes[0].name = "Preset_Selector";
            settingTypes[0].SetActive(false);
            settingTypes[0].GetComponent<EnumSelector>().delayedApply = false;

            settingTypes.Add(GameObject.Instantiate(GameObject.Find("UI Canvas/Settings Frame/Video Settings/Full Screen"), tabChild.transform));
            GameObject.DestroyImmediate(settingTypes[1].GetComponent<I2.Loc.Localize>());
            GameObject.DestroyImmediate(settingTypes[1].GetComponent<SettingsFullscreen>());
            settingTypes[1].GetComponent<TextMeshProUGUI>().text = "Preset_Tick";
            settingTypes[1].name = "Preset_Tick";
            settingTypes[1].SetActive(false);

            settingTypes.Add(GameObject.Instantiate(GameObject.Find("UI Canvas/Settings Frame/Video Settings/Render Scale"), tabChild.transform));
            GameObject.DestroyImmediate(settingTypes[2].GetComponent<I2.Loc.Localize>());
            GameObject.DestroyImmediate(settingTypes[2].GetComponent<SettingsRenderScale>());
            settingTypes[2].GetComponent<TextMeshProUGUI>().text = "Preset_Slider";
            settingTypes[2].name = "Preset_Slider";
            settingTypes[2].SetActive(false);
            GameObject.DestroyImmediate(settingTypes[2].transform.Find("Slider Background/Tooltip").gameObject);

            //Create Settings
            foreach (ModSettings.ModSetting setting in ModSettings.settings)
            {

                GameObject newSetting = GameObject.Instantiate(settingTypes[(int)setting.settingType], tabChild.transform);
                newSetting.name = setting.modID + "_" + setting.settingID;
                switch (setting.settingType)
                {
                    case SettingTypes.Selector:
                        var Selector = newSetting.GetComponent<EnumSelector>();
                        Selector.options = ((ModSettings.Selector)setting).selectorOptions;
                        var value0 = ModSettings.LoadSetting(setting.settingID, setting.modID, setting.settingType);
                        if (value0 == null)
                        {
                            ((ModSettings.Selector)setting).selectedValue = (string)setting.defaultValue;
                            ModSettings.SaveSetting(setting.settingID, setting.modID, setting.defaultValue);
                        }
                        else
                        {
                            ((ModSettings.Selector)setting).selectedValue = (string)value0;
                        }
                        for (int i = 0; i < ((ModSettings.Selector)setting).selectorOptions.Count; i++)
                        {
                            if (((ModSettings.Selector)setting).selectorOptions[i] == ((ModSettings.Selector)setting).selectedValue)
                            {
                                Selector.SetIndex(i);
                                break;
                            }
                        }
                        Selector.onChange.AddListener(new UnityAction(setting.OnChange));
                        ((ModSettings.Selector)setting).selector = Selector;
                        break;
                    case SettingTypes.Tick:
                        var Tick = newSetting.GetComponentInChildren<Checkbox>();
                        var value1 = ModSettings.LoadSetting(setting.settingID, setting.modID, setting.settingType);
                        if (value1 == null)
                        {
                            ((ModSettings.Tick)setting).value = (bool)setting.defaultValue;
                            ModSettings.SaveSetting(setting.settingID, setting.modID, setting.defaultValue);
                        }
                        else
                        {
                            ((ModSettings.Tick)setting).value = (bool)value1;
                        }
                        Tick.onToggle.AddListener(new UnityAction(setting.OnChange));
                        Tick.SetState(((ModSettings.Tick)setting).value);
                        ((ModSettings.Tick)setting).checkbox = Tick;
                        break;
                    case SettingTypes.Slider:
                        var Slider = newSetting.GetComponent<TFUISlider>();
                        var value2 = ModSettings.LoadSetting(setting.settingID, setting.modID, setting.settingType);
                        if (value2 == null)
                        {
                            ((ModSettings.Slider)setting).value = (float)setting.defaultValue;
                            ModSettings.SaveSetting(setting.settingID, setting.modID, setting.defaultValue);
                        }
                        else
                        {
                            ((ModSettings.Slider)setting).value = Mathf.RoundBasedOnMinimumDifference((float)value2, ((ModSettings.Slider)setting).valueIncrement);
                            if (((ModSettings.Slider)setting).value > ((ModSettings.Slider)setting).maxValue)
                            {
                                ((ModSettings.Slider)setting).value = ((ModSettings.Slider)setting).maxValue;
                            }
                            else if (((ModSettings.Slider)setting).value < ((ModSettings.Slider)setting).minValue)
                            {
                                ((ModSettings.Slider)setting).value = ((ModSettings.Slider)setting).minValue;
                            }
                        }
                        Slider.onChange.AddListener(new UnityAction(setting.OnChange));
                        Slider.minValue = ((ModSettings.Slider)setting).minValue;
                        Slider.maxValue = ((ModSettings.Slider)setting).maxValue;
                        Slider.increments = ((ModSettings.Slider)setting).valueIncrement;
                        Slider.SetValue(((ModSettings.Slider)setting).value);
                        ((ModSettings.Slider)setting).slider = Slider;
                        break;
                }
                newSetting.SetActive(false);
                newSetting.GetComponent<TextMeshProUGUI>().text = setting.dispayName;
                modSettings.Add(newSetting);

                if (!modIDs.Contains(setting.modID))
                {
                    modIDs.Add(setting.modID);
                }
            }
            //Final Touchups
            if (modIDs.Count > 0)
            {
                enumSelector.options = modIDs;
                enumSelector.SetIndex(0);
                enumSelector.onChange.AddListener(new UnityAction(OnModSelectChange));
                OnModSelectChange();
            }

            //Fix Navigations
            TFUITextButton modsButton = tabParent.GetComponent<TFUITextButton>();
            TFUITextButton gameplayButton = GameObject.Find("UI Canvas/Settings Frame/Category Tabs/Gameplay").GetComponent<TFUITextButton>();
            TFUITextButton videoButton = GameObject.Find("UI Canvas/Settings Frame/Category Tabs/Video").GetComponent<TFUITextButton>();
            modsButton.leftNav = gameplayButton;
            modsButton.rightNav = videoButton;
            videoButton.leftNav = modsButton;
            gameplayButton.rightNav = modsButton;
            TFUITextButton buttonInfo = modSelect.GetComponent<TFUITextButton>();
            buttonInfo.topNav = modsButton;
            modsButton.botNav = buttonInfo;
            if (modIDs.Count == 0)
            {
                buttonInfo.botNav = modsButton;
                modsButton.topNav = buttonInfo;
            }
        }

        private static void OnModSelectChange()
        {
            EnumSelector enumSelector = modSelect.GetComponent<EnumSelector>();
            int Index = -1;
            for(int i = 0; i < modIDs.Count; i++)
            {
                if (enumSelector.Index == i)
                {
                    Index = i;
                    break;
                }
            }
            if (Index == -1)
            {
                return;
            }

            List<TFUITextButton> buttons = new List<TFUITextButton>();
            foreach(GameObject setting in modSettings)
            {
                if (setting.name.StartsWith(modIDs[Index]))
                {
                    setting.SetActive(true);
                    buttons.Add(setting.GetComponent<TFUITextButton>());
                }
                else
                {
                    setting.SetActive(false);
                }
            }

            TFUITextButton buttonInfo = modSelect.GetComponent<TFUITextButton>();
            TFUITextButton tabInfo = modTab.GetComponent<TFUITextButton>();

            buttonInfo.botNav = buttons[0];
            tabInfo.topNav = buttons[buttons.Count - 1];

            for (int i = 0; i < buttons.Count; i++)
            {
                if (i == 0)
                {
                    buttons[i].topNav = buttonInfo;
                    buttons[i].botNav = buttons[i + 1];
                }
                else if (i == buttons.Count - 1)
                {
                    buttons[i].topNav = buttons[i - 1];
                    buttons[i].botNav = tabInfo;
                }
                else
                {
                    buttons[i].topNav = buttons[i - 1];
                    buttons[i].botNav = buttons[i + 1];
                }
            }
        }

        private static GameObject modSelect;
        private static GameObject modTab;

        private static List<GameObject> settingTypes = new List<GameObject>();

        private static List<string> modIDs = new List<string>();
        private static List<GameObject> modSettings = new List<GameObject>();
    }
}
