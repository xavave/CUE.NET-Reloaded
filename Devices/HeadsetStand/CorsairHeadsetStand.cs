// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Exceptions;
using CUE.NET.Native;

namespace CUE.NET.Devices.HeadsetStand
{
    /// <summary>
    /// Represents the SDK for a corsair headset stand.
    /// </summary>
    public class CorsairHeadsetStand : AbstractCueDevice
    {
        #region Properties & Fields

        /// <summary>
        /// Gets specific information provided by CUE for the headset stand.
        /// </summary>
        public CorsairHeadsetStandDeviceInfo HeadsetStandDeviceInfo { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsairHeadsetStand"/> class.
        /// </summary>
        /// <param name="info">The specific information provided by CUE for the headset stand</param>
        internal CorsairHeadsetStand(CorsairHeadsetStandDeviceInfo info)
            : base(info)
        {
            this.HeadsetStandDeviceInfo = info;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the headset stand.
        /// </summary>
        public override void Initialize()
        {
            // API 4.x: Get LED positions for this device
            _CorsairLedPosition_V4[] ledPositions = new _CorsairLedPosition_V4[256]; // max LEDs
            int ledCount;
            CorsairError error = _CUESDK.CorsairGetLedPositions(DeviceId, ledPositions.Length, ledPositions, out ledCount);

            if (error == CorsairError.Success)
            {
                // Sort by LED ID for easy iteration by clients
                for (int i = 0; i < ledCount; i++)
                {
                    _CorsairLedPosition_V4 ledPosition = ledPositions[i];
                    // API 4.x provides center coordinates (cx, cy) - create 1mm x 1mm rectangle centered on the point
                    InitializeLed(ledPosition.id, new RectangleF((float)(ledPosition.cx - 0.5), (float)(ledPosition.cy - 0.5), 1f, 1f));
                }
            }

            base.Initialize();
        }

        #endregion
    }
}
