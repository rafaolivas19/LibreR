namespace LibreR.Models.Enums {
    /// <summary>
    /// Represents the possible serialization types.
    /// </summary>
    public enum Serializer {
        /// <summary>
        /// Pretty format, using line jumps and indentation.
        /// </summary>
        PrettyFormat,
        /// <summary>
        /// Pretty format, using line jumps, indentation and including null values.
        /// </summary>
        PrettyFormatWithNullValues,
        /// <summary>
        /// Shortest format, no line jumps or indentation.
        /// </summary>
        OneLine,
        /// <summary>
        /// Shortest format, no line jumps or indentation. Includes null values.
        /// </summary>
        OneLineWithNullValues
    }
}
