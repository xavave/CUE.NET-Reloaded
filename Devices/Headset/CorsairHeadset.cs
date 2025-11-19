// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System.Drawing;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Headset.Enums;
using CUE.NET.Native;

namespace CUE.NET.Devices.Headset
{
    /// <summary>
    /// Represents the SDK for a corsair headset.
    /// </summary>
    public class CorsairHeadset : AbstractCueDevice
    {
        #region Properties & Fields

        /// <summary>
        /// Gets specific information provided by CUE for the headset.
        /// </summary>
        public CorsairHeadsetDeviceInfo HeadsetDeviceInfo { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsairHeadset"/> class.
        /// </summary>
        /// <param name="info">The specific information provided by CUE for the headset</param>
        internal CorsairHeadset(CorsairHeadsetDeviceInfo info)
            : base(info)
        {
            this.HeadsetDeviceInfo = info;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the the headset.
        /// </summary>
        public override void Initialize()
        {
            // API 4.x: Get LED positions for this device
            _CorsairLedPosition_V4[] ledPositions = new _CorsairLedPosition_V4[256]; // max LEDs
            int ledCount;
            CorsairError error = _CUESDK.CorsairGetLedPositions(DeviceId, ledPositions.Length, ledPositions, out ledCount);

            if (error == CorsairError.Success)
            {
                for (int i = 0; i < ledCount; i++)
                {
                    _CorsairLedPosition_V4 ledPosition = ledPositions[i];
                    // API 4.x provides center coordinates (cx, cy) - create 1 logical unit rectangle centered on the point
                    InitializeLed(ledPosition.id, new RectangleF((float)(ledPosition.cx - 0.5), (float)(ledPosition.cy - 0.5), 1f, 1f));
                }
            }

            base.Initialize();
        }

        #endregion
    }
}
