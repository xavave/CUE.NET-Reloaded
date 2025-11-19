// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Native;

namespace CUE.NET.Devices.Keyboard
{
    /// <summary>
    /// Represents the SDK for a corsair keyboard.
    /// </summary>
    public class CorsairKeyboard : AbstractCueDevice
    {
        #region Properties & Fields

        /// <summary>
        /// Gets specific information provided by CUE for the keyboard.
        /// </summary>
        public CorsairKeyboardDeviceInfo KeyboardDeviceInfo { get; }

        #region Indexers

        /// <summary>
        /// Gets the <see cref="CorsairLed" /> representing the given character by calling the SDK-method 'CorsairGetLedLuidForKeyName'.<br />
        /// Note that this currently only works for letters.
        /// </summary>
        /// <param name="key">The character of the key.</param>
        /// <returns>The led representing the given character or null if no led is found.</returns>
        public CorsairLed this[char key]
        {
            get
            {
                uint ledLuid;
                CorsairError error = _CUESDK.CorsairGetLedLuidForKeyName(DeviceId, key, out ledLuid);
                if (error != CorsairError.Success)
                    return null;

                CorsairLed led;
                return LedMapping.TryGetValue(ledLuid, out led) ? led : null;
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsairKeyboard"/> class.
        /// </summary>
        /// <param name="info">The specific information provided by CUE for the keyboard</param>
        internal CorsairKeyboard(CorsairKeyboardDeviceInfo info)
            : base(info)
        {
            this.KeyboardDeviceInfo = info;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the keyboard.
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
                    // API 4.x provides center coordinates (cx, cy) - create 1mm x 1mm rectangle centered on the point
                    InitializeLed(ledPosition.id, new RectangleF((float)(ledPosition.cx - 0.5), (float)(ledPosition.cy - 0.5), 1f, 1f));
                }
            }

            base.Initialize();
        }

        #endregion
    }
}
