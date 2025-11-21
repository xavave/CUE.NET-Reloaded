using System;
using System.Runtime.InteropServices;
using CUE.NET.Devices.Generic.Enums;

namespace CUE.NET.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CorsairDataType_BooleanArray
    {
        internal IntPtr items; // bool*
        internal uint count;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CorsairDataType_Int32Array
    {
        internal IntPtr items; // int*
        internal uint count;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CorsairDataType_Float64Array
    {
        internal IntPtr items; // double*
        internal uint count;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CorsairDataType_StringArray
    {
        internal IntPtr items; // char**
        internal uint count;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct CorsairDataValue
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.I1)]
        internal bool boolean;

        [FieldOffset(0)]
        internal int int32;

        [FieldOffset(0)]
        internal double float64;

        [FieldOffset(0)]
        internal IntPtr @string; // char*

        [FieldOffset(0)]
        internal CorsairDataType_BooleanArray boolean_array;

        [FieldOffset(0)]
        internal CorsairDataType_Int32Array int32_array;

        [FieldOffset(0)]
        internal CorsairDataType_Float64Array float64_array;

        [FieldOffset(0)]
        internal CorsairDataType_StringArray string_array;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CorsairProperty
    {
        internal CorsairDataType type;
        internal CorsairDataValue value;
    }
}
