using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eva.Modules.Logger {
    public enum LogSeverity {
        Critical,
        Warning,
        Verbose,
        Debug,
        Info,
    }
    public class Log {
        Dictionary<LogSeverity, ConsoleColor> ColorTable = new Dictionary<LogSeverity, ConsoleColor> {
            {LogSeverity.Critical, ConsoleColor.Magenta},
            {LogSeverity.Warning, ConsoleColor.Red},
            {LogSeverity.Verbose, ConsoleColor.Yellow},
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

            int sourceLimit = 30;
            string addSpacesSeverity = severity.ToString().Length < "Critical".Length ? String.Concat(Enumerable.Repeat(" ", "Critical".Length - severity.ToString().Length)) : "";
            string addSpacesSource = source.ToString().Length < sourceLimit ? String.Concat(Enumerable.Repeat(" ", sourceLimit - source.ToString().Length)) : "";
            if (source.IndexOf('|') != -1) {
                string firstWord = source.Substring(0, source.IndexOf('|'));
                string secondWord = source.Substring(source.IndexOf('|') + 1);

                source = firstWord + addSpacesSource.Insert(0, " ") + secondWord;
                addSpacesSource = "";
            }
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