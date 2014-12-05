// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Dissidence">
//   Copyright (c) 2014 Florian Maunier
// </copyright>
// <summary>
//   The logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TwitchModel
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The logger.
    /// </summary>
    internal class Logger
    {
        #region Public Methods and Operators

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Debug(string message)
        {
            #if DEBUG
                Print("DEBUG", message, ConsoleColor.Cyan);
            #endif
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Error(string message)
        {
            Print("ERROR", message);
        }

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Fatal(string message)
        {
            Print("FATAL", message);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Info(string message)
        {
            Print("INFO", message);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Warn(string message)
        {
            Print("WARNING", message);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The print.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private static void Print(string level, string message)
        {
            Console.Write(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            Console.Write("|");
            Console.Write(level);
            Console.Write("|");
            Console.WriteLine(message);
        }

        /// <summary>
        /// The print.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="foregroundColor">
        /// The foreground color.
        /// </param>
        private static void Print(string level, string message, ConsoleColor foregroundColor)
        {
            Console.Write(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            Console.Write("|");
            Console.Write(level);
            Console.Write("|");
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        #endregion
    }
}