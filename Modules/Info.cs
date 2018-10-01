using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.Commands;
using System.IO;
using Discord.WebSocket;

namespace PrimaskiBot.Modules {
    public class Info : ModuleBase<SocketCommandContext> {
        string filepath = "..\\..\\suggestions.txt";

        [Command("info")]
        public async Task GetInfo() {
            await ReplyAsync(
                "***Bot Name:*** Primaski Bot#4325\n" +
                "***Bot Creator:*** Primaski#0826\n" +
                "***Bot Creation Date:*** 2018/09/07\n" +
                "***Bot ID:*** 487718576892018689\n" +
                "***Bot Prefix:*** p* or prim*\n" +
                "***Written in:*** C#\n" +
                "***Host:*** Local\n" +
                "***Bot Token:*** u wish\n" +
                "***Bot Invite Link:*** https://discordapp.com/api/oauth2/authorize?client_id=487718576892018689&permissions=8&scope=bot \n");
        }

        [Command("say")]
        public async Task Say([Remainder] string args = null) {
            if (args == null) {
                await ReplyAsync("I mean, like, what do you want me to say?");
            } else {
                if (args.Contains("p*")) {
                    await ReplyAsync("Why are you trying to break my bot?");
                    return;
                }
                await Context.Message.DeleteAsync();
                await ReplyAsync(args);
            }
        }

        [Command("suggest")]
        public async Task Suggest([Remainder] string suggestion = null) {
            if (suggestion == null) {
                await ReplyAsync("Try again. Please type what you want to suggest after t\\*suggest.");
                return;
            }
            StreamWriter sw = File.AppendText(filepath);
            sw.WriteLine(DateTime.Now.Date + DateTime.Now.TimeOfDay);
            sw.WriteLine(Context.User.Username);
            sw.WriteLine(suggestion);
            sw.WriteLine("");
            sw.Close();
            await ReplyAsync("Your suggestion has been successfully added!");
            return;
        }

        [Command("ping")]
        public async Task Ping() {
            await ReplyAsync("Pong!");
        }
    }
}
