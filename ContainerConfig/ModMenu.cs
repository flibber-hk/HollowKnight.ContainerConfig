using System;
using ItemChanger.Extensions;
using Satchel.BetterMenus;

namespace ContainerConfig
{
    public static class ModMenu
    {
        private static GlobalSettings GS => ContainerConfigMod.GS;

        private static Menu MenuRef;

        private static readonly string[] ContainerOptions = new[]
        {
            "No Change",
            "Shiny",
            "Mimic",
        };
        private static readonly string[] ReplaceOptions = new[]
        {
            "No Special Container",
            "No Special Item",
            "Any Possible",
        };

        internal static MenuScreen GetMenuScreen(MenuScreen modListMenu)
        {
            MenuRef ??= new Menu(ContainerConfigMod.instance.GetName(), new Element[]
            {
                new HorizontalOption(
                    "Preferred Container Type",
                    "Which container type to place at locations",
                    ContainerOptions,
                    n => GS.DefaultContainerType = ContainerOptions[n],
                    () => ContainerOptions.IndexOf(GS.DefaultContainerType)
                    ),
                new HorizontalOption(
                    "Replace Single Locations",
                    "If false, will only affect locations with multiple items",
                    new[]{ "False", "True" },
                    n => GS.AffectSingleLocations = n == 1,
                    () => GS.AffectSingleLocations ? 1 : 0
                    ),
                new HorizontalOption(
                    "Replacement",
                    "How to decide which locations to affect",
                    ReplaceOptions,
                    n => GS.ReplacementSelectorOption = (GlobalSettings.ReplacementSelectorOptions)n,
                    () => (int)GS.ReplacementSelectorOption
                    ),
                new TextPanel("Changes made here will only apply to new save files"),
            });

            return MenuRef.GetMenuScreen(modListMenu);
        }
    }
}
