using System;
using System.Linq;
using ItemChanger;
using ItemChanger.Extensions;
using Satchel.BetterMenus;
using UnityEngine;
using UnityEngine.UI;
using MenuButton = Satchel.BetterMenus.MenuButton;

namespace ContainerConfig
{
    public static class ModMenu
    {
        private static GlobalSettings GS
        {
            get => ContainerConfigMod.GS;
            set => ContainerConfigMod.GS = value;
        }

        public static event Action RecalculateClicked;

        private static Menu MenuRef;

        private static readonly string[] ContainerOptions = new[]
        {
            "No Change",
            Container.Shiny,
            Container.Mimic,
            Container.GrubJar,
        };
        private static readonly string[] ReplaceOptions = new[]
        {
            "No Special Container",
            "No Special Item",
            "Any Possible",
        };

        private static void SetApplyColor(Color c)
        {
            MenuRef.Find("Apply").gameObject.transform.Find("Label").GetComponent<Text>().color = c;
        }

        private static void HideCustomizationElements()
        {
            MenuRef.Find("Preferred Container").Hide();
            MenuRef.Find("Replace Single Locations").Hide();
            MenuRef.Find("Replacement").Hide();
        }

        private static void ShowCustomizationElements()
        {
            MenuRef.Find("Preferred Container").Show();
            MenuRef.Find("Replace Single Locations").Show();
            MenuRef.Find("Replacement").Show();
        }

        private static int GetContainerIndex(string container)
        {
            int index = Array.IndexOf(ContainerOptions, container);
            if (index == -1) index = 0;
            return index;
        }

        internal static MenuScreen GetMenuScreen(MenuScreen modListMenu)
        {
            MenuRef ??= new Menu(ContainerConfigMod.instance.GetName(), new Element[]
            {
                new HorizontalOption(
                    "Config Preset",
                    "",
                    GlobalSettingsPresets.Presets.Select(pair => pair.name).Append("Custom").ToArray(),
                    n => 
                    {
                        if (n < GlobalSettingsPresets.Presets.Count)
                        {
                            ContainerConfigMod.GS = GlobalSettingsPresets.Presets.ElementAt(n).settings;
                            HideCustomizationElements();
                        }
                        else
                        {
                            ShowCustomizationElements();
                        }
                        SetApplyColor(Color.yellow);
                    },
                    () =>
                    {
                        for (int i = 0; i < GlobalSettingsPresets.Presets.Count; i++)
                        {
                            if (GlobalSettingsPresets.Presets[i].settings == GS)
                            {
                                HideCustomizationElements();
                                return i;
                            }
                        }
                        ShowCustomizationElements();
                        return GlobalSettingsPresets.Presets.Count;
                    }
                    ),
                new HorizontalOption(
                    "Preferred Container",
                    "Which container type to place at locations",
                    ContainerOptions,
                    n => 
                    {
                        GS = GS with { DefaultContainerType = ContainerOptions[n] == "No Change" ? Container.Unknown : ContainerOptions[n] };
                        SetApplyColor(Color.yellow); 
                    },
                    () => GetContainerIndex(GS.DefaultContainerType)
                    ),
                new HorizontalOption(
                    "Replace Single Locations",
                    "If false, will only affect locations with multiple items",
                    new[]{ "False", "True" },
                    n =>
                    { 
                        GS = GS with { AffectSingleLocations = n == 1 };
                        SetApplyColor(Color.yellow);
                    },
                    () => GS.AffectSingleLocations ? 1 : 0
                    ),
                new HorizontalOption(
                    "Replacement",
                    "How to decide which locations to affect",
                    ReplaceOptions,
                    n =>
                    {
                        GS = GS with { ReplacementSelectorOption = (GlobalSettings.ReplacementSelectorOptions)n };
                        SetApplyColor(Color.yellow);
                    },
                    () => (int)GS.ReplacementSelectorOption
                    ),
                new MenuButton(
                    "Apply",
                    "Click to apply to the current save",
                    _ =>
                    {
                        RecalculateClicked?.Invoke();
                        SetApplyColor(Color.white);
                    }
                    )
            });

            return MenuRef.GetMenuScreen(modListMenu);
        }
    }
}
