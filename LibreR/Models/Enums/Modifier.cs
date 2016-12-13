using System;

namespace LibreR.Models.Enums {
    [Flags]
    public enum Modifier {
        NoMod = 0x0000,
        Alt = 0x0001,
        Ctrl = 0x0002,
        Shift = 0x0004,
        Win = 0x0008
    }
}
