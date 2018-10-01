using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace PrimaskiBot.Modules {
    public class React : ModuleBase<SocketCommandContext> {
        [Command("hug")]
        public async Task Hug([Remainder] string args = null) {
            if (args == null) {
                await ReplyAsync("Aw! Here's a hug, " + Context.User.Username + "! \n" + "https://pre00.deviantart.net/9d78/th/pre/f/2018/191/d/e/magilou_about_to_hug_ya_by_buuwad-dcgt3rf.png");
            } else if ((args.ToLower()).Contains("primaski bot")) {
                await ReplyAsync("You... want me to hug myself...?");
            } else if ((args.ToLower()).Contains("prim")){
                await ReplyAsync("Thanks for showing Primaski love! Here you go, Prima! \n" + "https://pre00.deviantart.net/9d78/th/pre/f/2018/191/d/e/magilou_about_to_hug_ya_by_buuwad-dcgt3rf.png");
            } else {
                await ReplyAsync("Aw! Here's a hug, " + args + "! \n" + "https://pre00.deviantart.net/9d78/th/pre/f/2018/191/d/e/magilou_about_to_hug_ya_by_buuwad-dcgt3rf.png");
            }
        }

        [Command("slap")]
        public async Task Slap([Remainder] string args = null) {
            if (args == null) {
                await ReplyAsync("I mean, you didn't specify anyone, so I'm just going to slap you, " + Context.User.Username);
            } else if ((args.ToLower()).Contains((Context.User.Username).ToLower()) || args.Contains(Context.User.Mention) || args.Contains(Context.User.Id.ToString())) {
                await ReplyAsync("Why would you want me to slap you...? \n https://orig00.deviantart.net/cf61/f/2017/121/a/7/__litten_frightened___by_screinja_x-db7t8ml.gif");
                return;
            } else if ((args.ToLower()).Contains("prim")) {
                await ReplyAsync("I refuse to slap Primaski.");
                return;
            } else {
                await ReplyAsync(args + " has received a slap from " + Context.User.Username);
            }
            await ReplyAsync("https://pa1.narvii.com/5924/022c58d83ca01f3687246a1ad69b60d684a46398_hq.gif");
            return;
        }

        [Command("breed")]
        public async Task PingAsync([Remainder] string args = null) {
            Random rando = new Random();
            int rand = -1;
            if (args != null) {
                await ReplyAsync(Context.User.Username + " asks " + args + " for a night.");
                await Task.Delay(1000);
                rand = rando.Next(1, 101);
                if (rand < 51) {
                    await ReplyAsync(args + " is tired! Try again later.");
                } else {
                    await ReplyAsync(args + " licks their lips. " + args + " is ready.");
                    await Task.Delay(1000);
                    await ReplyAsync(Context.User.Username + " thrusts!");
                    await Task.Delay(1000);
                    await ReplyAsync(args + " thrusts!");
                    await Task.Delay(2000);
                    rand = rando.Next(1, 101);
                    if (rand < 90) {
                        await ReplyAsync("That was wonderful! However, you were unsuccessful at having a child.");
                    } else {
                        await ReplyAsync("That was wonderful! What's this? " + args + " is pregnant!");
                    }
                }
                return;
            }
            if (Context.User.Username == "Zizfotsys") {
                await ReplyAsync("Zizfotsys asks Primaski for a night.");
                await Task.Delay(1000);
                rand = rando.Next(1, 101);
                if (rand < 0) {
                    await ReplyAsync("Primaski is tired! Try again later.");
                } else {
                    await ReplyAsync("Primaski licks his lips. He's ready.");
                    await Task.Delay(1000);
                    await ReplyAsync("Zizfotsys thrusts!");
                    await Task.Delay(1000);
                    await ReplyAsync("Primaski thrusts!");
                    await Task.Delay(2000);
                    rand = rando.Next(1, 101);
                    if (rand < 0) {
                        await ReplyAsync("That was wonderful! However, you were unsuccessful at having a child.");
                    } else {
                        await ReplyAsync("That was wonderful! What's this? Zizfotsys is pregnant!");
                    }
                }
            } else if (Context.User.Username == "Primaski") {
                await ReplyAsync("Primaski asks Zizfotsys for a night.");
                await Task.Delay(1000);
                rand = rando.Next(1, 101);
                if (rand < 0) {
                    await ReplyAsync("Zizfotsys is tired! Try again later.");
                } else {
                    await ReplyAsync("Zizfotsys licks his lips. He's ready.");
                    await Task.Delay(1000);
                    await ReplyAsync("Primaski thrusts!");
                    await Task.Delay(1000);
                    await ReplyAsync("Zizfotsys thrusts!");
                    await Task.Delay(2000);
                    rand = rando.Next(1, 101);
                    if (rand < 0) {
                        await ReplyAsync("That was wonderful! However, you were unsuccessful at having a child.");
                    } else {
                        await ReplyAsync("That was wonderful! What's this? Primaski is pregnant!");
                    }
                }
            } else {
                await ReplyAsync("Excuse me, but who would breed with you? Put a name if you'd like to breed with someone!");
            }
        }
    }
}
