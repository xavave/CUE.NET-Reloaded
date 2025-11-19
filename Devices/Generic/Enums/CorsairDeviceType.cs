// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

namespace CUE.NET.Devices.Generic.Enums
{
    /// <summary>
    /// Contains list of available device types.
    /// </summary>
    public enum CorsairDeviceType
    {
        Unknown = 0x0000,
        Mouse = 0x0001,
        Keyboard = 0x0002,
        Headset = 0x0008,
        Mousemat = 0x0004,
        HeadsetStand = 0x0010,
        // API 4.x filter value to get all device types
        All = unchecked((int)0xFFFFFFFF)
    };
}
