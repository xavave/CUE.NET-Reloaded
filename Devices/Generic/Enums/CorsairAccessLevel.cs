// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace CUE.NET.Devices.Generic.Enums
{
    /// <summary>
    /// Contains list of available SDK access levels.
    /// </summary>
    public enum CorsairAccessLevel
    {
        /// <summary>
        /// Shared mode (default).
        /// </summary>
        Shared = 0,

        /// <summary>
        /// Exclusive lightings, but shared events.
        /// </summary>
        ExclusiveLightingControl = 1,

        /// <summary>
        /// Exclusive key events, but shared lightings.
        /// </summary>
        ExclusiveKeyEventsListening = 2,

        /// <summary>
        /// Exclusive mode.
        /// </summary>
        ExclusiveLightingControlAndKeyEventsListening = 3
    }
}
