using System.Diagnostics;
using System.Web;
using Eva.Modules.Logger;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Eva.Modules.TwitchBot {
    public delegate Task VoiceVoxExitedEvent();
    public delegate void VoiceVoxPlayerExitedEvent();
    public delegate Task<string> TranslateFinishedEvent();
    public delegate string EvaResponeGeneratedEvent();
    public delegate string TranslateFinalEvent();
    public delegate void VoiceVoxStarted();
    public class TwitchBotEvents {
        public event VoiceVoxExitedEvent? onVoiceVoxExited = null;
        public void onVoiceVoxExitedInvoke() {
            onVoiceVoxExited?.Invoke();
        }
        public event VoiceVoxPlayerExitedEvent? onVoiceVoxPlayerExited = null;
        public void onVoiceVoxPlayerExitedInvoke() {
            onVoiceVoxPlayerExited?.Invoke();
        }
        public event VoiceVoxStarted? onVoiceVoxStarted = null;
        public void onVoiceVoxStartedInvoke() {
            onVoiceVoxStarted?.Invoke();
        }
        // ! Translate -> GenerateResponse -> Translate
        public event TranslateFinishedEvent? onTranslateFinished = null;
        public void onTranslateFinishedInvoke() {
            onTranslateFinished?.Invoke();
        }
        public event EvaResponeGeneratedEvent? onEVARsponseGenerated = null;
        public void onEVARsponseGeneratedInvoke() {
            onEVARsponseGenerated?.Invoke();
        }
        public event TranslateFinalEvent? onTranslateFinalFinished = null;
        public void onTranslateFinalFinishedInvoke() {
            onTranslateFinalFinished?.Invoke();
        }
    }
    class TwitchBotInitialize {
        const string _TwitchUsername = "asmrsama";
        TwitchClient client;
        Dictionary<string, string> _nextMessageStack = new Dictionary<string, string>() { { ".empty", "" } };
        TwitchBotEvents EventHandler = new TwitchBotEvents();
        string EvaResponseTranslated = "@@@ EVA RESPONSE TRANSLATED @@@";
        Log log = new Log();
        public TwitchBotInitialize() {
            log.NewLog(LogSeverity.Info, "Twitch Module", "Twitch Module has been initialized!");

            ConnectionCredentials credentials = new ConnectionCredentials("asmrsama", "aqiooprzxv6yzm2i5rgjlrf8838glj");
            var clientOptions = new ClientOptions {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, _TwitchUsername);

            client.OnConnected += Client_OnConnected;
            client.OnMessageReceived += Client_OnMessageReceived;

            EventHandler.onVoiceVoxExited += PlayAudio;
            // EventHandler.onVoiceVoxStarted += something; // !
            EventHandler.onVoiceVoxPlayerExited += ChooseNextMessage;

            client.Connect();
        }

        // Events handling
        private async void Client_OnConnected(object? sender, OnConnectedArgs e) {
            log.NewLog(LogSeverity.Info, "Twitch Module", "Connected to chat!");

            string readyForEva = Translate("en", "やあ、ここで初めてメッセージを書くよ。");
            string EvaResponse = GetResponse(readyForEva);
            string EvaResponseFinal = Translate("ja", EvaResponse);
            await RunVoiceVox(EvaResponseFinal);
        }

        private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e) {
            log.NewLog(LogSeverity.Info, "Twitch Module", $"{e.ChatMessage.Username} wrote {e.ChatMessage.Message}");

            if (!_nextMessageStack.ContainsKey(e.ChatMessage.Username)) {
                _nextMessageStack.Add(e.ChatMessage.Username, e.ChatMessage.Message);
            }
            if (_nextMessageStack.ContainsKey(e.ChatMessage.Username) && _nextMessageStack[e.ChatMessage.Username] != e.ChatMessage.Message) {
                _nextMessageStack[e.ChatMessage.Username] = e.ChatMessage.Message;
            }
        }

        private async Task RunVoiceVox(string msg) {
            _ = Task.Run(async () => {
                try {
                    Process proc = new Process();
                    proc.StartInfo.FileName = "/bin/zsh";
                    proc.StartInfo.Arguments = "-ic \" " + $"cd ~/VoiceVox/;echo \"\"{msg}\"\" >text.txt;voicevox_generate" + " \"";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();

                    using (StreamReader procreader = proc.StandardOutput) {
                        string result = procreader.ReadToEnd();
                    }
                } catch (Exception ex) { log.NewLog(LogSeverity.Warning, "Twitch Module|VoiceVox", ex.ToString(), 1); }

                EventHandler.onVoiceVoxExitedInvoke();
                await Task.CompletedTask;
            });
            await Task.CompletedTask;
        }

        private async Task PlayAudio() {
            _ = Task.Run(async () => {
                try {
                    Process proc = new Process();
                    proc.StartInfo.FileName = "/bin/zsh";
                    proc.StartInfo.Arguments = "-ic \" " + $"fuckyou" + " \"";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();

                    using (StreamReader procreader = proc.StandardOutput) {
                        string result = procreader.ReadToEnd();
                    }
                } catch (Exception ex) { log.NewLog(LogSeverity.Warning, "Twitch Module|VoiceVox", ex.ToString(), 1); }
                log.NewLog(LogSeverity.Info, "Twitch Module|VoiceVox", "Phrase played successfully!", 1);
                EventHandler.onVoiceVoxPlayerExitedInvoke();
                await Task.CompletedTask;
            });
            await Task.CompletedTask;
        }

        private string Translate(string translateTo = "ja", string input = "Hey, it's a test phrase!") {
            string result = "";

            Process proc = new Process();
            proc.StartInfo.FileName = "/bin/zsh";
            proc.StartInfo.Arguments = "-c \" " + $"python ~/VSProjects/Eva/Python\\ Scripts/Translate.py \"\"{translateTo}\"\" \"\"{input}\"\"" + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();

            using (StreamReader procreader = proc.StandardOutput) {
                result = procreader.ReadToEnd();
                log.NewLog(LogSeverity.Info, "Twitch Module|Translate", $"Translated phrase: {result.Replace("\n", " ")}", 1);
            }

            if (translateTo == "ja") { EvaResponseTranslated = result; log.NewLog(LogSeverity.Info, "Twitch Module|Translate", $"Response translated: {EvaResponseTranslated.Replace("\n", " ")} ", 1); }
            return result;
        }

        private string GetResponse(string prompt = "What's your name ?") {
            string result = "";

            Process proc = new Process();
            proc.StartInfo.FileName = "/bin/zsh";
            proc.StartInfo.Arguments = "-c \" " + $"cd ~/VSProjects/Eva/Python\\ Scripts/;python ConversationAIRequest.py \"\"{prompt}\"\"" + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();

            using (StreamReader procreader = proc.StandardOutput) {
                result = procreader.ReadToEnd();

                result = result.Substring(result.IndexOf("EVA_FINAL: ") + "EVA_FINAL: ".Length).Replace("\n", " ");
                log.NewLog(LogSeverity.Info, "Twitch Module|Eva", $"Response: {result}", 1);
            }

            return result;
        }

        private async void ChooseNextMessage() {
            // TODO > Make next message normalizers (ex. remove links, emoji etc.)

            _ = Task.Run(async () => {
                // Console.Clear();
                log.NewLog(LogSeverity.Info, "Twitch Module|Next Message", $"Waiting for 2.5 seconds...");
                Thread.Sleep(2500);
                string nextMessage = "";
                if (_nextMessageStack.Count > 1) {
                    nextMessage = _nextMessageStack[_nextMessageStack.Keys.ElementAt(new Random().Next(1, _nextMessageStack.Keys.Count))];
                    _nextMessageStack.Clear();
                    _nextMessageStack.Add(".empty", "");
                }
                if (nextMessage == "") { log.NewLog(LogSeverity.Verbose, "Twitch Module|Next Message", $"Next message is empty."); ChooseNextMessage(); return; }

                log.NewLog(LogSeverity.Info, "Twitch Module|Next Message", $"Next message is: {nextMessage}", 1);
                nextMessage = HttpUtility.UrlEncode(nextMessage.Trim());

                string readyForEva = Translate("en", nextMessage);
                string EvaRespone = GetResponse(readyForEva);
                EvaResponseTranslated = Translate("ja", EvaRespone);
                log.NewLog(LogSeverity.Debug, "Twitch Module|Next Message", $"EvaResponseTranslated: {EvaResponseTranslated}", 1);
                await RunVoiceVox(EvaResponseTranslated);

                await Task.CompletedTask;
            });
            await Task.CompletedTask;
        }
    }
}