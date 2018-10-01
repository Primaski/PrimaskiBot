using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Primaski_Bot.Modules;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace PrimaskiBot.Modules {
    public class CreateAccount : ModuleBase<SocketCommandContext> {
        [Command("create")]
        public async Task PingAsync([Remainder] string args = null) {
            if (args != null) {
                if (args.ToLower().Contains("uno")) {
                    string userID = Context.User.Id.ToString();
                    string username = Context.User.Username;
                    /*IMPLEMENT ABILITY TO DO IT BY PING
                     * if (args.Contains("<@!")) {
                        string resultString = Regex.Match(args, @"<\100!(\d*)>").Groups[1].Value;
                        //Discord.WebSocket.SocketGuildUser;
                        await ReplyAsync("this: \\" + resultString);
                        if (user == null) {
                            await ReplyAsync("user not found");
                            return;
                        }
                        username = user.Username;
            

                        await ReplyAsync(userID + " " + username);
                        return;
                    }*/

                    //await ReplyAsync(args);

                    bool attempt = false;
                    if (args.ToLower().Contains("green")) {
                        attempt = UtilSaveFile.CreateAccount(userID, username, "green");
                    } else if (args.ToLower().Contains("blue")) {
                        attempt = UtilSaveFile.CreateAccount(userID, username, "blue");
                    } else if (args.ToLower().Contains("yellow")) {
                        attempt = UtilSaveFile.CreateAccount(userID, username, "yellow");
                    } else if (args.ToLower().Contains("red")) {
                        attempt = UtilSaveFile.CreateAccount(userID, username, "red");
                    } else {
                        await ReplyAsync("If you are registering for an uno account, please specify a team: green, blue, red or yellow.");
                        return;
                    }
                    if (attempt) { await ReplyAsync("Your account has been created with Uno compatibility!"); } else { await ReplyAsync("You already have an account!"); }
                } else {
                        await ReplyAsync("If you are registering for an uno account, please specify a team: green, blue, red or yellow.");
                        return;
                    }
            } else {
                bool attempt = UtilSaveFile.CreateAccount(Context.User.Id.ToString(), Context.User.Username);
                if (attempt) { await ReplyAsync("Your account has been created!"); } else { await ReplyAsync("You already have an account!"); }
            }
        }
    }
}
