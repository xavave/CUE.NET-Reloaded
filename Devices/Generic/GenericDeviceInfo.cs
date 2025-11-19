using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Native;

namespace CUE.NET.Devices.Generic
{
    /// <summary>
    /// Represents generic information about a CUE device.
    /// </summary>
    public class GenericDeviceInfo : IDeviceInfo
    {
        #region Properties & Fields

        /// <summary>
        /// Gets the device type. (<see cref="CUE.NET.Devices.Generic.Enums.CorsairDeviceType" />)
        /// </summary>
        public CorsairDeviceType Type { get; }

        /// <summary>
        /// Gets the unique device identifier.
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// Gets the device serial number. Can be empty if not available.
        /// </summary>
        public string Serial { get; }

        /// <summary>
        /// Gets the device model (like "K95RGB").
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Gets the number of controllable LEDs on the device.
        /// </summary>
        public int LedCount { get; }

        /// <summary>
        /// Gets the number of channels controlled by the device.
        /// </summary>
        public int ChannelCount { get; }

        /// <summary>
        /// Get a flag that describes device capabilities. Always has Lighting capability in API 4.x.
        /// </summary>
        public CorsairDeviceCaps CapsMask => CorsairDeviceCaps.Lighting;

        #endregion

        #region Constructors

        /// <summary>
        /// Internal constructor of managed <see cref="GenericDeviceInfo"/> from API 4.x native struct.
        /// </summary>
        /// <param name="nativeInfo">The native <see cref="_CorsairDeviceInfo_V4" />-struct</param>
        internal GenericDeviceInfo(_CorsairDeviceInfo_V4 nativeInfo)
        {
            Type = nativeInfo.type;
            DeviceId = nativeInfo.id;
            Serial = nativeInfo.serial;
            Model = nativeInfo.model;
            LedCount = nativeInfo.ledCount;
            ChannelCount = nativeInfo.channelCount;
        }

        #endregion
    }
}
