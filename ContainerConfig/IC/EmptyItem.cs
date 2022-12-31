using ItemChanger;
using ItemChanger.Tags;
using Modding;
using System;
using UnityEngine;

namespace ContainerConfig.IC
{
    public class EmptyUIDef : UIDef
    {
        public override void SendMessage(MessageType type, Action callback = null) => callback?.Invoke();
        public override string GetPreviewName()
        {
            ContainerConfigMod.instance.LogWarn("Empty UIDef preview requested");

            return string.Empty;
        }

        public override string GetPostviewName()
        {
            ContainerConfigMod.instance.LogWarn("Empty UIDef postview requested");

            return string.Empty;
        }

        public override string GetShopDesc()
        {
            ContainerConfigMod.instance.LogWarn("Empty UIDef shop desc requested");

            return string.Empty;
        }

        public override Sprite GetSprite()
        {
            ContainerConfigMod.instance.LogWarn("Empty UIDef sprite requested");

            return CanvasUtil.NullSprite();
        }
    }

    /// <summary>
    /// Item that has no effect other than to request a particular container type.
    /// It is expected to use <see cref="EmptyItem.Create(string container)"/> to create an empty item.
    /// </summary>
    public class EmptyItem : AbstractItem
    {
        public string PreferredContainer { get; set; }

        public override void GiveImmediate(GiveInfo info) { }
        public override bool GiveEarly(string containerType) => true;

        public override string GetPreferredContainer() => PreferredContainer;

        public static EmptyItem Create(string container)
        {
            EmptyItem item = new() 
            { 
                PreferredContainer = container,
                UIDef = new EmptyUIDef(),
                name = $"Force_Container-{container}"
            };

            item.AddTag<CompletionWeightTag>().Weight = 0;

            // Hide from recent items
            InteropTag tag = item.AddTag<InteropTag>();
            tag.Message = "RecentItems";
            tag.Properties.Add("IgnoreItem", true);

            return item;
        }

    }
}
