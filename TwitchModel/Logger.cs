using System;
using System.Globalization;

namespace TwitchModel
{
    class Logger
    {
        private static void Print(string level, string message)
        {
            Console.Write(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            Console.Write("|");
            Console.Write(level);
            Console.Write("|");
            Console.WriteLine(message);
        }

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

        public static void Info(string message)
        {
            Print("INFO", message);
        }
        
        public static void Debug(string message)
        {
            Print("DEBUG", message, ConsoleColor.Cyan);
        }
        public static void Error(string message)
        {
            Print("ERROR", message);
        }
        public static void Warn(string message)
        {
            Print("WARNING", message);
        }
        public static void Fatal(string message)
        {
            Print("FATAL", message);
        }
    }
}
