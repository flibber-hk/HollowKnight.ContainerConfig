using System.Collections.Generic;

namespace ContainerConfig
{
    public static class GlobalSettingsPresets
    {
        public static readonly List<(string name, GlobalSettings settings)> Presets = new()
        {
            ("Disabled", new() { DefaultContainerType = ItemChanger.Container.Unknown }),
            
            ("Prefer Multi Shiny", new() 
            { 
                AffectSingleLocations = false,
                DefaultContainerType = ItemChanger.Container.Shiny,
                ReplacementSelectorOption = GlobalSettings.ReplacementSelectorOptions.NoRequestedContainer
            }),

            ("No Unexpected Items", new()
            {
                AffectSingleLocations = false,
                DefaultContainerType = ItemChanger.Container.Shiny,
                ReplacementSelectorOption = GlobalSettings.ReplacementSelectorOptions.AnyGiveLate
            }),

            ("Mimicpocalypse", new()
            {
                AffectSingleLocations = true,
                DefaultContainerType = ItemChanger.Container.Mimic,
                ReplacementSelectorOption = GlobalSettings.ReplacementSelectorOptions.AnyContainerLocation
            })
        };
    }
}
