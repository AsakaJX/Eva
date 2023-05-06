using Eva.Modules.Logger;
using Eva.Modules.TwitchBot;

namespace Eva {
    class Program {
        static void Main() {
            Console.Clear(); // ? Clear console because VSCode is stupid

            Log log = new Log();
            log.NewLog(LogSeverity.Info, "Eva Main", "Eva is starting...");

            TwitchBotInitialize _TwitchBot = new TwitchBotInitialize();
            Console.ReadLine();
        }
    }
}