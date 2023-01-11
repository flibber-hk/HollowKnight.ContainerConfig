using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ContainerConfig
{
    public record GlobalSettings
    {
        /// <summary>
        /// Force locations to have this container where possible.
        /// </summary>
        public string DefaultContainerType { get; init; } = ItemChanger.Container.Unknown;

        /// <summary>
        /// If false, only apply to locations with more than one item.
        /// </summary>
        public bool AffectSingleLocations { get; init; } = false;

        public enum ReplacementSelectorOptions
        {
            /// <summary>
            /// If no item at the location requests a container, then apply the selected container type.
            /// </summary>
            NoRequestedContainer,
            /// <summary>
            /// If any item at the location does not GiveEarly the default container, then replace it.
            /// </summary>
            AnyGiveLate,
            /// <summary>
            /// Always replace where possible.
            /// </summary>
            AnyContainerLocation,
        }

        /// <summary>
        /// Used to decide whether to replace.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))] public ReplacementSelectorOptions ReplacementSelectorOption { get; init; }
    }
}
