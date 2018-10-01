using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PrimaskiBot.Modules {
    public class EightBall : ModuleBase<SocketCommandContext> {
        string[] responses = { "Not a chance in hell.",
        "I mean, probably?",
        "Absolutely. 100% chance.",
        " *moans sexually* Sorry, I was distracted, can you ask it again?",
        "Ahahahaha. Ahahahaha. No.",
        "To be honest, that's a stupid question.",
        "YES!",
        "Only on Thursdays.",
        "I highly doubt it.",
        "I'm 99% sure the answer is yes.",
        "I'm 99% sure the answer is no.",
        "Why would you even ask such a thing?",
        "Hell yeah, my dude.",
        "Hell no, my dude."};

        [Command("8ball")]
        public async Task TaskAsync([Remainder] string args = null) {
            Random rando = new Random();
            int rand = rando.Next(0, responses.Length);
            if (args == null) {
                await ReplyAsync("You should probably ask the 8 ball something. It's not a mind reader. :eyes:");
            } else {
                await ReplyAsync(Context.User.Username + " is wondering: **" + args + "** \n\n" +
                    "Psychic Prima thinks: **" + responses[rand] + "**");
            }
        }
    }
}
