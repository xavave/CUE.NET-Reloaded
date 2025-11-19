// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using CUE.NET.Devices.Generic;
using CUE.NET.Native;

namespace CUE.NET.Devices.Mousemat
{
    /// <summary>
    /// Represents specific information for a CUE Mousemat.
    /// </summary>
    public class CorsairMousematDeviceInfo : GenericDeviceInfo
    {
        #region Constructors

        /// <summary>
        /// Internal constructor of managed <see cref="CorsairMousematDeviceInfo" /> for API 4.x.
        /// </summary>
        /// <param name="nativeInfo">The native <see cref="_CorsairDeviceInfo_V4" />-struct</param>
        internal CorsairMousematDeviceInfo(_CorsairDeviceInfo_V4 nativeInfo)
            : base(nativeInfo)
        { }

        #endregion
    }
}