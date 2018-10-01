using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace PrimaskiBot.Modules {
    public class Zeastereggs : ModuleBase<SocketCommandContext> {
        [Command("easter")]
        public async Task EasterEgg() {
            await ReplyAsync("Yes, you have found an Easter Egg. I bet you feel special.");
        }

        [Command("easteregg")]
        public async Task EasterEggSecond() {
            await ReplyAsync("Yes, you have found an Easter Egg. I bet you feel special.");
        }

        [Command("stayinside")]
        public async Task StayInside() {
            await ReplyAsync("You stay inside. You have a good day as a result.");
            return;
        }

        [Command("ban")]
        public async Task BanUser([Remainder] string args = null) {
            if (args != null) {
                await ReplyAsync("Banned user: **" + args + "** :ok_hand:");
            } else {
                await ReplyAsync("Who should I ban?");
            }
            return;
        }

        [Command("binary")]
        public async Task ConvertToBinary([Remainder] string args = null) {
            if (args.ToLower().Contains("ascii")) {
                byte[] bytes = Encoding.ASCII.GetBytes(args.Replace("ascii ", ""));
                StringBuilder result = new StringBuilder();
                foreach (byte b in bytes) {
                    result.Append(Convert.ToString(b, 2));
                }
                string final = result.ToString();
                await ReplyAsync(final);
                return;
            }
            if (!Int32.TryParse(args, out int ignore)) {
                await ReplyAsync(args + " is not a number");
                return;
            }
            string binary = Convert.ToString(Int32.Parse(args), 2);
            await ReplyAsync(binary);
        }

        [Command("leaderboard")]
        public async Task MistypeLeaderboard() {
            await ReplyAsync("Were you looking for the leaderboard? Try typing: `p*uno leaderboard` instead.");
        }

        [Command("lb")]
        public async Task MistypeLb() {
            await ReplyAsync("Were you looking for the leaderboard? Try typing: `p*uno lb` instead.");
        }

        [Command("score")]
        public async Task Mistypescore() {
            await ReplyAsync("Were you looking for your score? Try typing: `p*uno score` instead.");
        }

        [Command("wins")]
        public async Task Mistypewins()
        {
            await ReplyAsync("Were you looking for your wins? Try typing: `p*uno wins` instead.");
        }

        [Command("catch")]
        public async Task Mistypepoke([Remainder] string args = null) {
            if (args == null) {
                args = "pokemon";
            }
                await ReplyAsync("I'm not the Pokecord bot, haha! Maybe you'll have more luck finding your " + args + " if you do `p!catch`!");
            return;
        }

        [Command("dab")]
        public async Task Dab([Remainder] string args = null){
            await ReplyAsync("https://steamuserimages-a.akamaihd.net/ugc/872998007404242358/0B5020C9375C74493C763E7F489694091311EDD1/");
            return;
        }

        [Command("bananaboy")]
        public async Task Bananaboy() {
            await ReplyAsync("i work ever day and peel banana. it is the only thing i know how to do.");
            await Task.Delay(2000);
            await ReplyAsync("i start at young age, picking up banana from market and taking it home to mom.");
            await Task.Delay(2000);
            await ReplyAsync("\"look mom\" i say in excitement.");
            await Task.Delay(2000);
            await ReplyAsync("\"banana\" my mom agrees, tears of excitement running down her face.");
            await Task.Delay(2000);
            await ReplyAsync("it was mom who taught me to peel banana. she cry every time.");
            await Task.Delay(2000);
            await ReplyAsync("every day mom cry.");
            await Task.Delay(2000);
            await ReplyAsync("one day i bring home banana, mom is not there.");
            await Task.Delay(2000);
            await ReplyAsync("my nine brothers are not there.");
            await Task.Delay(2000);
            await ReplyAsync("there is a note on the kitchen table. i stare at the words.");
            await Task.Delay(2000);
            await ReplyAsync("it is too bad that i can not read.");
            await Task.Delay(2000);
            await ReplyAsync("the only thing i know is banana.");
            await Task.Delay(2000);
            await ReplyAsync("i wait all night. i peel banana.");
            await Task.Delay(2000);
            await ReplyAsync("mom does not come back.");
            await Task.Delay(2000);
            await ReplyAsync(" next morning i find more banana. i come home and there is new family in my home.");
            await Task.Delay(2000);
            await ReplyAsync("they say they don't want banana.");
            await Task.Delay(2000);
            await ReplyAsync("i peel banana. new family throws me out.");
            await Task.Delay(2000);
            await ReplyAsync("the must not like banana. i am alone.");
            await Task.Delay(2000);
            await ReplyAsync("i only have banana in this world. it is all i need.");
            await Task.Delay(2000);
            return;
        }
    }
}