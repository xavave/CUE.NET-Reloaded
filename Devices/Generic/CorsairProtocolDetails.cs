// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using CUE.NET.Native;

namespace CUE.NET.Devices.Generic
{
    /// <summary>
    /// Managed wrapper for CorsairSessionDetails (API 4.x).
    /// </summary>
    public class CorsairProtocolDetails
    {
        #region Properties & Fields

        /// <summary>
        /// Version of SDK client (like 4.0.1). Always contains valid value even if there was no iCUE found.
        /// </summary>
        public string ClientVersion { get; }

        /// <summary>
        /// Version of SDK server (like 4.0.1) or empty (0.0.0) if the iCUE was not found.
        /// </summary>
        public string ServerVersion { get; }

        /// <summary>
        /// Version of iCUE (like 3.33.100) or empty (0.0.0) if the iCUE was not found.
        /// </summary>
        public string ServerHostVersion { get; }

        /// <summary>
        /// Boolean that specifies if there were breaking changes between version implemented by server and client.
        /// </summary>
        public bool BreakingChanges { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Internal constructor of managed CorsairProtocolDetails from SessionDetails.
        /// </summary>
        /// <param name="nativeDetails">The native CorsairSessionDetails-struct</param>
        internal CorsairProtocolDetails(_CorsairSessionDetails nativeDetails)
        {
            ClientVersion = FormatVersion(nativeDetails.clientVersion);
            ServerVersion = FormatVersion(nativeDetails.serverVersion);
            ServerHostVersion = FormatVersion(nativeDetails.serverHostVersion);

            // Breaking changes only if server is not connected (version 0.0.0)
            // API 4.x is designed to be forward and backward compatible within the same major version
            // so we don't enforce strict version matching
            BreakingChanges = nativeDetails.serverVersion.major == 0;
        }

        private static string FormatVersion(_CorsairVersion version)
        {
            return $"{version.major}.{version.minor}.{version.patch}";
        }

        #endregion
    }
}
