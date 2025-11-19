// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Keyboard.Enums;
using CUE.NET.Native;

namespace CUE.NET.Devices.Keyboard
{
    /// <summary>
    /// Represents specific information for a CUE keyboard.
    /// </summary>
    public class CorsairKeyboardDeviceInfo : GenericDeviceInfo
    {
        #region Properties & Fields

        /// <summary>
        /// Gets the physical layout of the keyboard.
        /// </summary>
        public CorsairPhysicalKeyboardLayout PhysicalLayout { get; private set; }

        /// <summary>
        /// Gets the logical layout of the keyboard as set in CUE settings.
        /// </summary>
        public CorsairLogicalKeyboardLayout LogicalLayout { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Internal constructor of managed CorsairDeviceInfo for API 4.x.
        /// </summary>
        /// <param name="nativeInfo">The native CorsairDeviceInfo_V4-struct</param>
        internal CorsairKeyboardDeviceInfo(_CorsairDeviceInfo_V4 nativeInfo)
            : base(nativeInfo)
        {
            // Note: API 4.x doesn't provide layout info in device info
            // These should be queried via CorsairGetDevicePropertyInfo if needed
            this.PhysicalLayout = CorsairPhysicalKeyboardLayout.Invalid;
            this.LogicalLayout = CorsairLogicalKeyboardLayout.Invalid;
        }

        #endregion
    }
}
