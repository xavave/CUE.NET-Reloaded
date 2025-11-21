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
        Keyboard = 0x0001,
        Mouse = 0x0002,
        Mousemat = 0x0004,
        Headset = 0x0008,
        HeadsetStand = 0x0010,
        FanLedController = 0x0020,
        LedController = 0x0040,
        MemoryModule = 0x0080,
        Cooler = 0x0100,
        Motherboard = 0x0200,
        GraphicsCard = 0x0400,
        Touchbar = 0x0800,
        GameController = 0x1000,
        // API 4.x filter value to get all device types
        All = unchecked((int)0xFFFFFFFF)
    };
}
