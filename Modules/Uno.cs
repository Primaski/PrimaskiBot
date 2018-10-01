using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Primaski_Bot.Modules;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.IO;

namespace PrimaskiBot.Modules {
    public class Uno : ModuleBase<SocketCommandContext> {
        static string maindir = "..\\..\\";

        [Command("uno")]
        public async Task PingAsync([Remainder] string args = null) {

            bool isPointManager = false;
            var User = Context.User as SocketGuildUser;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Point Manager");

            if (User.Roles.Contains(role)) {
                isPointManager = true;
            }

            /* NORMAL USER COMMANDS */
            /* LEADERBOARD */

            if (!args.Contains("log")) {
                if (args.Contains("leaderboard") || args.Contains("lb")) {
                    bool digitIsPresent = args.Any(c => char.IsDigit(c));
                    List<string> leaderboard;
                    if (digitIsPresent) {
                        var tempArgs = args;
                        if (args.Contains("leaderboard")) {
                            tempArgs = args.Replace("leaderboard ", "");
                        } else {
                            tempArgs = args.Replace("lb ", "");
                        }
                        if (!Int32.TryParse(tempArgs, out int ignore)) {
                            await ReplyAsync("The correct format is: `p*uno leaderboard [(starting index)]`");
                            return;
                        }
                        int no = Int32.Parse(tempArgs);
                        if (no < 1) {
                            await ReplyAsync("Starting index cannot be less than 1.");
                            return;
                        }
                        leaderboard = UtilUno.GetLeaderboard(no);
                    } else {
                        leaderboard = UtilUno.GetLeaderboard();
                    }
                    if (leaderboard == null) {
                        await ReplyAsync("Critical Error. Bug will be reported.");
                    }
                    if (leaderboard.Count() < 1) {
                        await ReplyAsync("Critical Error. Bug will be reported.");
                    }

                    StringBuilder outputstringbuilder = new StringBuilder("");
                    foreach (var x in leaderboard) {
                        outputstringbuilder.AppendLine(x);
                    }

                    string outputText = outputstringbuilder.ToString();
                    await ReplyAsync(outputText);
                    return;
                }

                if (args.ToLower().Contains("wins")) {
                    string user;
                    bool refersToSelf = true;
                    if (Regex.Matches(args.Replace("wins", ""), @"[a-zA-Z]").Count >= 1) {
                        refersToSelf = false;
                        args = args.Replace("wins ", "");
                        args = UtilSaveFile.MakeUsernameCompatible(args);
                        user = args;
                    } else {
                        user = Context.User.Username;
                    }
                    List<string> result = UtilUno.GetUnoWins(user);
                    if (result.Count == 0 && refersToSelf) {
                        await ReplyAsync("An error was thrown. Do you have an account yet? Try typing: `p*create uno <team color>`.");
                        return;
                    } else if (result.Count == 0 && !refersToSelf) {
                        await ReplyAsync(user + " is not in the database!");
                        return;
                    }

                    if (refersToSelf) {
                        await ReplyAsync("Your Uno stats:");
                    } else {
                        await ReplyAsync(user + "'s Uno stats:");
                    }
                    StringBuilder outputstringbuilder = new StringBuilder("");
                    foreach (var x in result) {
                        outputstringbuilder.AppendLine(x);
                    }

                    string outputText = outputstringbuilder.ToString();
                    await ReplyAsync(outputText);
                    return;
                }

                /* CREATE UNO ACCOUNT (miscommand) */
                if (args.ToLower().Contains("create")) {
                    await ReplyAsync("Please type `p*create uno <team color>`, not `p*uno create <team color>`.");
                    return;
                }

                /* RETRIEVE USER SCORE / RANKING */
                if (args.ToLower().Contains("score")) {
                    string user;
                    bool refersToSelf = true;
                    if (Regex.Matches(args.Replace("score",""), @"[a-zA-Z]").Count >= 1) {
                        refersToSelf = false;
                        args = args.Replace("score ", "");
                        args = UtilSaveFile.MakeUsernameCompatible(args);
                        user = args;
                    } else {
                        user = Context.User.Username;
                    }
                    List<string> result = UtilUno.GetIndvidiualLeaderboardStats(user);
                    if (result.Count == 0 && refersToSelf) {
                        await ReplyAsync("An error was thrown. Do you have an account yet? Try typing: `p*create uno <team color>`.");
                        return;
                    } else if (result.Count == 0 && !refersToSelf){
                        await ReplyAsync(user + " is not in the database!");
                        return;
                    }
                    if (refersToSelf) {
                        await ReplyAsync("You currently have **" + result.ElementAt(0) + "** Uno points~");
                        if ((result.ElementAt(1) != "-1" && result.ElementAt(2) != "0")) {

                            if (result.ElementAt(1).EndsWith("1") && result.ElementAt(1) != "11") {
                                await ReplyAsync("You are in **" + result.ElementAt(1) + "st** place out of **" + result.ElementAt(2) + "** users!");
                                return;
                            } else if (result.ElementAt(1).EndsWith("2") && result.ElementAt(1) != "12") {
                                await ReplyAsync("You are in **" + result.ElementAt(1) + "nd** place out of **" + result.ElementAt(2) + "** users!");
                                return;
                            } else if (result.ElementAt(1).EndsWith("3") && result.ElementAt(1) != "13") {
                                await ReplyAsync("You are in **" + result.ElementAt(1) + "rd** place out of **" + result.ElementAt(2) + "** users!");
                                return;
                            }
                            await ReplyAsync("You are in **" + result.ElementAt(1) + "th** place out of **" + result.ElementAt(2) + "** users!");
                            return;
                        }
                    } else {
                        await ReplyAsync(user + " currently has **" + result.ElementAt(0) + "** Uno points~");
                        if ((result.ElementAt(1) != "-1" && result.ElementAt(2) != "0")) {

                            if (result.ElementAt(1).EndsWith("1") && result.ElementAt(1) != "11") {
                                await ReplyAsync("They are in **" + result.ElementAt(1) + "st** place out of **" + result.ElementAt(2) + "** users!");
                                return;
                            } else if (result.ElementAt(1).EndsWith("2") && result.ElementAt(1) != "12") {
                                await ReplyAsync("They are in **" + result.ElementAt(1) + "nd** place out of **" + result.ElementAt(2) + "** users!");
                                return;
                            } else if (result.ElementAt(1).EndsWith("3") && result.ElementAt(1) != "13") {
                                await ReplyAsync("They are in **" + result.ElementAt(1) + "rd** place out of **" + result.ElementAt(2) + "** users!");
                                return;
                            }
                            await ReplyAsync("They are in **" + result.ElementAt(1) + "th** place out of **" + result.ElementAt(2) + "** users!");
                            return;
                        }
                    }
                    return;
                }

                /* help menu (miscommand) */
                if (args.ToLower().Contains("help")) {
                    await ReplyAsync("Please type `p*help uno` instead of `p*uno help`.");
                    return;
                }

            }

            /* ADMIN FUNCTIONS */

            if (args.Contains("log")) {
                if (!isPointManager) {
                    await ReplyAsync("You are not authorized to log this game. Please contact" +
                        "a Point Manager (usually a Team Leader or Admin).");
                    return;
                }

                bool unoGame = UtilUno.DoesMessageContainAllArgs(args, new string[] { "has no more cards", "2. " }, new string[] {
                "no longer participating", "2. "});
                bool CAHGame = UtilUno.DoesMessageContainAllArgs(args, new string[] { "WON THE ", "ALL PRAISE" });

                if (!(unoGame || CAHGame)) {
                    await ReplyAsync("Improper format. Please ensure that you copy the entire message " +
                        "returned by the Uno bot. If you are logging a game that is not Uno, please note " +
                        "that this bot has not reached that functionality yet.");
                    return;
                }

                if (unoGame) {
                    List<string> result = UtilUno.LogUnoGame(args, Context.User.Username);
                    if (result.Count == 0) {
                        await ReplyAsync("Improper format. Please ensure that you copy the entire message " +
                            "returned by the Uno bot. If you are logging a game that is not Uno, please note " +
                            "that this bot has not reached that functionality yet.");
                        return;
                    }
                    string dogCsv = string.Join("\n", result.ToArray());
                    await ReplyAsync(dogCsv);
                    return;
                }
                if (CAHGame) {
                    List<string> argArr = new List<string>();
                    StringBuilder currentWritableLine = new StringBuilder();
                    char currentChar;
                    for(int i = 0; i < args.Length; i++) {
                        currentChar = args[i];
                        if(currentChar != '\n') {
                            currentWritableLine.Append(currentChar);
                        } else { //reached end of line, add it to argArr and reinitialize
                            argArr.Add(currentWritableLine.ToString());
                            currentWritableLine.Clear();
                        }
                    }
                    argArr.Add(currentWritableLine.ToString());

                    /*foreach(string arg in argArr) {
                        await ReplyAsync("Line: " + arg);
                    }*/

                    List<string> result = UtilUno.LogCAHGame(argArr, Context.User.Username);
                    return;
                    
                }
            }



        }
    }
}
