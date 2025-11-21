// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Exceptions;

namespace CUE.NET.Native
{
    // ReSharper disable once InconsistentNaming
    internal static class _CUESDK
    {
        #region Library Management

        private static IntPtr _dllHandle = IntPtr.Zero;

        /// <summary>
        /// Gets the loaded architecture (x64/x86).
        /// </summary>
        internal static string LoadedArchitecture { get; private set; }

        /// <summary>
        /// Reloads the SDK.
        /// </summary>
        internal static void Reload()
        {
            UnloadCUESDK();
            LoadCUESDK();
        }

        private static void LoadCUESDK()
        {
            if (_dllHandle != IntPtr.Zero) return;

            LoadedArchitecture = Environment.Is64BitProcess ? "x64" : "x86";

            // HACK: Load library at runtime to support both, x86 and x64 with one managed dll
            List<string> possiblePathList = Environment.Is64BitProcess ? CueSDK.PossibleX64NativePaths : CueSDK.PossibleX86NativePaths;
            string dllPath = null;
            foreach (string path in possiblePathList)
                if (File.Exists(path))
                {
                    dllPath = path;
                    break;
                }

            if (dllPath == null) throw new WrapperException($"Can't find the iCUE-SDK at one of the expected locations:\r\n '{string.Join("\r\n", possiblePathList.Select(Path.GetFullPath))}'");

            _dllHandle = LoadLibrary(dllPath);

            // Load API 4.x function pointers
            _corsairConnectPointer = (CorsairConnectPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairConnect"), typeof(CorsairConnectPointer));
            _corsairGetSessionDetailsPointer = (CorsairGetSessionDetailsPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairGetSessionDetails"), typeof(CorsairGetSessionDetailsPointer));
            _corsairDisconnectPointer = (CorsairDisconnectPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairDisconnect"), typeof(CorsairDisconnectPointer));
            _corsairGetDevicesPointer = (CorsairGetDevicesPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairGetDevices"), typeof(CorsairGetDevicesPointer));
            _corsairGetDeviceInfoPointer = (CorsairGetDeviceInfoPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairGetDeviceInfo"), typeof(CorsairGetDeviceInfoPointer));
            _corsairGetLedPositionsPointer = (CorsairGetLedPositionsPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairGetLedPositions"), typeof(CorsairGetLedPositionsPointer));
            _corsairSetLedColorsPointer = (CorsairSetLedColorsPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairSetLedColors"), typeof(CorsairSetLedColorsPointer));
            _corsairGetLedColorsPointer = (CorsairGetLedColorsPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairGetLedColors"), typeof(CorsairGetLedColorsPointer));
            _corsairSetLayerPriorityPointer = (CorsairSetLayerPriorityPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairSetLayerPriority"), typeof(CorsairSetLayerPriorityPointer));
            _corsairGetLedLuidForKeyNamePointer = (CorsairGetLedLuidForKeyNamePointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairGetLedLuidForKeyName"), typeof(CorsairGetLedLuidForKeyNamePointer));
            _corsairRequestControlPointer = (CorsairRequestControlPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairRequestControl"), typeof(CorsairRequestControlPointer));
            _corsairReleaseControlPointer = (CorsairReleaseControlPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairReleaseControl"), typeof(CorsairReleaseControlPointer));
            _corsairSubscribeForEventsPointer = (CorsairSubscribeForEventsPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairSubscribeForEvents"), typeof(CorsairSubscribeForEventsPointer));
            _corsairUnsubscribeFromEventsPointer = (CorsairUnsubscribeFromEventsPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairUnsubscribeFromEvents"), typeof(CorsairUnsubscribeFromEventsPointer));
            _corsairConfigureKeyEventPointer = (CorsairConfigureKeyEventPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairConfigureKeyEvent"), typeof(CorsairConfigureKeyEventPointer));
            _corsairGetDevicePropertyInfoPointer = (CorsairGetDevicePropertyInfoPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairGetDevicePropertyInfo"), typeof(CorsairGetDevicePropertyInfoPointer));
            _corsairReadDevicePropertyPointer = (CorsairReadDevicePropertyPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairReadDeviceProperty"), typeof(CorsairReadDevicePropertyPointer));
            _corsairWriteDevicePropertyPointer = (CorsairWriteDevicePropertyPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairWriteDeviceProperty"), typeof(CorsairWriteDevicePropertyPointer));
            _corsairFreePropertyPointer = (CorsairFreePropertyPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairFreeProperty"), typeof(CorsairFreePropertyPointer));
            _corsairSetLedColorsBufferPointer = (CorsairSetLedColorsBufferPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairSetLedColorsBuffer"), typeof(CorsairSetLedColorsBufferPointer));
            _corsairSetLedColorsFlushBufferAsyncPointer = (CorsairSetLedColorsFlushBufferAsyncPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "CorsairSetLedColorsFlushBufferAsync"), typeof(CorsairSetLedColorsFlushBufferAsyncPointer));
        }

        private static void UnloadCUESDK()
        {
            if (_dllHandle == IntPtr.Zero) return;

            // API 4.x: Disconnect before unloading to avoid crashes with active callbacks
            try
            {
                if (_corsairDisconnectPointer != null)
                    _corsairDisconnectPointer();
            }
            catch
            {
                // Ignore errors during disconnect - we're unloading anyway
            }

            // Free the library once - Windows will handle reference counting
            FreeLibrary(_dllHandle);
            _dllHandle = IntPtr.Zero;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr dllHandle);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr dllHandle, string name);

        #endregion

        #region SDK-METHODS

        #region Pointers

        private static CorsairConnectPointer _corsairConnectPointer;
        private static CorsairGetSessionDetailsPointer _corsairGetSessionDetailsPointer;
        private static CorsairDisconnectPointer _corsairDisconnectPointer;
        private static CorsairGetDevicesPointer _corsairGetDevicesPointer;
        private static CorsairGetDeviceInfoPointer _corsairGetDeviceInfoPointer;
        private static CorsairGetLedPositionsPointer _corsairGetLedPositionsPointer;
        private static CorsairSetLedColorsPointer _corsairSetLedColorsPointer;
        private static CorsairGetLedColorsPointer _corsairGetLedColorsPointer;
        private static CorsairSetLayerPriorityPointer _corsairSetLayerPriorityPointer;
        private static CorsairGetLedLuidForKeyNamePointer _corsairGetLedLuidForKeyNamePointer;
        private static CorsairRequestControlPointer _corsairRequestControlPointer;
        private static CorsairReleaseControlPointer _corsairReleaseControlPointer;
        private static CorsairSubscribeForEventsPointer _corsairSubscribeForEventsPointer;
        private static CorsairUnsubscribeFromEventsPointer _corsairUnsubscribeFromEventsPointer;
        private static CorsairConfigureKeyEventPointer _corsairConfigureKeyEventPointer;
        private static CorsairGetDevicePropertyInfoPointer _corsairGetDevicePropertyInfoPointer;
        private static CorsairReadDevicePropertyPointer _corsairReadDevicePropertyPointer;
        private static CorsairWriteDevicePropertyPointer _corsairWriteDevicePropertyPointer;
        private static CorsairFreePropertyPointer _corsairFreePropertyPointer;
        private static CorsairSetLedColorsBufferPointer _corsairSetLedColorsBufferPointer;
        private static CorsairSetLedColorsFlushBufferAsyncPointer _corsairSetLedColorsFlushBufferAsyncPointer;

        #endregion

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void CorsairSessionStateChangedHandler(IntPtr context, IntPtr eventData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void CorsairEventHandler(IntPtr context, IntPtr eventData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairConnectPointer(CorsairSessionStateChangedHandler onStateChanged, IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairGetSessionDetailsPointer(out _CorsairSessionDetails details);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairDisconnectPointer();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairGetDevicesPointer(ref _CorsairDeviceFilter filter, int sizeMax, [In, Out] _CorsairDeviceInfo_V4[] devices, out int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairGetDeviceInfoPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, out _CorsairDeviceInfo_V4 deviceInfo);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairGetLedPositionsPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, int sizeMax, [In, Out] _CorsairLedPosition_V4[] ledPositions, out int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairSetLedColorsPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, int size, [In] _CorsairLedColor_V4[] ledColors);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairGetLedColorsPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, int size, [In, Out] _CorsairLedColor_V4[] ledColors);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairSetLayerPriorityPointer(uint priority);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairGetLedLuidForKeyNamePointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, char keyName, out uint ledId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairRequestControlPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, CorsairAccessLevel accessLevel);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairReleaseControlPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairSubscribeForEventsPointer(CorsairEventHandler onEvent, IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairUnsubscribeFromEventsPointer();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairConfigureKeyEventPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, ref CorsairKeyEventConfiguration config);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairGetDevicePropertyInfoPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, CorsairDevicePropertyId propertyId, uint index, out CorsairDataType dataType, out uint flags);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairReadDevicePropertyPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, CorsairDevicePropertyId propertyId, uint index, out CorsairProperty property);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairWriteDevicePropertyPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, CorsairDevicePropertyId propertyId, uint index, ref CorsairProperty property);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairFreePropertyPointer(ref CorsairProperty property);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairSetLedColorsBufferPointer([MarshalAs(UnmanagedType.LPStr)] string deviceId, int size, [In] _CorsairLedColor_V4[] ledColors);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void CorsairAsyncCallback(IntPtr context, CorsairError error);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate CorsairError CorsairSetLedColorsFlushBufferAsyncPointer(CorsairAsyncCallback callback, IntPtr context);

        #endregion

        // ReSharper disable EventExceptionNotDocumented

        /// <summary>
        /// iCUE-SDK: Sets handler for session state changes, checks versions of SDK client, server and host (iCUE).
        /// </summary>
        internal static CorsairError CorsairConnect(CorsairSessionStateChangedHandler onStateChanged, IntPtr context)
        {
            return _corsairConnectPointer(onStateChanged, context);
        }

        /// <summary>
        /// iCUE-SDK: Checks versions of SDK client, server and host (iCUE).
        /// </summary>
        internal static CorsairError CorsairGetSessionDetails(out _CorsairSessionDetails details)
        {
            return _corsairGetSessionDetailsPointer(out details);
        }

        /// <summary>
        /// iCUE-SDK: Removes handler for session state changes previously set by CorsairConnect.
        /// </summary>
        internal static CorsairError CorsairDisconnect()
        {
            return _corsairDisconnectPointer();
        }

        /// <summary>
        /// iCUE-SDK: Populates the buffer with filtered collection of devices.
        /// </summary>
        internal static CorsairError CorsairGetDevices(ref _CorsairDeviceFilter filter, int sizeMax, _CorsairDeviceInfo_V4[] devices, out int size)
        {
            return _corsairGetDevicesPointer(ref filter, sizeMax, devices, out size);
        }

        /// <summary>
        /// iCUE-SDK: Gets information about device specified by deviceId.
        /// </summary>
        internal static CorsairError CorsairGetDeviceInfo(string deviceId, out _CorsairDeviceInfo_V4 deviceInfo)
        {
            return _corsairGetDeviceInfoPointer(deviceId, out deviceInfo);
        }

        /// <summary>
        /// iCUE-SDK: Provides a list of supported device LEDs by its id with their positions.
        /// </summary>
        internal static CorsairError CorsairGetLedPositions(string deviceId, int sizeMax, _CorsairLedPosition_V4[] ledPositions, out int size)
        {
            return _corsairGetLedPositionsPointer(deviceId, sizeMax, ledPositions, out size);
        }

        /// <summary>
        /// iCUE-SDK: Sets specified LEDs to some colors.
        /// </summary>
        internal static CorsairError CorsairSetLedColors(string deviceId, int size, _CorsairLedColor_V4[] ledColors)
        {
            return _corsairSetLedColorsPointer(deviceId, size, ledColors);
        }

        /// <summary>
        /// iCUE-SDK: Get current color for the list of requested LEDs.
        /// </summary>
        internal static CorsairError CorsairGetLedColors(string deviceId, int size, _CorsairLedColor_V4[] ledColors)
        {
            return _corsairGetLedColorsPointer(deviceId, size, ledColors);
        }

        /// <summary>
        /// iCUE-SDK: Set layer priority for this shared client.
        /// </summary>
        internal static CorsairError CorsairSetLayerPriority(uint priority)
        {
            return _corsairSetLayerPriorityPointer(priority);
        }

        /// <summary>
        /// iCUE-SDK: Retrieves LED luid for key name taking logical layout into account.
        /// </summary>
        internal static CorsairError CorsairGetLedLuidForKeyName(string deviceId, char keyName, out uint ledId)
        {
            return _corsairGetLedLuidForKeyNamePointer(deviceId, keyName, out ledId);
        }

        /// <summary>
        /// iCUE-SDK: Requests control using specified access level.
        /// </summary>
        internal static CorsairError CorsairRequestControl(string deviceId, CorsairAccessLevel accessLevel)
        {
            return _corsairRequestControlPointer(deviceId, accessLevel);
        }

        /// <summary>
        /// iCUE-SDK: Releases previously requested control for specified device.
        /// </summary>
        internal static CorsairError CorsairReleaseControl(string deviceId)
        {
            return _corsairReleaseControlPointer(deviceId);
        }

        /// <summary>
        /// iCUE-SDK: Registers a callback that will be called by SDK when some event happened.
        /// </summary>
        internal static CorsairError CorsairSubscribeForEvents(CorsairEventHandler onEvent, IntPtr context)
        {
            return _corsairSubscribeForEventsPointer(onEvent, context);
        }

        /// <summary>
        /// iCUE-SDK: Unregisters callback previously registered by CorsairSubscribeForEvents call.
        /// </summary>
        internal static CorsairError CorsairUnsubscribeFromEvents()
        {
            return _corsairUnsubscribeFromEventsPointer();
        }

        /// <summary>
        /// iCUE-SDK: Configures key event behavior.
        /// </summary>
        internal static CorsairError CorsairConfigureKeyEvent(string deviceId, ref CorsairKeyEventConfiguration config)
        {
            return _corsairConfigureKeyEventPointer(deviceId, ref config);
        }

        /// <summary>
        /// iCUE-SDK: Gets information about device property.
        /// </summary>
        internal static CorsairError CorsairGetDevicePropertyInfo(string deviceId, CorsairDevicePropertyId propertyId, uint index, out CorsairDataType dataType, out uint flags)
        {
            return _corsairGetDevicePropertyInfoPointer(deviceId, propertyId, index, out dataType, out flags);
        }

        /// <summary>
        /// iCUE-SDK: Reads device property.
        /// </summary>
        internal static CorsairError CorsairReadDeviceProperty(string deviceId, CorsairDevicePropertyId propertyId, uint index, out CorsairProperty property)
        {
            return _corsairReadDevicePropertyPointer(deviceId, propertyId, index, out property);
        }

        /// <summary>
        /// iCUE-SDK: Writes device property.
        /// </summary>
        internal static CorsairError CorsairWriteDeviceProperty(string deviceId, CorsairDevicePropertyId propertyId, uint index, ref CorsairProperty property)
        {
            return _corsairWriteDevicePropertyPointer(deviceId, propertyId, index, ref property);
        }

        /// <summary>
        /// iCUE-SDK: Frees property memory.
        /// </summary>
        internal static CorsairError CorsairFreeProperty(ref CorsairProperty property)
        {
            return _corsairFreePropertyPointer(ref property);
        }

        /// <summary>
        /// iCUE-SDK: Sets specified LEDs to some colors in the buffer.
        /// </summary>
        internal static CorsairError CorsairSetLedColorsBuffer(string deviceId, int size, _CorsairLedColor_V4[] ledColors)
        {
            return _corsairSetLedColorsBufferPointer(deviceId, size, ledColors);
        }

        /// <summary>
        /// iCUE-SDK: Flushes the LED colors buffer asynchronously.
        /// </summary>
        internal static CorsairError CorsairSetLedColorsFlushBufferAsync(CorsairAsyncCallback callback, IntPtr context)
        {
            return _corsairSetLedColorsFlushBufferAsyncPointer(callback, context);
        }

        // ReSharper restore EventExceptionNotDocumented

        #endregion
    }
}
