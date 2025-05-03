using Monocle;
using System;

namespace Celeste.Mod.VBC2
{
    /// <summary>
    /// The main mod class
    /// </summary>
    public class VBC2Module : EverestModule 
    {
        // We can only have one instance of this mod alive at any given time.
        public static VBC2Module Instance { get; private set; }

        // Define the type in which global settings will be stored
        public override Type SettingsType => typeof(VBC2ModuleSettings);
        /// <summary>
        /// The global mod settings for VBC2.
        /// </summary>
        public static VBC2ModuleSettings Settings => (VBC2ModuleSettings) Instance._Settings;

        // Define the type in which per-session data will be stored
        public override Type SessionType => typeof(VBC2ModuleSession);
        /// <summary>
        /// The per-session data for VBC2.
        /// </summary>
        public static VBC2ModuleSession Session => (VBC2ModuleSession) Instance._Session;

        // Define the type in which the mod's save data will be stored
        public override Type SaveDataType => typeof(VBC2ModuleSaveData);
        /// <summary>
        /// The save data for VBC2.
        /// </summary>
        public static VBC2ModuleSaveData SaveData => (VBC2ModuleSaveData) Instance._SaveData;

        public SpriteBank PandaSpriteBank;
        public SpriteBank SampleSpriteBank;

        public VBC2Module() 
        {
            Instance = this;
    #if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(VBC2Module), LogLevel.Verbose);
    #else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(VBC2Module), LogLevel.Info);
    #endif
        }

        public override void LoadContent(bool firstLoad)
        {
            SampleSpriteBank = new(GFX.Game, "Graphics/Sprites.xml");
            PandaSpriteBank = new(GFX.Game, "Graphics/VBC2xmls/Panda/Sprites.xml");
        }

        public override void Load() {
            // TODO: apply any hooks that should always be active
            
        }

        public override void Unload() {
            // TODO: unapply any hooks applied in Load()
        }
    }
}