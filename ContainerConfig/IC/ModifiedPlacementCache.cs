using ItemChanger;
using ItemChanger.Modules;
using ItemChanger.Placements;
using Modding;
using System.Collections.Generic;

namespace ContainerConfig.IC
{
    public class ModifiedPlacementCache : Module
    {
        private static readonly ILogger _logger = new SimpleLogger("ContainerConfig.ModifiedPlacementCache");

        public readonly HashSet<string> ModifiedPlacements = new();

        public override void Initialize() 
        {
            ModMenu.RecalculateClicked += RecalculateContainerTypes;
        }
        public override void Unload() 
        {
            ModMenu.RecalculateClicked -= RecalculateContainerTypes;
        }

        /// <summary>
        /// For each container-like placement, invalidate and recalculate the cached selected container.
        /// </summary>
        public void RecalculateContainerTypes()
        {
            foreach (AbstractPlacement pmt in ItemChanger.Internal.Ref.Settings.GetPlacements())
            {
                if (!ModifiedPlacements.Contains(pmt.Name)) continue;

                if (pmt is ExistingContainerPlacement epmt)
                {
                    ReflectionHelper.SetField<ExistingContainerPlacement, string>(epmt, "currentContainerType", Container.Unknown);
                    ReflectionHelper.CallMethod<ExistingContainerPlacement>(epmt, "UpdateContainerType");
                }
                else if (pmt is MutablePlacement mpmt)
                {
                    mpmt.containerType = Container.Unknown;
                }
                else if (pmt is DualPlacement dpmt)
                {
                    dpmt.containerType = Container.Unknown;
                    ReflectionHelper.CallMethod<DualPlacement>(dpmt, "SetContainerType");
                }

                else
                {
                    _logger.LogWarn($"Unrecognised placement {pmt.Name} of type {pmt.GetType().Name}");
                }
            }
        }
    }
}
