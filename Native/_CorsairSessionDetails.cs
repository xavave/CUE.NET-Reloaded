using System.Runtime.InteropServices;

namespace CUE.NET.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct _CorsairVersion
    {
        internal int major;
        internal int minor;
        internal int patch;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct _CorsairSessionDetails
    {
        internal _CorsairVersion clientVersion;
        internal _CorsairVersion serverVersion;
        internal _CorsairVersion serverHostVersion;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct _CorsairSessionStateChanged
    {
        internal CorsairSessionState state;
        internal _CorsairSessionDetails details;
    }

    internal enum CorsairSessionState
    {
        CSS_Invalid = 0,
        CSS_Closed = 1,
        CSS_Connecting = 2,
        CSS_Timeout = 3,
        CSS_ConnectionRefused = 4,
        CSS_ConnectionLost = 5,
        CSS_Connected = 6
    }
}
