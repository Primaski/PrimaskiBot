using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace PrimaskiBot {
    class Program {
        string botToken = "";
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private string unoServerID = "";

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync() {
            _client = new DiscordSocketClient(new DiscordSocketConfig {
                AlwaysDownloadUsers = true
            });
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += Log;
            _client.GuildAvailable += Connected;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
            await _client.SetGameAsync("P*help");
            await Task.Delay(-1);

        }

        private Task Log(LogMessage arg) {
            Console.WriteLine(arg);
            return Task.CompletedTask;

        }

        public async Task RegisterCommandsAsync() {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

        }

        //test method
        public Task Connected(SocketGuild e){
            if(e.Id.ToString() == unoServerID){
                e.DownloadUsersAsync();
                Task.Delay(6000);
                IReadOnlyCollection<SocketGuildUser> members = e.Users;
                int count = 0;
                foreach (SocketGuildUser x in members){
                    Console.WriteLine(x.Username);
                    count++;
                }
                Console.WriteLine(count);
            }
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage arg) {

            var message = (SocketUserMessage)arg;
            if (message is null || message.Author.IsBot) {
                return;
            }
            int argPos = 0;
            if (message.HasStringPrefix("prim*", ref argPos) || (message.HasStringPrefix("Prim*", ref argPos)) || (message.HasStringPrefix("P*", ref argPos)) || (message.HasStringPrefix("p*", ref argPos)) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) {
                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess) {
                    Console.WriteLine(result.ErrorReason);
                }
            }
            return;
        }
    }
}
