// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using CUE.NET.Devices;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Headset;
using CUE.NET.Devices.HeadsetStand;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Mouse;
using CUE.NET.Devices.Mousemat;
using CUE.NET.EventArgs;
using CUE.NET.Exceptions;
using CUE.NET.Native;

namespace CUE.NET
{
    /// <summary>
    /// Static entry point to work with the Corsair-SDK.
    /// </summary>
    public static partial class CueSDK
    {
        #region Properties & Fields

        // ReSharper disable UnusedAutoPropertyAccessor.Global

        /// <summary>
        /// Gets a modifiable list of paths used to find the native SDK-dlls for x86 applications.
        /// The first match will be used.
        /// </summary>
        public static List<string> PossibleX86NativePaths { get; } = new List<string> { "x86/CUESDK_2015.dll", "x86/CUESDK.dll" };

        /// <summary>
        /// Gets a modifiable list of paths used to find the native SDK-dlls for x64 applications.
        /// The first match will be used.
        /// </summary>
        public static List<string> PossibleX64NativePaths { get; } = new List<string> { "x64/iCUESDK.x64_2019.dll" };

        /// <summary>
        /// Indicates if the SDK is initialized and ready to use.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the loaded architecture (x64/x86).
        /// </summary>
        public static string LoadedArchitecture => _CUESDK.LoadedArchitecture;

        /// <summary>
        /// Gets the protocol details for the current SDK-connection.
        /// </summary>
        public static CorsairProtocolDetails ProtocolDetails { get; private set; }

        /// <summary>
        /// Gets whether the application has exclusive access to the SDK or not.
        /// </summary>
        public static bool HasExclusiveAccess { get; private set; }

        // Note: API 4.x returns errors directly from functions, no need for LastError property

        /// <summary>
        /// Gets all initialized devices managed by the CUE-SDK.
        /// </summary>
        public static IEnumerable<ICueDevice> InitializedDevices { get; private set; }

        /// <summary>
        /// Gets the managed representation of a keyboard managed by the CUE-SDK.
        /// Note that currently only one connected keyboard is supported.
        /// </summary>
        public static CorsairKeyboard KeyboardSDK { get; private set; }

        /// <summary>
        /// Gets the managed representation of a mouse managed by the CUE-SDK.
        /// Note that currently only one connected mouse is supported.
        /// </summary>
        public static CorsairMouse MouseSDK { get; private set; }

        /// <summary>
        /// Gets the managed representation of a headset managed by the CUE-SDK.
        /// Note that currently only one connected headset is supported.
        /// </summary>
        public static CorsairHeadset HeadsetSDK { get; private set; }

        /// <summary>
        /// Gets the managed representation of a mousemat managed by the CUE-SDK.
        /// Note that currently only one connected mousemat is supported.
        /// </summary>
        public static CorsairMousemat MousematSDK { get; private set; }

        /// <summary>
        /// Gets the managed representation of a headset stand managed by the CUE-SDK.
        /// Note that currently only one connected headset stand is supported.
        /// </summary>
        public static CorsairHeadsetStand HeadsetStandSDK { get; private set; }

        // ReSharper restore UnusedAutoPropertyAccessor.Global

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void OnKeyPressedDelegate(IntPtr context, CorsairKeyId keyId, [MarshalAs(UnmanagedType.I1)] bool pressed);
        private static OnKeyPressedDelegate _onKeyPressedDelegate;

