using Satchel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Modding;
using UnityEngine;

namespace ContainerConfig
{
    public class ContainerConfigMod : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
    {
        private static readonly string _version;
        static ContainerConfigMod()
        {
            Assembly asm = typeof(ContainerConfigMod).Assembly;
            string version = asm.GetName().Version.ToString();
            string hash = asm.GetAssemblyHash();
            _version = $"{version}-{hash}";
        }

        internal static ContainerConfigMod instance;

        public static GlobalSettings GS = new();

        public void OnLoadGlobal(GlobalSettings s) => GS = s;
        public GlobalSettings OnSaveGlobal() => GS;

        public bool ToggleButtonInsideMenu => false;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            return ModMenu.GetMenuScreen(modListMenu);
        }


        public ContainerConfigMod() : base("ContainerConfig")
        {
            instance = this;
        }

        public override string GetVersion() => _version;
        
        public override void Initialize()
        {
            Log("Initializing Mod...");

            ItemChanger.Events.BeforeStartNewGame += PlacementPatcher.ConfigurePlacements;
        }
    }
}