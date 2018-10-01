using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using System.IO;
using Primaski_Bot.Modules;

namespace PrimaskiBot.Modules {
    public class Balance : ModuleBase<SocketCommandContext> {
        string filepath = "..\\..\\balance.txt";
        int baseIncome = 100;

        [Command("bal")]
        public async Task PingAsync([Remainder] string args = null) {
            string userID = Context.User.Id.ToString();
            string balance = "-1";
            if (args == null) {
                balance = UtilSaveFile.GetBalanceById(userID, "Balance");

                if (balance == "-1") {
                    await ReplyAsync("Critical Error. Has been logged.");
                    return;
                } else if (balance == "-2") {
                    await ReplyAsync("Warning: You do not currently have a profile. If you would like to create one, please type `p\\*create`.");
                    return;
                }
                if (!Int32.TryParse(balance, out int ignore)) {
                    await ReplyAsync("Conversion to signed Int32 failed. Bug has been logged.");
                    await ReplyAsync("att: " + balance);
                    return;
                }
                await ReplyAsync("Your balance is: " + ":dollar: " + balance);
            } else {
                balance = UtilSaveFile.GetBalanceByUsername(args, "Balance");
                if (balance == "-1") {
                    await ReplyAsync("Critical Error. Has been logged.");
                    return;
                }else if(balance == "-2") {
                    await ReplyAsync("User " + args + " is not registered!");
                    return;
                } else {
                    await ReplyAsync(args + "'s balance is: :dollar: " + balance);
                }
            }
            return;
            
        }
    }
}
