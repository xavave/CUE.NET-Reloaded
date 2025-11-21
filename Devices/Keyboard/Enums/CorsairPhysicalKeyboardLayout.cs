// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace CUE.NET.Devices.Keyboard.Enums
{
    /// <summary>
    /// Contains list of available physical layouts for keyboards.
    /// </summary>
    public enum CorsairPhysicalKeyboardLayout
    {
        /// <summary>
        /// Invalid/Unknown layout (API 4.x does not provide layout info)
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// US-Keyboard
        /// </summary>
        US = 1,

        /// <summary>
        /// UK-Keyboard
        /// </summary>
        UK = 2,

        /// <summary>
        /// BR-Keyboard
        /// </summary>
        BR = 3,

        /// <summary>
        /// JP-Keyboard
        /// </summary>
        JP = 4,

        /// <summary>
        /// KR-Keyboard
        /// </summary>
        KR = 5,

        /// <summary>
        /// Zone-Layout (e.g. for devices without physical keys like mousepads, etc. if applicable)
        /// </summary>
        Zone = 6
    }
}
