using System;

namespace LibreR.Models.Enums {
    /// <summary>
    /// Enumerates the possible key modifiers.
    /// </summary>
    [Flags]
    public enum Modifier {
        /// <summary>
        /// No key modifier.
        /// </summary>
        NoMod = 0x0000,
        /// <summary>
        /// Alt key modifier.
        /// </summary>
        Alt = 0x0001,
        /// <summary>
        /// Control key modifier.
        /// </summary>
        Ctrl = 0x0002,
        /// <summary>
        /// Shift key modifier.
        /// </summary>
        Shift = 0x0004,
        /// <summary>
        /// Windows key modifier.
        /// </summary>
        Win = 0x0008
    }
}