        // API 4.x session state callback - must be kept alive to prevent GC
        private static readonly _CUESDK.CorsairSessionStateChangedHandler _sessionStateChangedHandler = OnSessionStateChanged;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the SDK reports that a key is pressed.
        /// Notice that right now only G- (keyboard) and M- (mouse) keys are supported.
        /// 
        /// To enable this event <see cref="EnableKeypressCallback"/> needs to be called.
        /// </summary>
        public static event EventHandler<KeyPressedEventArgs> KeyPressed;

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the SDK for the provided <see cref="CorsairDeviceType"/> is available or checks if iCUE is installed and SDK supported enabled if null is provided.
        /// </summary>
        /// <param name="sdkType">The <see cref="CorsairDeviceType"/> to check or null to check for general SDK availability.</param>
        /// <returns>The availability of the provided <see cref="CorsairDeviceType"/>.</returns>
        public static bool IsSDKAvailable(CorsairDeviceType? sdkType = null)
        {
            try
            {
                if (IsInitialized)
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases - everything else is true
                    switch (sdkType)
                    {
                        case CorsairDeviceType.Keyboard:
                            return KeyboardSDK != null;
                        case CorsairDeviceType.Mouse:
                            return MouseSDK != null;
                        case CorsairDeviceType.Headset:
                            return HeadsetSDK != null;
                        case CorsairDeviceType.Mousemat:
                            return MousematSDK != null;
                        case CorsairDeviceType.HeadsetStand:
                            return HeadsetStandSDK != null;
                        default:
                            return true;
                    }
                }
                else
                {
                    _CUESDK.Reload();
                    CorsairError error = _CUESDK.CorsairConnect(_sessionStateChangedHandler, IntPtr.Zero);

                    if (sdkType == null || sdkType == CorsairDeviceType.Unknown)
                        return error == CorsairError.Success;

                    if (error != CorsairError.Success)
                        return false;

                    _CorsairDeviceInfo_V4[] deviceInfos = new _CorsairDeviceInfo_V4[64];
                    int deviceCount;
                    error = _CUESDK.CorsairGetDevices(IntPtr.Zero, deviceInfos.Length, deviceInfos, out deviceCount);

                    if (error != CorsairError.Success)
                        return false;

                    for (int i = 0; i < deviceCount; i++)
                    {
                        if (deviceInfos[i].type == sdkType.Value)
                            return true;
                    }

                    // Disconnect after check
                    _CUESDK.CorsairDisconnect();
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        // ReSharper disable once ExceptionNotThrown
        /// <summary>
        /// Initializes the iCUE-SDK (API 4.x). This method should be called exactly ONE time, before anything else is done.
        /// </summary>
        /// <param name="exclusiveAccess">Specifies whether the application should request exclusive access or not.</param>
        /// <exception cref="WrapperException">Thrown if the SDK is already initialized, the SDK is not compatible to iCUE or if iCUE returns unknown devices.</exception>
        /// <exception cref="CUEException">Thrown if the iCUE-SDK provides an error.</exception>
        public static void Initialize(bool exclusiveAccess = false)
        {
            if (IsInitialized)
                throw new WrapperException("CueSDK is already initialized.");

            _CUESDK.Reload();

            // Connect to iCUE (API 4.x)
            CorsairError error = _CUESDK.CorsairConnect(_sessionStateChangedHandler, IntPtr.Zero);
            if (error != CorsairError.Success)
                Throw(error, true);

            // Get session details
            _CorsairSessionDetails sessionDetails;
            error = _CUESDK.CorsairGetSessionDetails(out sessionDetails);
            if (error != CorsairError.Success)
                Throw(error, true);

            ProtocolDetails = new CorsairProtocolDetails(sessionDetails);

            if (ProtocolDetails.BreakingChanges)
                throw new WrapperException("The SDK currently used isn't compatible with the installed version of iCUE.\r\n"
                    + $"iCUE-Version: {ProtocolDetails.ServerHostVersion}\r\n"
                    + $"Server-Version: {ProtocolDetails.ServerVersion}\r\n"
                    + $"Client-Version: {ProtocolDetails.ClientVersion}");

            // Get all devices
            IList<ICueDevice> devices = new List<ICueDevice>();
            _CorsairDeviceInfo_V4[] deviceInfos = new _CorsairDeviceInfo_V4[64]; // CORSAIR_DEVICE_COUNT_MAX
            int deviceCount;
            error = _CUESDK.CorsairGetDevices(IntPtr.Zero, deviceInfos.Length, deviceInfos, out deviceCount);

            if (error != CorsairError.Success)
                Throw(error, true);

            for (int i = 0; i < deviceCount; i++)
            {
                _CorsairDeviceInfo_V4 nativeDeviceInfo = deviceInfos[i];

                // Create device-specific info and device
                ICueDevice device;
                switch (nativeDeviceInfo.type)
                {
                    case CorsairDeviceType.Keyboard:
                        {
                            CorsairKeyboardDeviceInfo keyboardInfo = new CorsairKeyboardDeviceInfo(nativeDeviceInfo);

                            // Request exclusive access per device if needed
                            if (exclusiveAccess)
                            {
                                error = _CUESDK.CorsairRequestControl(keyboardInfo.DeviceId, CorsairAccessLevel.ExclusiveLightingControl);
                                if (error != CorsairError.Success)
                                    Throw(error, true);
                            }

                            device = KeyboardSDK = new CorsairKeyboard(keyboardInfo);
                        }
                        break;
                    case CorsairDeviceType.Mouse:
                        {
                            CorsairMouseDeviceInfo mouseInfo = new CorsairMouseDeviceInfo(nativeDeviceInfo);

                            // Request exclusive access per device if needed
                            if (exclusiveAccess)
                            {
                                error = _CUESDK.CorsairRequestControl(mouseInfo.DeviceId, CorsairAccessLevel.ExclusiveLightingControl);
                                if (error != CorsairError.Success)
                                    Throw(error, true);
                            }

                            device = MouseSDK = new CorsairMouse(mouseInfo);
                        }
                        break;
                    case CorsairDeviceType.Headset:
                        {
                            CorsairHeadsetDeviceInfo headsetInfo = new CorsairHeadsetDeviceInfo(nativeDeviceInfo);

                            // Request exclusive access per device if needed
                            if (exclusiveAccess)
                            {
                                error = _CUESDK.CorsairRequestControl(headsetInfo.DeviceId, CorsairAccessLevel.ExclusiveLightingControl);
                                if (error != CorsairError.Success)
                                    Throw(error, true);
                            }

                            device = HeadsetSDK = new CorsairHeadset(headsetInfo);
                        }
                        break;
                    case CorsairDeviceType.Mousemat:
                        {
                            CorsairMousematDeviceInfo mousematInfo = new CorsairMousematDeviceInfo(nativeDeviceInfo);

                            // Request exclusive access per device if needed
                            if (exclusiveAccess)
                            {
                                error = _CUESDK.CorsairRequestControl(mousematInfo.DeviceId, CorsairAccessLevel.ExclusiveLightingControl);
                                if (error != CorsairError.Success)
                                    Throw(error, true);
                            }

                            device = MousematSDK = new CorsairMousemat(mousematInfo);
                        }
                        break;
                    case CorsairDeviceType.HeadsetStand:
                        {
                            CorsairHeadsetStandDeviceInfo headsetStandInfo = new CorsairHeadsetStandDeviceInfo(nativeDeviceInfo);

                            // Request exclusive access per device if needed
                            if (exclusiveAccess)
                            {
                                error = _CUESDK.CorsairRequestControl(headsetStandInfo.DeviceId, CorsairAccessLevel.ExclusiveLightingControl);
                                if (error != CorsairError.Success)
                                    Throw(error, true);
                            }

                            device = HeadsetStandSDK = new CorsairHeadsetStand(headsetStandInfo);
                        }
                        break;
                    // ReSharper disable once RedundantCaseLabel
                    case CorsairDeviceType.Unknown:
                    default:
                        throw new WrapperException("Unknown Device-Type");
                }

                device.Initialize();
                devices.Add(device);
            }

            InitializedDevices = new ReadOnlyCollection<ICueDevice>(devices);
            HasExclusiveAccess = exclusiveAccess;
            IsInitialized = true;
        }

        /// <summary>
        /// Enables the keypress-callback.
        /// This method needs to be called to enable the <see cref="KeyPressed"/>-event.
        ///
        /// NOTE: API 4.x requires rewriting this functionality using CorsairSubscribeForEvents
        /// </summary>
        public static void EnableKeypressCallback()
        {
            if (!IsInitialized)
                throw new WrapperException("CueSDK isn't initialized.");

            if (_onKeyPressedDelegate != null)
                return;

            _onKeyPressedDelegate = OnKeyPressed;
            // TODO: Implement CorsairSubscribeForEvents for API 4.x
            // _CUESDK.CorsairSubscribeForEvents(...);
            throw new NotImplementedException("Key event handling needs to be migrated to API 4.x CorsairSubscribeForEvents");
        }

        /// <summary>
        /// Resets the colors of all devices back to the last saved color-data. (If there wasn't a manual save, that's the data from the time the SDK was initialized.)
        /// </summary>
        public static void Reset()
        {
            foreach (ICueDevice device in InitializedDevices)
                device.RestoreColors();
        }

        /// <summary>
        /// Reinitialize the CUE-SDK and temporarily hand back full control to CUE.
        /// </summary>
        public static void Reinitialize()
        {
            Reinitialize(HasExclusiveAccess);
        }

        /// <summary>
        /// Reinitialize the iCUE-SDK and temporarily hand back full control to iCUE.
        /// </summary>
        /// <param name="exclusiveAccess">Specifies whether the application should request exclusive access or not.</param>
        public static void Reinitialize(bool exclusiveAccess)
        {
            if (!IsInitialized)
                throw new WrapperException("CueSDK isn't initialized.");

            if (_onKeyPressedDelegate != null)
                throw new WrapperException("Keypress-Callback is enabled.");

            KeyboardSDK?.ResetLeds();
            MouseSDK?.ResetLeds();
            HeadsetSDK?.ResetLeds();
            MousematSDK?.ResetLeds();
            HeadsetStandSDK?.ResetLeds();

            // Disconnect and reconnect
            _CUESDK.CorsairDisconnect();
            _CUESDK.Reload();

            CorsairError error = _CUESDK.CorsairConnect(_sessionStateChangedHandler, IntPtr.Zero);
            if (error != CorsairError.Success)
                Throw(error, false);

            // Get device list to verify previously loaded devices
            _CorsairDeviceInfo_V4[] deviceInfos = new _CorsairDeviceInfo_V4[64];
            int deviceCount;
            error = _CUESDK.CorsairGetDevices(IntPtr.Zero, deviceInfos.Length, deviceInfos, out deviceCount);

            if (error != CorsairError.Success)
                Throw(error, false);

            Dictionary<CorsairDeviceType, GenericDeviceInfo> reloadedDevices = new Dictionary<CorsairDeviceType, GenericDeviceInfo>();
            for (int i = 0; i < deviceCount; i++)
            {
                GenericDeviceInfo info = new GenericDeviceInfo(deviceInfos[i]);
                reloadedDevices[info.Type] = info;

                // Request exclusive access per device if needed
                if (exclusiveAccess)
                {
                    error = _CUESDK.CorsairRequestControl(info.DeviceId, CorsairAccessLevel.ExclusiveLightingControl);
                    if (error != CorsairError.Success)
                        Throw(error, false);
                }
            }

            // Verify previously loaded devices are still connected
            if (KeyboardSDK != null)
                if (!reloadedDevices.ContainsKey(CorsairDeviceType.Keyboard)
                    || ((GenericDeviceInfo)KeyboardSDK.DeviceInfo).Model != reloadedDevices[CorsairDeviceType.Keyboard].Model)
                    throw new WrapperException("The previously loaded Keyboard got disconnected.");
            if (MouseSDK != null)
                if (!reloadedDevices.ContainsKey(CorsairDeviceType.Mouse)
                    || ((GenericDeviceInfo)MouseSDK.DeviceInfo).Model != reloadedDevices[CorsairDeviceType.Mouse].Model)
                    throw new WrapperException("The previously loaded Mouse got disconnected.");
            if (HeadsetSDK != null)
                if (!reloadedDevices.ContainsKey(CorsairDeviceType.Headset)
                    || ((GenericDeviceInfo)HeadsetSDK.DeviceInfo).Model != reloadedDevices[CorsairDeviceType.Headset].Model)
                    throw new WrapperException("The previously loaded Headset got disconnected.");
            if (MousematSDK != null)
                if (!reloadedDevices.ContainsKey(CorsairDeviceType.Mousemat)
                    || ((GenericDeviceInfo)MousematSDK.DeviceInfo).Model != reloadedDevices[CorsairDeviceType.Mousemat].Model)
                    throw new WrapperException("The previously loaded Mousemat got disconnected.");
            if (HeadsetStandSDK != null)
                if (!reloadedDevices.ContainsKey(CorsairDeviceType.HeadsetStand)
                    || ((GenericDeviceInfo)HeadsetStandSDK.DeviceInfo).Model != reloadedDevices[CorsairDeviceType.HeadsetStand].Model)
                    throw new WrapperException("The previously loaded Headset Stand got disconnected.");

            HasExclusiveAccess = exclusiveAccess;
            IsInitialized = true;
        }

        private static void Throw(CorsairError error, bool reset)
        {
            if (reset)
            {
                ProtocolDetails = null;
                HasExclusiveAccess = false;
                KeyboardSDK = null;
                MouseSDK = null;
                HeadsetSDK = null;
                MousematSDK = null;
                HeadsetStandSDK = null;
                IsInitialized = false;
            }

            throw new CUEException(error);
        }

        private static void OnKeyPressed(IntPtr context, CorsairKeyId keyId, bool pressed)
            => KeyPressed?.Invoke(null, new KeyPressedEventArgs(keyId, pressed));

        private static void OnSessionStateChanged(IntPtr context, IntPtr eventData)
        {
            // API 4.x session state change callback
            // For now, we just ignore session state changes
            // Could be enhanced to handle reconnection, version changes, etc.
        }

        #endregion
    }
}
