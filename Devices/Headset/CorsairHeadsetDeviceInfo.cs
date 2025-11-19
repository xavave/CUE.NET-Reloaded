using CUE.NET.Devices.Generic;
using CUE.NET.Native;

namespace CUE.NET.Devices.Headset
{
    /// <summary>
    /// Represents specific information for a CUE headset.
    /// </summary>
    public class CorsairHeadsetDeviceInfo : GenericDeviceInfo
    {
        #region Constructors

        /// <summary>
        /// Internal constructor of managed <see cref="CorsairHeadsetDeviceInfo" /> for API 4.x.
        /// </summary>
        /// <param name="nativeInfo">The native <see cref="_CorsairDeviceInfo_V4" />-struct</param>
        internal CorsairHeadsetDeviceInfo(_CorsairDeviceInfo_V4 nativeInfo)
            : base(nativeInfo)
        { }

        #endregion
    }
}
