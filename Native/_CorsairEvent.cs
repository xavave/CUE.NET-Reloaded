using System;
using System.Runtime.InteropServices;
using CUE.NET.Devices.Generic.Enums;

namespace CUE.NET.Native
{
    internal enum CorsairEventId
    {
        Invalid = 0,
        DeviceConnectionStatusChangedEvent = 1,
        KeyEvent = 2
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct CorsairKeyEvent
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string deviceId;
        internal int keyId; // CorsairMacroKeyId
        [MarshalAs(UnmanagedType.I1)]
        internal bool isPressed;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct CorsairDeviceConnectionStatusChangedEvent
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string deviceId;
        [MarshalAs(UnmanagedType.I1)]
        internal bool isConnected;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CorsairEvent
    {
        internal CorsairEventId id;
        internal IntPtr data; // Union of pointers
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CorsairKeyEventConfiguration
    {
        internal int keyId; // CorsairMacroKeyId
        [MarshalAs(UnmanagedType.I1)]
        internal bool isIntercepted;
    }
}
