using System.Diagnostics;
using System.Web;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Eva.TwitchBot {
    public delegate Task VoiceVoxExitedEvent();
    public delegate void VoiceVoxPlayerExitedEvent();
    public delegate Task<string> TranslateFinishedEvent();
    public delegate string EvaResponeGeneratedEvent();
    public delegate string TranslateFinalEvent();
    public class TwitchBotEvents {
        public event VoiceVoxExitedEvent? onVoiceVoxExited = null;
        public void onVoiceVoxExitedInvoke() {
            onVoiceVoxExited?.Invoke();
        }
        public event VoiceVoxPlayerExitedEvent? onVoiceVoxPlayerExited = null;
        public void onVoiceVoxPlayerExitedInvoke() {
            onVoiceVoxPlayerExited?.Invoke();
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
        public TwitchBotInitialize() {
            ConnectionCredentials credentials = new ConnectionCredentials("asmrsama", "aqiooprzxv6yzm2i5rgjlrf8838glj");
            var clientOptions = new ClientOptions {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, _TwitchUsername);

            // Events handling
            // client.OnConnected += Client_OnConnected;
            // client.OnMessageReceived += Client_OnMessageReceived;

            // EventHandler.onVoiceVoxExited += PlayAudio;
            // EventHandler.onVoiceVoxPlayerExited += ChooseNextMessage;

            // ! Getting message -> Translate -> Generate -> Translate -> VoiceVox
            client.OnConnected += Client_OnConnected;
            client.OnMessageReceived += Client_OnMessageReceived;

            // EventHandler.onTranslateFinished += GetResponse;
            // EventHandler.onTranslateFinalFinished += ChooseNextMessage;
            EventHandler.onVoiceVoxExited += PlayAudio;
            EventHandler.onVoiceVoxPlayerExited += ChooseNextMessage;

            // TranslateFinishedEvent.

            client.Connect();
        }

        // Events handling
        private async void Client_OnConnected(object? sender, OnConnectedArgs e) {
            Console.WriteLine($"🦽 Connected to chat! 🦽");
            // await PlayAudio();
            string readyForEva = Translate("en", "やあ、ここで初めてメッセージを書くよ。");
            string EvaRespone = GetResponse(readyForEva);
            string EvaResponseFinal = Translate("ja", EvaRespone);
            await RunVoiceVox(EvaResponseFinal);
        }

        private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e) {
            Console.WriteLine($"{e.ChatMessage.Username} : {e.ChatMessage.Message}");

            if (!_nextMessageStack.ContainsKey(e.ChatMessage.Username)) {
                _nextMessageStack.Add(e.ChatMessage.Username, e.ChatMessage.Message);
            }
            if (_nextMessageStack.ContainsKey(e.ChatMessage.Username) && _nextMessageStack[e.ChatMessage.Username] != e.ChatMessage.Message) {
                _nextMessageStack[e.ChatMessage.Username] = e.ChatMessage.Message;
            }
        }

        private async Task<Process> RunVoiceVox(string msg) {
            Process proc = new System.Diagnostics.Process();
            _ = Task.Run(async () => {
                Console.WriteLine($"Current phrase: {msg}");
                proc.StartInfo.FileName = "/bin/zsh";
                proc.StartInfo.Arguments = "-ic \" " + $"cd ~/VoiceVox/;echo \"\"{msg}\"\" >text.txt;voicevox_generate" + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                using (StreamReader procreader = proc.StandardOutput) {
                    string result = procreader.ReadToEnd();
                    Console.WriteLine(result);
                }
                EventHandler.onVoiceVoxExitedInvoke();
                await Task.CompletedTask;
            });
            await Task.CompletedTask;
            return proc;
        }

        private async Task PlayAudio() {
            _ = Task.Run(async () => {
                Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "/bin/zsh";
                proc.StartInfo.Arguments = "-ic \" " + "fuckyou" + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();

                using (StreamReader procreader = proc.StandardOutput) {
                    string result = procreader.ReadToEnd();
                    //Console.WriteLine(result);
                }
                Console.WriteLine("✅ [VOICEVOX] Played successfully! ✅");
                EventHandler.onVoiceVoxPlayerExitedInvoke();
                await Task.CompletedTask;
            });
            await Task.CompletedTask;
        }

        private string Translate(string translateTo = "ja", string input = "Hey, it's a test phrase!") {
            string result = "";
            // _ = Task.Run(async () => {
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/zsh";
            proc.StartInfo.Arguments = "-ic \" " + $"python ~/VSProjects/Eva/Python\\ Scripts/Translate.py \"\"{translateTo}\"\" \"\"{input}\"\"" + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();

            string addSpaces = "";
            if (translateTo == "ja") { addSpaces = "        "; }

            using (StreamReader procreader = proc.StandardOutput) {
                result = procreader.ReadToEnd();
                Console.Write($"{addSpaces}   ╰ ✨ TRANSLATED PHRASE ✨\n{addSpaces}     ╰  {result}");
            }
            // await Task.CompletedTask;
            // if (translateTo == "en") { await GetResponse(result); }
            if (translateTo == "ja") { EvaResponseTranslated = result; Console.WriteLine($"{addSpaces}      ╰ 💎 EVA's RESPONSE TRANSLATED 💎\n{addSpaces}         ╰  {EvaResponseTranslated}"); }
            return result;
            // });
            // await Task.CompletedTask;
            // return result;
        }

        private string GetResponse(string prompt = "What's your name ?") {
            string result = "";
            // _ = Task.Run(async () => {
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/zsh";
            proc.StartInfo.Arguments = "-ic \" " + $"python ~/VSProjects/Eva/Python\\ Scripts/ConversationAIRequest.py \"\"{prompt}\"\"" + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();

            using (StreamReader procreader = proc.StandardOutput) {
                result = procreader.ReadToEnd();
                // int firstEVAResponse = result.IndexOf("EVA_RESPONSE: ");
                // result = result.Substring(firstEVAResponse, result.IndexOf("EVA_RESPONSE: ", firstEVAResponse + "EVA_RESPONSE: ".Length));
                result = result.Substring(result.IndexOf("EVA_FINAL: ") + "EVA_FINAL: ".Length).Replace("\n", "");
                Console.WriteLine($"      ╰ 💎 EVA's RESPONSE 💎\n         ╰  {result}");
            }
            // await Task.CompletedTask;
            // EvaResponseTranslated = Translate("ja", result);
            // VoiceVoxEventHandler.onVoiceVoxPlayerExitedInvoke(); // ! THIS IS FOR DEBUG PURPOSES, SHOULD BE REMOVED / COMMENTED
            return result;
            // });
            // await Task.CompletedTask;
            // return result;
        }

        private async void ChooseNextMessage() {
            // TODO > Make next message normalizers (ex. remove links, emoji etc.)

            _ = Task.Run(async () => {
                Console.Clear();
                Console.Write("🛏️ STOPPING FOR 2.5 SECONDS... 🛏️\n ╰ "); // ! [2.5 SECONDS] <- SUBJECT TO CHANGE
                Thread.Sleep(2500);
                string nextMessage = "";
                if (_nextMessageStack.Count > 1) {
                    nextMessage = _nextMessageStack[_nextMessageStack.Keys.ElementAt(new Random().Next(1, _nextMessageStack.Keys.Count))];
                    _nextMessageStack.Clear();
                    _nextMessageStack.Add(".empty", "");
                }
                if (nextMessage == "") { Console.Write("🗿 NEXT MESSAGE IS EMPTY 🗿\n"); ChooseNextMessage(); return; }

                Console.Write($"☄️ NEXT PHRASE IS ☄️\n   ╰  {nextMessage}\n");
                nextMessage = HttpUtility.UrlEncode(nextMessage.Trim());

                // string readyForEva = await Translate("en", nextMessage);
                string readyForEva = Translate("en", nextMessage);
                string EvaRespone = GetResponse(readyForEva);
                EvaResponseTranslated = Translate("ja", EvaRespone);
                await RunVoiceVox(EvaResponseTranslated);

                // RunVoiceVox(nextMessage);
                await Task.CompletedTask;
            });
            await Task.CompletedTask;
        }
    }
}