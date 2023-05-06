using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eva.Modules.Logger {
    public enum LogSeverity {
        Info,
        Warning,
        Critical,
        Debug
    }
    public class Log {
        Dictionary<LogSeverity, ConsoleColor> ColorTable = new Dictionary<LogSeverity, ConsoleColor> {
            {LogSeverity.Critical, ConsoleColor.Magenta},
            {LogSeverity.Warning, ConsoleColor.Red},
            {LogSeverity.Debug, ConsoleColor.Yellow},
            {LogSeverity.Info, ConsoleColor.Green},
        };
        public void NewLog(
            LogSeverity severity = LogSeverity.Info,
            string source = "* no source provided *",
            string message = "* empty *",
            int depth = 0
        ) {
            if (depth == 0) { Console.Write('\n'); }
            // string addSpacesDepth = String.Concat(Enumerable.Repeat(" ", depth));
            string arrow = "⇢";
            // if (depth > 0) { arrow = $"{addSpacesDepth}↳"; }
            if (depth > 0) { arrow = $"↳"; }

            string addSpacesSeverity = severity.ToString().Length < "Critical".Length ? String.Concat(Enumerable.Repeat(" ", "Critical".Length - severity.ToString().Length)) : "";
            string addSpacesSource = source.ToString().Length < "SOMETHINGVERYLONGLMAOO".Length ? String.Concat(Enumerable.Repeat(" ", "SOMETHINGVERYLONGLMAOO".Length - source.ToString().Length)) : "";
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{date}  ");
            Console.ForegroundColor = ColorTable[severity];
            Console.Write($"{addSpacesSeverity}{severity.ToString().ToUpper()}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(" ⇢ ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{source}{addSpacesSource}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" ]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\t{arrow}\t");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{message}");

            Console.ResetColor();
            Console.WriteLine();
        }
    }
}