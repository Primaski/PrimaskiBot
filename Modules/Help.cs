using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace PrimaskiBot.Modules {
    public class Help : ModuleBase<SocketCommandContext> {
        [Command("help")]
        public async Task PingAsync([Remainder] string args = null) {
            if(args == null) {
                await ReplyAsync("The help menu has changed! Please type one of the following commands:\n" +
                    ">**p\\*help fun** - General fun and static utilities like the 8 ball and copypastas.\n" +
                    ">**p\\*help adventure** - Adventures where you can go on quests and stuff.\n" +
                    ">**p\\*help uno** - For the uno server!\n" +
                    ">**p\\*help admin** - For the admins of the Uno server!\n"+
                    ">**p\\*help bot** - General bot commands.");
                return;
            }
            if(args.ToLower().Contains("fun")) {
                await ReplyAsync("\n***Fun Commands:***\n" +
                "**>8ball [question]** : Ask Psychic Primaski a question!\n" +
                "**>navyseal, lenny**: Copypastas\n" +
                "**>bananaboy**: Read Abs' pride and joy novel. You won't regret it.\n" +
                "**>hug [(name)]**\n" +
                "**>slap [(name)]**\n" +
                "**>breed [name]**\n");
                return;
            }
            if (args.ToLower().Contains("adventure")) {
                await ReplyAsync("\n***Adventure commands***:\n" +
                    "**>create**: Create an account for adventures! Logs Username and Discord ID.\n" +
                "**>create uno <team color>**: Create an account for adventures and the Uno server! (necessary for logging scores)\n" +
                "**>gooutside**: What mysteries await you outdoors?\n" +
                "**>friends** : See what my friends are doing right now!\n" +
                "**>event**: RPG events! (under construction)\n" +
                "**>bal**: Check your coin balance\n" +
                "**>daily**: Earn your allowance (under construction)");
                return;
            }
            if (args.ToLower().Contains("uno")) {
                await ReplyAsync("\n***Uno commands***:\n" +
                    "**>create uno <team color>:** Create an account for adventures and the Uno server! (necessary for logging scores)\n" +
                    "**>uno leaderboard ([starting index])**: Show user and team rankings.\n" +
                    "**>uno score ([user])**: Show your current Uno score and leaderboard ranking!\n" +
                    "**>uno wins ([user])**: Show how many gold, silver and bronze medals you've won! Tracking started on fifth fortnight.\n" +
                    "\n*(If you're a Point Manager looking for a specific command, please look at the pinned messages in #team-leaders.)*");
                    return;
            }
            if (args.ToLower().Contains("admin"))
            {
                await ReplyAsync("\n***Uno admin commands***:\n" +
                    "**>uno log (<number of joiners>) <winning message>**: Log Uno games to add both points and wins!\n" +
                    "**>setuno (user) (amount)**: SETS aa score to an individual user (the 'all' command can be used to set all users to a score). Note wins will not be tracked.\n" +
                    "**>adduno (user) (amount)**: ADDS a score to an individual user\n" +
                    "**>unosearch (letter)** : Searches all users in the database that begin with a certain letter.\n" +
                    "**>addgold, addsilver, addbronze, addwin (user)**: Adds one win exactly to a specified user. Gold for 1st place, silver 2nd, bronze 3rd, win for any other win (adds to total). Should" +
                    "only be used with adduno, since uno log automatically grants wins.\n"
                    );
                return;
            }
            if (args.ToLower().Contains("bot")) {
                await ReplyAsync("\n***General Bot commands***:\n" +
               "**>help**, **say**, **suggest**, **info**, **ping**\n");
               return;
            }
            await ReplyAsync("Not a recognized command. Try \"help fun, help adventure, help uno, or help bot");
            return;
        }
        
    }
}
