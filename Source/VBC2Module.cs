using System;

namespace Celeste.Mod.VBC2;

public class VBC2Module : EverestModule {
    public static VBC2Module Instance { get; private set; }

    public override Type SettingsType => typeof(VBC2ModuleSettings);
    public static VBC2ModuleSettings Settings => (VBC2ModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(VBC2ModuleSession);
    public static VBC2ModuleSession Session => (VBC2ModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(VBC2ModuleSaveData);
    public static VBC2ModuleSaveData SaveData => (VBC2ModuleSaveData) Instance._SaveData;

    public VBC2Module() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(VBC2Module), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(VBC2Module), LogLevel.Info);
#endif
    }

    public override void Load() {
        // TODO: apply any hooks that should always be active
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }
}