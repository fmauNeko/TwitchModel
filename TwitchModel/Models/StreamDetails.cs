// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamDetails.cs" company="Dissidence">
//   Copyright (c) 2014 Florian Maunier
// </copyright>
// <summary>
//   The stream details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TwitchModel.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The stream details.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    internal class StreamDetails
    {
        // ReSharper disable InconsistentNaming
        #region Public Properties

        /// <summary>
        /// Gets or sets the _id.
        /// </summary>
        public long _id { get; set; }

        /// <summary>
        /// Gets or sets the _links.
        /// </summary>
        public object _links { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        public Channel channel { get; set; }

        /// <summary>
        /// Gets or sets the created_at.
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        public string game { get; set; }

        /// <summary>
        /// Gets or sets the preview.
        /// </summary>
        public object preview { get; set; }

        /// <summary>
        /// Gets or sets the viewers.
        /// </summary>
        public int viewers { get; set; }

        #endregion

        // ReSharper restore InconsistentNaming
    }
}