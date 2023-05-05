using System.Diagnostics;

using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Eva.TwitchBot {
    public delegate Task VoiceVoxExitedEvent();
    public delegate void VoiceVoxPlayerExitedEvent();
    public class TwitchBotEvents {
        public event VoiceVoxExitedEvent? onVoiceVoxExited = null;
        public void onVoiceVoxExitedInvoke() {
            onVoiceVoxExited?.Invoke();
        }
        public event VoiceVoxPlayerExitedEvent? onVoiceVoxPlayerExited = null;
        public void onVoiceVoxPlayerExitedInvoke() {
            onVoiceVoxPlayerExited?.Invoke();
        }

    }
    class TwitchBotInitialize {
        TwitchClient client;
        Dictionary<string, string> _nextMessageStack = new Dictionary<string, string>() { { ".empty", "" } };
        TwitchBotEvents VoiceVoxEventHandler = new TwitchBotEvents();
        public TwitchBotInitialize() {
            ConnectionCredentials credentials = new ConnectionCredentials("asmrsama", "aqiooprzxv6yzm2i5rgjlrf8838glj");
            var clientOptions = new ClientOptions {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "asmrsama");

            // Events handling
            client.OnConnected += Client_OnConnected;
            client.OnMessageReceived += Client_OnMessageReceived;

            VoiceVoxEventHandler.onVoiceVoxExited += PlayAudio;
            VoiceVoxEventHandler.onVoiceVoxPlayerExited += ChooseNextMessage;

            client.Connect();
        }

        // Events handling
        private async void Client_OnConnected(object? sender, OnConnectedArgs e) {
            await PlayAudio();
            Console.WriteLine($"Connected to chat!");
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

        private Process RunVoiceVox(string msg) {
            Console.WriteLine($"Current phrase: {msg}");
            Process proc = new System.Diagnostics.Process();
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
            VoiceVoxEventHandler.onVoiceVoxExitedInvoke();
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
                Console.WriteLine("Played successfully!");
                VoiceVoxEventHandler.onVoiceVoxPlayerExitedInvoke();
                await Task.CompletedTask;
            });
            await Task.CompletedTask;
        }

        private async void ChooseNextMessage() {
            _ = Task.Run(async () => {
                Console.Clear();
                Console.WriteLine("STOPPING FOR 2.5 SECONDS...");
                Thread.Sleep(2500);
                string nextMessage = "";
                if (_nextMessageStack.Count > 1) {
                    nextMessage = _nextMessageStack[_nextMessageStack.Keys.ElementAt(new Random().Next(1, _nextMessageStack.Keys.Count))];
                    _nextMessageStack.Clear();
                    _nextMessageStack.Add(".empty", "");
                }
                if (nextMessage == "") { Console.WriteLine("NEXT MESSAGE IS EMPTY\n"); ChooseNextMessage(); return; }
                Console.WriteLine($"NEXT PHRASE IS: {nextMessage}\n");
                RunVoiceVox(nextMessage);
                await Task.CompletedTask;
            });
            await Task.CompletedTask;
        }
    }
}