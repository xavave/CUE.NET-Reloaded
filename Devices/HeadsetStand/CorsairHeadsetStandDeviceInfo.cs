// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using CUE.NET.Devices.Generic;
using CUE.NET.Native;

namespace CUE.NET.Devices.HeadsetStand
{
    /// <summary>
    /// Represents specific information for a CUE headset stand.
    /// </summary>
    public class CorsairHeadsetStandDeviceInfo : GenericDeviceInfo
    {
        #region Constructors

        /// <summary>
        /// Internal constructor of managed <see cref="CorsairHeadsetStandDeviceInfo" /> for API 4.x.
        /// </summary>
        /// <param name="nativeInfo">The native <see cref="_CorsairDeviceInfo_V4" />-struct</param>
        internal CorsairHeadsetStandDeviceInfo(_CorsairDeviceInfo_V4 nativeInfo)
            : base(nativeInfo)
        { }

        #endregion
    }
}