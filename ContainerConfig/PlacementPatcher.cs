using ItemChanger;
using ItemChanger.Placements;
using ItemChanger.Tags;
using System.Linq;

namespace ContainerConfig
{
    public static class PlacementPatcher
    {
        private static GlobalSettings GS => ContainerConfigMod.GS;

        public static void ConfigurePlacements()
        {
            if (GS.DefaultContainerType == Container.Unknown) return;

            foreach (AbstractPlacement pmt in ItemChanger.Internal.Ref.Settings.GetPlacements())
            {
                PatchPlacement(pmt);
            }
        }

        public static void PatchPlacement(AbstractPlacement pmt)
        {
            if (pmt.Items.Count == 0) return;

            if (pmt is ISingleCostPlacement scpmt && scpmt.Cost != null)
            {
                // Has a cost, so will be a shiny. No need to do anything
                return;
            }

            string currentContainer;

            if (pmt is MutablePlacement mpmt)
            {
                if (mpmt.Location.forceShiny)
                {
                    // Force shiny, so no need to do anything
                    return;
                }

                currentContainer = MutablePlacement.ChooseContainerType(mpmt, mpmt.Location, mpmt.Items);
            }
            else if (pmt is ExistingContainerPlacement epmt)
            {
                if (epmt.Location.nonreplaceable)
                {
                    // Nonreplaceable, so no need to do anything
                    return;
                }

                currentContainer = ExistingContainerPlacement.ChooseContainerType(epmt, epmt.Location, epmt.Items);
            }
            else
            {
                // Not a patchable placement type
                return;
            }

            if (currentContainer == GS.DefaultContainerType)
            {
                // No need to change the container type because it's already what we want
                return;
            }

            if (!ShouldModify(pmt, currentContainer)) return;

            DoModify(pmt, GS.DefaultContainerType);
        }


        /// <summary>
        /// Modifies the current placement to ensure that the container is the container type.
        /// It can be assumed that <paramref name="containerType"/> is not null or Unknown.
        /// </summary>
        public static void DoModify(AbstractPlacement pmt, string containerType)
        {
            // Exclude chests specifically, because they're handled specially by IC.
            if (containerType != Container.Chest)
            {
                pmt.AddTag<UnsupportedContainerTag>().containerType = Container.Chest;
            }

            if (containerType == Container.Shiny && pmt is MutablePlacement mpmt)
            {
                mpmt.Location.forceShiny = true;
            }
            else
            {
                pmt.Items.Insert(0, IC.EmptyItem.Create(containerType));
            }
        }



        /// <summary>
        /// Checks the global settings to see if the placement is a candidate for modification
        /// </summary>
        public static bool ShouldModify(AbstractPlacement pmt, string currentContainer)
        {
            if (!GS.AffectSingleLocations && pmt.Items.Count <= 1)
            {
                return false;
            }

            if (!GS.ForceReplaceECLs
                && pmt is ExistingContainerPlacement epmt 
                && currentContainer == epmt.Location.containerType
                // If there is an item requesting the current container, it is probably being "replaced"
                && pmt.Items.All(item => item.GetPreferredContainer() != currentContainer))
            {
                // Not replaced
                return false;
            }

            switch (GS.ReplacementSelectorOption)
            {
                case GlobalSettings.ReplacementSelectorOptions.NoRequestedContainer:
                    return pmt.Items.All(item => item.GetPreferredContainer() == null || item.GetPreferredContainer() == Container.Unknown);

                case GlobalSettings.ReplacementSelectorOptions.AnyGiveLate:
                    return pmt.Items.Any(item => !item.GiveEarly(currentContainer));

                case GlobalSettings.ReplacementSelectorOptions.AnyContainerLocation:
                    return true;


                default:
                    return false;
            }
        }
    }
}
