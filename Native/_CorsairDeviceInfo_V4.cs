using System.Runtime.InteropServices;
using CUE.NET.Devices.Generic.Enums;

namespace CUE.NET.Native
{
    // Device ID is a string of max 128 characters
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct _CorsairDeviceId
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string id;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct _CorsairDeviceInfo_V4
    {
        internal CorsairDeviceType type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string serial;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string model;

        internal int ledCount;
        internal int channelCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct _CorsairDeviceFilter
    {
        internal int deviceTypeMask;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct _CorsairLedPosition_V4
    {
        internal uint id; // CorsairLedLuid
        internal double cx;
        internal double cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct _CorsairLedColor_V4
    {
        internal uint id; // CorsairLedLuid
        internal byte r;
        internal byte g;
        internal byte b;
        internal byte a; // alpha channel
    }
}
