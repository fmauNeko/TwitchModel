// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Channel.cs" company="Dissidence">
//   Copyright (c) 2014 Florian Maunier
// </copyright>
// <summary>
//   The channel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TwitchModel.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The channel.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    internal class Channel
    {
        // ReSharper disable InconsistentNaming
        #region Public Properties

        /// <summary>
        /// Gets or sets the _id.
        /// </summary>
        public int _id { get; set; }

        /// <summary>
        /// Gets or sets the _links.
        /// </summary>
        public object _links { get; set; }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        public object background { get; set; }

        /// <summary>
        /// Gets or sets the banner.
        /// </summary>
        public object banner { get; set; }

        /// <summary>
        /// Gets or sets the broadcaster_language.
        /// </summary>
        public object broadcaster_language { get; set; }

        /// <summary>
        /// Gets or sets the created_at.
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        public object delay { get; set; }

        /// <summary>
        /// Gets or sets the display_name.
        /// </summary>
        public string display_name { get; set; }

        /// <summary>
        /// Gets or sets the followers.
        /// </summary>
        public int followers { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        public string game { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        public string logo { get; set; }

        /// <summary>
        /// Gets or sets the mature.
        /// </summary>
        public object mature { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether partner.
        /// </summary>
        public bool partner { get; set; }

        /// <summary>
        /// Gets or sets the profile_banner.
        /// </summary>
        public string profile_banner { get; set; }

        /// <summary>
        /// Gets or sets the profile_banner_background_color.
        /// </summary>
        public string profile_banner_background_color { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the updated_at.
        /// </summary>
        public string updated_at { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the video_banner.
        /// </summary>
        public string video_banner { get; set; }

        /// <summary>
        /// Gets or sets the views.
        /// </summary>
        public int views { get; set; }

        #endregion

        // Resharper restore InconsistentNaming
    }
}