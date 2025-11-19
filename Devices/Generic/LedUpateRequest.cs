// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace CUE.NET.Devices.Generic
{
    /// <summary>
    /// Represents a request to update a led.
    /// </summary>
    public class LedUpateRequest
    {
        #region Properties & Fields

        /// <summary>
        /// Gets the LUID of the led to update.
        /// </summary>
        public uint LedId { get; }

        /// <summary>
        /// Gets the requested color of the led.
        /// </summary>
        public CorsairColor Color { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LedUpateRequest"/> class.
        /// </summary>
        /// <param name="ledId">The LUID of the led to update.</param>
        /// <param name="color">The requested color of the led.</param>
        public LedUpateRequest(uint ledId, CorsairColor color)
        {
            this.LedId = ledId;
            this.Color = color;
        }

        #endregion
    }
}
