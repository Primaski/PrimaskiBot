using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using System.IO;
using Primaski_Bot;
using Primaski_Bot.Modules;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace PrimaskiBot.Modules {
    public class Admin : ModuleBase<SocketCommandContext> {
        string filepath = "..\\..\\balance.txt";
        string improperFormatSetBal = "The proper format is `setbal/setuno/addbal/adduno user amount`";
        Program s;

        //WRITE IN ORDER OF ADMIN + COMMAND
        [Command("setuno")]
        public async Task SetUnoScore([Remainder] string args = null) {

            var User = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Point Manager");
            if (!User.Roles.Contains(role)) {
                await ReplyAsync("You do not have permission to set the balance of others.");
                return;
            }
            if ((args.Length - 3) < 1) {
                await ReplyAsync("Args: " + args + ", Args Length: " + args.Length);
                await ReplyAsync("At line 33: " + improperFormatSetBal);
                return;
            }
            if ((args.Length - 3) < 1) {
                Console.WriteLine(args);
                await ReplyAsync("At line 40: " + improperFormatSetBal);
                return;
            }
            string user, amountStr;
            int division = -1;
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == ' ')
                    division = i;
            }

            if (division == -1) {
                await ReplyAsync("At line 53: " + improperFormatSetBal);
                return;
            }
            user = args.Substring(0, division);
            amountStr = args.Substring(division + 1, args.Length - (division + 1));

            if (!Int32.TryParse(amountStr, out int ignore)) {
                await ReplyAsync("At line 61: " + improperFormatSetBal);
                return;
            }

            string attempt = "-1";

            attempt = UtilSaveFile.SetBalanceByUsername(user, amountStr, "Unopoints", Context.User.Username,
                5);

            if (attempt == "-1") {
                await ReplyAsync("Error: Either " + user + " was not found, or the amount was incorrect. Changes were not performed. Try capitalizing where necessary.");
                return;
            } else {
                if (user != "all") {
                    await ReplyAsync(user + "'s Uno Score was successfully changed to " + attempt);
                } else {
                    await ReplyAsync("Everyone's Uno Score has been set to " + amountStr);
                }
            }
            return;
        }

        [Command("adduno")]
        public async Task AddUnoScore([Remainder] string args = null) {

            var User = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Point Manager");
            if (!User.Roles.Contains(role)) {
                await ReplyAsync("You do not have permission to set the balance of others.");
                return;
            }

            if ((args.Length - 3) < 1) {
                Console.WriteLine(args);
                await ReplyAsync("At line 98: " + improperFormatSetBal);
                return;
            }

            string user, amountStr;
            int division = -1;
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == ' ')
                    division = i;
            }
            
            if (division == -1) {
                await ReplyAsync("At line 110: " + improperFormatSetBal);
                return;
            }
            user = args.Substring(0, division);
            amountStr = args.Substring(division + 1, args.Length - (division + 1));

            if (!Int32.TryParse(amountStr, out int ignore)) {
                await ReplyAsync("At line 117: " + improperFormatSetBal);
                return;
            }

            string attempt = "-1";

            attempt = UtilSaveFile.AddBalanceByUsername(user, amountStr, "Unopoints", Context.User.Username, 6);

            if (attempt == "-1") {
                await ReplyAsync("Error: Either " + user + " was not found, or the amount was incorrect. Changes were not performed.");
                return;
            } else {
                if (user != "all") {
                    await ReplyAsync(user + "'s Uno Score was successfully changed to " + attempt);
                } else {
                    await ReplyAsync("Everyone's Uno Score has been set to " + amountStr);
                }
            }
            return;

        }

        [Command("logleaderboard")]
        public async Task LogLeaderboard() {
            var User = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Point Manager");
            if (!User.Roles.Contains(role)) {
                await ReplyAsync("You do not have permission to log the leaderboard.");
                return;
            }

            UtilUno.SaveLeaderboard();
            await ReplyAsync("Success. Please see file path: " + "*\\Primaski Bot\\Primaski Bot\\HistoricalUnoScore.txt. Type r to revert changes." +
                " (note: will not throw success message)");
            return;
        }

        [Command("addgold")]
        public async Task AddGold([Remainder] string args = null){
            var User = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Point Manager");
            if (!User.Roles.Contains(role)) {
                await ReplyAsync("You do not have permission to set the balance of others.");
                return;
            }

            args = UtilSaveFile.MakeUsernameCompatible(args);
            string attempt = UtilSaveFile.AddBalanceByUsername(args,"1", "Gold bal change by " + Context.User.Username, Context.User.Username,6);
            if (attempt == "-1"){
                await ReplyAsync("Attempt was unsuccessful. Either " + args + " was not found in the database, or the format is wrong." +
                    "\n\n The proper format is: `p*addgold <User>`. \n\n One is always added by default.");
            } else {
                await ReplyAsync("Attempt was successful. " + args + " has received 1 gold win, making their new total " + attempt);
            }
        }

        [Command("addsilver")]
        public async Task AddSilver([Remainder] string args = null){
            var User = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Point Manager");
            if (!User.Roles.Contains(role)){
                await ReplyAsync("You do not have permission to set the balance of others.");
                return;
            }

            args = UtilSaveFile.MakeUsernameCompatible(args);
            string attempt = UtilSaveFile.AddBalanceByUsername(args, "1", "Unosilvers", "Silver bal change by " + Context.User.Username, 6);
            if (attempt == "-1"){
                await ReplyAsync("Attempt was unsuccessful. Either " + args + " was not found in the database, or the format is wrong." +
                    "\n\n The proper format is: `p*addsilver <User>`. \n\n One is always added by default.");
            }else{
                await ReplyAsync("Attempt was successful. " + args + " has received 1 silver win, making their new total " + attempt);
            }
        }

        [Command("addbronze")]
        public async Task AddBronze([Remainder] string args = null) {
            var User = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Point Manager");
            if (!User.Roles.Contains(role)) {
                await ReplyAsync("You do not have permission to set the balance of others.");
                return;
            }

            args = UtilSaveFile.MakeUsernameCompatible(args);
            string attempt = UtilSaveFile.AddBalanceByUsername(args, "1", "Unobronzes", "Bronze bal change by " + Context.User.Username, 6);
            if (attempt == "-1"){
                await ReplyAsync("Attempt was unsuccessful. Either " + args + " was not found in the database, or the format is wrong." +
                    "\n\n The proper format is: `p*addbronze <User>`. \n\n One is always added by default.");
            }else{
                await ReplyAsync("Attempt was successful. " + args + " has received 1 bronze win, making their new total " + attempt);
            }
        }

        [Command("addwin")]
        public async Task AddWin([Remainder] string args = null){
            var User = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Point Manager");
            if (!User.Roles.Contains(role)){
                await ReplyAsync("You do not have permission to set the balance of others.");
                return;
            }

            args = UtilSaveFile.MakeUsernameCompatible(args);
            string attempt = UtilSaveFile.AddBalanceByUsername(args, "1", "Unowhiteflags", "Win bal change by " + Context.User.Username, 6);
            if (attempt == "-1"){
                await ReplyAsync("Attempt was unsuccessful. Either " + args + " was not found in the database, or the format is wrong." +
                    "\n\n The proper format is: `p*addgold <User>`. \n\n One is always added by default.");
            } else{
                await ReplyAsync("Attempt was successful. " + args + " has received 1 general win, making their new total " + attempt);
            }
        }

        [Command("unosearch")]
        public async Task SearchByUsername([Remainder] string args = null) {
            args = args.Replace("unosearch ","");
            args = args.Substring(0, 1);

            List<string> results = UtilUno.GetUsernameBySubstring(Regex.Match(args, @"[a-z|A-Z]").ToString());

            if (results.Count() == 0) {
                await ReplyAsync("No usernames begin with " + args);
            }else{
                StringBuilder currentWritableLine = new StringBuilder();
                foreach(string x in results){
                    currentWritableLine.Append(x + "\n");
                }
                string res = currentWritableLine.ToString();
                await ReplyAsync("Usernames in the database that begin with " + args + ":");
                await ReplyAsync(res);
            }
        }
    }
}
