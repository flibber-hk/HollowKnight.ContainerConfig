using ContainerConfig.IC;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Placements;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ContainerConfig
{
    public static class PlacementPatcher
    {
        private static GlobalSettings GS => ContainerConfigMod.GS;

        private static readonly Modding.ILogger _logger = new Modding.SimpleLogger("ContainerConfig.PlacementPatcher");

        private static readonly MethodInfo _mutablePlacementMi = typeof(MutablePlacement).GetMethod(nameof(MutablePlacement.ChooseContainerType));
        private static Hook _mutablePlacementHook;
        private static readonly MethodInfo _ecpMi = typeof(ExistingContainerPlacement).GetMethod(nameof(ExistingContainerPlacement.ChooseContainerType));
        private static Hook _existingContainerPlacementHook;

        internal static void Hook()
        {
            _mutablePlacementHook = new(_mutablePlacementMi, MutablePlacementChoose);
            _existingContainerPlacementHook = new(_ecpMi, EcpChoose);
        }

        private static string MutablePlacementChoose(
            Func<ISingleCostPlacement, ContainerLocation, IEnumerable<AbstractItem>, string> orig,
            ISingleCostPlacement pmt,
            ContainerLocation loc,
            IEnumerable<AbstractItem> items)
        {
            List<AbstractItem> itemsList = new(items);

            string originalChoice = orig(pmt, loc, itemsList);
            string newChoice = orig(pmt, loc, itemsList.Prepend(EmptyItem.Create(GS.DefaultContainerType)));

            return GlobalChoose(pmt, loc, itemsList, originalChoice, newChoice);
        }

        private static string EcpChoose(
            Func<ISingleCostPlacement, ExistingContainerLocation, IEnumerable<AbstractItem>, string> orig,
            ISingleCostPlacement pmt,
            ExistingContainerLocation loc,
            IEnumerable<AbstractItem> items)
        {
            List<AbstractItem> itemsList = new(items);

            string originalChoice = orig(pmt, loc, itemsList);
            string newChoice = orig(pmt, loc, itemsList.Prepend(EmptyItem.Create(GS.DefaultContainerType)));

            return GlobalChoose(pmt, loc, itemsList, originalChoice, newChoice);
        }

        /// <summary>
        /// Choose a container type based on the global settings.
        /// </summary>
        /// <param name="pmt">The placement.</param>
        /// <param name="loc">The location. This will either be a ContainerLocation or an ExistingContainerLocation.</param>
        /// <param name="items">The items at the placement.</param>
        /// <param name="originalChoice">The container type selected by ItemChanger.</param>
        /// <param name="newChoice">The container that would appear if an item
        /// requesting the container from the global settings were prepended.</param>
        /// <returns>The container that should appear.</returns>
        private static string GlobalChoose(
            ISingleCostPlacement pmt,
            AbstractLocation loc,
            List<AbstractItem> items,
            string originalChoice,
            string newChoice)
        {
            if (items.Count <= 1 && !GS.AffectSingleLocations)
            {
                return originalChoice;
            }

            if (newChoice != GS.DefaultContainerType)
            {
                // Requesting the container did not choose the one we wanted, so it is probably not available for this location.
                return originalChoice;
            }

            switch (GS.ReplacementSelectorOption)
            {
                case GlobalSettings.ReplacementSelectorOptions.AnyGiveLate:
                    if (items.Any(item => !item.GiveEarly(originalChoice)))
                    {
                        return newChoice;
                    }
                    else
                    {
                        return originalChoice;
                    }
                case GlobalSettings.ReplacementSelectorOptions.AnyContainerLocation:
                    return newChoice;
                case GlobalSettings.ReplacementSelectorOptions.NoRequestedContainer:
                    if (items.All(item => item.GetPreferredContainer() != originalChoice))
                    {
                        return newChoice;
                    }
                    else
                    {
                        return originalChoice;
                    }
            }

            _logger.LogWarn($"Unhandled placement at {loc.name}");
            return originalChoice;
        }
    }
}
