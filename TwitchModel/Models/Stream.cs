// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Stream.cs" company="Dissidence">
//   Copyright (c) 2014 Florian Maunier
// </copyright>
// <summary>
//   The stream.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TwitchModel.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The stream.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    internal class Stream
    {
        // ReSharper disable InconsistentNaming
        #region Public Properties

        /// <summary>
        /// Gets or sets the _links.
        /// </summary>
        public object _links { get; set; }

        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        public StreamDetails stream { get; set; }

        #endregion

        // ReSharper restore InconsistentNaming
    }
}