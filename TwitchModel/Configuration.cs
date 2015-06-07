// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Dissidence">
//   Copyright (c) 2014 Florian Maunier
// </copyright>
// <summary>
//   The configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TwitchModel
{
    using System.Collections.Generic;

    using FX.Configuration;

    /// <summary>
    /// The configuration.
    /// </summary>
    internal class Configuration : JsonConfiguration
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ThingModelURI { get; set; }

        #endregion
    }
}