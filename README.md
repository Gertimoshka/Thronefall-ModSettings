# Thronefall - ModSettings
This mod adds mod settings for other mods to use and be able to simply and easily add settings for their mods

# How to Install
You will need to download BepInEx 6 and copy it into your Thronefall Directory, after that you will need to Extract the downloaded zip into the plugins folder of the BePiNex Folder.

# Dependencies
None

# How to Use (as Modders)
Once you install you will have to add this mod as a library for your mod, after that its simple.
There are currently 3 types of options:
Selector
```C#
ModSettings.CreateSelector("<SettingName>", "<modID>", "<displayName>", new List<string>(), "<defaultValue>");
string value = ModSettings.GetSelectorValue("<SettingName>", "<modID>");
```
This will create a selector which can have options inserted as string list, the default value is optional and if left blank, will become the first option.
Getting Selector Value will return the current option as string value.
```C#
ModSettings.CreateCheckbox("<SettingName>", "<modID>", "<displayName>", false);
bool value = ModSettings.GetCheckboxValue("<SettingName>", "<modID>");
```
This will create a checkbox which is very simple to understand.
Getting Checkbox Value will return a bool based on state.
```C#
ModSettings.CreateSlider("<SettingName>", "<modID>", "<displayName>", 0f, 1f, 0.1f, 1f);
float value = ModSettings.GetSliderValue("<SettingName>", "<modID>");
```
This will create a slider that can have minValue, maxValue, valueIncrements, defaultValue
Getting Slider Value will return a float based on current number, note if value is changed in settings to a number outside the range it will snap back into range, and will round to nearest increment
