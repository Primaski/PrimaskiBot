using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Primaski_Bot.Modules {
    public static class UtilUno {
        static string userSavesPath = "..\\..\\save_files\\";
        static string ballog = "..\\..\\ballog.txt";
        static string buglog = "..\\..\\buglog.txt";
        static string maindir = "..\\..\\";

        /// <summary>
        /// Returns the ordered leaderboard of Uno players in the specified userSavesPath directory. Appends team scores underneath.
        /// </summary>
        public static List<string> GetLeaderboard(int startingIndex = 1) {
            if(startingIndex <= 0) {
                startingIndex = 1;
            }
            return RetrieveLeaderboardStats(startingIndex);
        }

        /// <summary>
        /// Pass in a username, returns a List(string), elem 0: player score, elem 1: ranking, elem 2: number of users total
        /// </summary>
        public static List<string> GetIndvidiualLeaderboardStats(string user) {
            List<User> unsortedLeaderboard = GetUsersUnoList();
            var sortedList = unsortedLeaderboard.OrderByDescending(f => f.unoPointsInt);
            user = UtilSaveFile.MakeUsernameCompatible(user);
            int i = 1;
            int placeTemp = -1;
            int playerScore = -1;
            foreach (User person in sortedList) {
                if (person.name == user) {
                    placeTemp = i;
                    playerScore = person.unoPointsInt;
                }
                i++;
            }
            if (playerScore == -1) {
                return new List<string>();
            }
            string placement = placeTemp.ToString();
            string noOfUsers = sortedList.Count().ToString();
            List<string> tempList = new List<string>();
            tempList.Add(playerScore.ToString());
            if (playerScore == 0) {
                tempList.Add(noOfUsers);
            } else {
                tempList.Add(placement);
            }
            tempList.Add(noOfUsers);
            return tempList;

        }

        /// <summary>
        /// Updates everyone's Uno points based on the server's algorithm for Uno Points. Returns List(string) of all updates performed / not performed.
        /// </summary>
        public static List<string> LogUnoGame(string args, string caller) {
            byte lengthOfCallCommand = 4; //log (with space following)
            args = args.TrimStart();
            args = args.Substring(lengthOfCallCommand, args.Length - lengthOfCallCommand);
            

            /*attempt to retrieve "number of players" who participated. assumes 99 is max possible. 
             * if no int exists, will assume equal to number of players who are listed.*/
            sbyte numberOfCountablePlayersStated = 0;
            if (Int32.TryParse(args.Substring(0, 1), out int ignore2)) {
                numberOfCountablePlayersStated = (sbyte)Int32.Parse(args.Substring(0, 1));
                bool tempbool = Int32.TryParse(args.Substring(0, 2), out int ignore);
                if (tempbool) {
                    sbyte tempval = (sbyte)Int32.Parse(args.Substring(0, 2));
                    if (tempval > 9 && tempval < 100) { //10+ players
                        numberOfCountablePlayersStated = tempval;
                    }
                }
            }

            //receive placement stats
            sbyte numberOfCountablePlayersListed = 0;
            List<string> orderedWinners = new List<string>();
            string winnerNo = "1";
            string currentWinner;

            //put winners inside a string list
            while (args.Contains(winnerNo + ".")) {
                numberOfCountablePlayersListed++;
                string followingVal = (numberOfCountablePlayersListed + 1).ToString();
                if (!args.Contains(followingVal + ".")) { //last countable player
                    currentWinner = args.Substring(args.IndexOf(winnerNo + "."));
                    currentWinner = currentWinner.Replace(winnerNo + ".", "");
                    currentWinner = UtilSaveFile.MakeUsernameCompatible(currentWinner);
                    orderedWinners.Add(currentWinner);

                    break;
                }
                if (args.IndexOf(winnerNo + ".") > args.IndexOf(followingVal + ".")) {
                    Console.WriteLine("At iteration " + numberOfCountablePlayersListed + ", " + winnerNo
                        + " came before " + followingVal);
                    return new List<string>();
                }
                string temp;
                currentWinner = args.Substring(args.IndexOf(winnerNo + "."), args.IndexOf(followingVal + ".")
                    - args.IndexOf(winnerNo + "."));
                currentWinner = currentWinner.Replace(winnerNo + ".", "");
                currentWinner = UtilSaveFile.MakeUsernameCompatible(currentWinner);
                orderedWinners.Add(currentWinner);
                int tempInt = Int32.Parse(winnerNo);
                tempInt++;
                winnerNo = tempInt.ToString();
            }
            if (numberOfCountablePlayersStated == 0) {
                numberOfCountablePlayersStated = numberOfCountablePlayersListed;
            } else if (numberOfCountablePlayersStated < numberOfCountablePlayersListed) {
                Console.WriteLine("More players were listed than the supposed number stated.");
                return new List<string>();
            }

            if (numberOfCountablePlayersStated < 2 || orderedWinners.Count() < 2) { //error checking
                return new List<string>();
            }
            //determine point attributes (1's are assumed to be given out to all remainder players)
            List<string> awards = new List<string>();
            awards.Add("5");
            if (numberOfCountablePlayersStated >= 3) {
                awards.Add("10");
            }
            //every 1-2 extra players gives a new first place award of +5
            byte newestElem = 10;
            for (int temp = numberOfCountablePlayersStated - 3; temp > 0; temp -= 2) {
                awards.Add((newestElem + 5).ToString());
                newestElem += 5;
            }

            //Add balances
            byte currWinner = 0;
            byte maxWinner = (byte)(orderedWinners.Count());
            sbyte currAward = (sbyte)(awards.Count() - 1);
            sbyte maxAward = -1; //when one ooint distrib should come in
            string returnedval = "";
            List<string> result = new List<string>();
            while (currWinner < maxWinner) {
                returnedval = UtilSaveFile.AddBalanceByUsername(orderedWinners[currWinner], awards[currAward],
                    "Unopoints", caller, 4);
                if (returnedval != "-1") {
                    result.Add(orderedWinners[currWinner] + " has earned " + awards[currAward] + " points," +
                        " bringing their total to " + returnedval + "!");
                    AddUnoWins(orderedWinners[currWinner], currWinner + 1,caller);
                } else {
                    result.Add(orderedWinners[currWinner] + " does not have an account, and their " +
                        awards[currAward] + " has not been added. Try deleting symbols that used to be emojis!");
                }
                currWinner++;
                currAward--;
                if (currAward == maxAward) {
                    while (currWinner < maxWinner) {
                        returnedval = UtilSaveFile.AddBalanceByUsername(orderedWinners[currWinner], "1",
                            "Unopoints", caller, 4);
                        if (returnedval != "-1") {
                            result.Add(orderedWinners[currWinner] + " has earned 1 point," +
                        " bringing their total to " + returnedval + "!");
                            AddUnoWins(orderedWinners[currWinner], currWinner + 1,caller);
                        } else {
                            result.Add(orderedWinners[currWinner] + " does not have an account, and their 1 point" +
                                " has not been added.");
                        }
                        currWinner++;
                    }
                }
            }

            return result;
        }

        public static bool AddUnoWins(string username, int placement, string caller) {
            username = UtilSaveFile.MakeUsernameCompatible(username);
            if(placement <= 0) {
                return false;
            }
            if(placement >= 4)
            {
                placement = 4;
            }
            switch (placement) {
                case 1: UtilSaveFile.AddBalanceByUsername(username, "1", "Unowins", "Gold bal change by " + caller, 4); break;
                case 2: UtilSaveFile.AddBalanceByUsername(username, "1", "Unosilvers", "Silver bal change by " + caller, 4); break;
                case 3: UtilSaveFile.AddBalanceByUsername(username, "1", "Unobronzes", "Bronze bal change by " + caller, 4); break;
                case 4: UtilSaveFile.AddBalanceByUsername(username, "1", "Unowhiteflags", "Standard win change by " + caller, 4); break;
                default: break;
            }
            return true;
        }

        internal static List<string> GetUsernameBySubstring(string v)
        {
            List<string> result = new List<string>();
            DirectoryInfo direct = new DirectoryInfo(userSavesPath);
            FileInfo[] filesInDir = direct.GetFiles(v+"*.txt");

            foreach(FileInfo x in filesInDir)
            {
                string xname = x.ToString();
                int countableIndex = -1;
                for(int i = xname.Length-1; i > 0; i--)
                {
                    if(xname[i] == '_')
                    {
                        countableIndex = i;
                        break;
                    }
                }
                result.Add(xname.Substring(0, countableIndex));
            }

            return result;
        }

        internal static List<string> GetUnoWins(string user) {
            string x = UtilSaveFile.GetBalanceByUsername(user, "Unowins");
            if (x == "-2"){
                return new List<string>(); //user doesn't exist
            }
            List<string> result = new List<string>();
            result.Add(":first_place: " + x);
            string y = UtilSaveFile.GetBalanceByUsername(user, "Unosilvers");
            result.Add(":second_place: " + y);
            string z = UtilSaveFile.GetBalanceByUsername(user, "Unobronzes");
            result.Add(":third_place: " + z);
            int total = Int32.Parse(x) + Int32.Parse(y) + Int32.Parse(z) + Int32.Parse(UtilSaveFile.GetBalanceByUsername(user, "Unowhiteflags"));
            result.Add("Total games played: **" + total.ToString() + "**");
            return result;
        }

        public static List<string> LogCAHGame(List<string> args, string caller) {
            
            byte lengthOfCallCommand = 4; //log (with space following)
            args[0] = args[0].TrimStart();
            args[0] = args[0].Substring(lengthOfCallCommand, args[0].Length - lengthOfCallCommand);

            //EXCLUDED USERS
            List<string> excludedUsers = new List<string>();
            if (Regex.Matches(args[0], @"[a-zA-Z]").Count >= 1) {
                if (args[0].Contains(",")) { //several usernames
                    excludedUsers = args[0].Split(',').ToList<string>();
                } else { //one username
                    excludedUsers.Add(args[0]);
                }
            }
            for(int i = 0; i < excludedUsers.Count; i++) {
                excludedUsers[i] = UtilSaveFile.MakeUsernameCompatible(excludedUsers[i]);
                Console.WriteLine(excludedUsers[i]);
            }

            //COLLECT USERS
            int noOfUsersPlayed = -1;
            List<string> includedUsers = new List<string>();
            foreach (string line in args) {
                if (line.Contains(":")) { //implies is a line with a scored user
                    includedUsers.Add(line);
                }
            }

            throw new NotImplementedException();

        }



        public static List<User> GetUsersUnoList() {
            //GET FILES
            DirectoryInfo direct = new DirectoryInfo(userSavesPath);
            FileInfo[] filesInDir = direct.GetFiles();
            List<String> fileLinksInDir = new List<string>();

            foreach (FileInfo local in filesInDir) {
                fileLinksInDir.Add(userSavesPath + local.ToString());
            }

            //GET USER STATS
            List<User> unsortedLeaderboard = new List<User>();
            foreach (string file in fileLinksInDir) {
                string pattern = "Unopoints:";
                string score = File.ReadAllLines(file).FirstOrDefault(l => l.Contains(pattern));
                string username = File.ReadAllLines(file).FirstOrDefault(m => m.Contains("Username:"));
                string team = File.ReadAllLines(file).FirstOrDefault(n => n.Contains("Unoteam:"));
                //-1 suggests that a player is not in the Uno server, and should not be counted
                if (!score.Contains("Unopoints:-1")) {
                    unsortedLeaderboard.Add(new User(username.Replace("Username:", ""), score.Replace("Unopoints:", ""),
                        team.Replace("Unoteam:", "")));
                }
            }
            return unsortedLeaderboard;
        }



        public static bool DoesMessageContainAllArgs(string botMessage, string[] args, string[] altArgs = null) {
            bool argsIsMissingArgument = false;

            for (int i = 0; i < args.Length; i++) {
                if (!botMessage.Contains(args[i])) {
                    Console.WriteLine("Could not find " + args[i]);
                    argsIsMissingArgument = true;
                    if (altArgs == null) {
                        return false;
                    }
                }
            }

            if (altArgs != null && argsIsMissingArgument) {
                for (int i = 0; i < altArgs.Length; i++) {
                    if (!botMessage.Contains(altArgs[i])) {
                        return false;
                    }
                }
            }

            return true;
        }


        /* PRIVATE INTERNAL METHODS */

        private static List<string> RetrieveLeaderboardStats(int startingIndex) {
            List<User> unsortedLeaderboard = GetUsersUnoList();
            //ORDER TEAM MEMBERS BY POINTS AND DERIVE TEAM STATS
            int blueTeam, greenTeam, yellowTeam, redTeam, adminTeam;
            int tempStore;
            blueTeam = greenTeam = yellowTeam = redTeam = adminTeam = 0;
            foreach (User player in unsortedLeaderboard) {
                if (!Int32.TryParse(player.unoPoints, out int ignore)) {
                    UtilSaveFile.ReportBug(player.name, "Critical Error in UtilUno: " + player.name +
                        "does not have a parsable Uno Score.");
                    return new List<string>();
                } else {
                    tempStore = Int32.Parse(player.unoPoints);
                }
                switch (player.team) {
                    case "blue":
                        blueTeam += tempStore;
                        continue;
                    case "green":
                        greenTeam += tempStore;
                        continue;
                    case "yellow":
                        yellowTeam += tempStore;
                        continue;
                    case "red":
                        redTeam += tempStore;
                        continue;
                    case "admin":
                        adminTeam += tempStore;
                        continue;
                    default: continue;
                }
            }
            //REWRITE LIST TO TEN WE WANT TO SHOW
            List<User> sortedList = new List<User>(unsortedLeaderboard.OrderByDescending(user => user.unoPointsInt));
            if (startingIndex > (sortedList.Count() - 10)) {
                startingIndex = sortedList.Count() - 10; //maximum starting index
            } else {
                startingIndex--; //humans count from 1
            }
            //remove all elements from sorted list except for 10 or less we want
            sortedList.RemoveRange(0, startingIndex);
            if (sortedList.Count() >= 10) {
                sortedList.RemoveRange(10, sortedList.Count() - 10);
            }
            //CREATE RESULTING LIST
            List<string> result = new List<string>();
            int playerno = 0;
            int currentRank = startingIndex + 1;
            result.Add("Overall scores:");
            int previousPlayerUnoPoints = -1;
            int previousPlayerUnoStanding = -1; //used in case of repeating standings of equal scores
            string teamcolor = "";

            while ((playerno < sortedList.Count())) {
                User currentPlayer = sortedList.ElementAt(playerno);
                //TryParse is not necessary since it was tested earlier
                int currPlayerUnoPoints = Int32.Parse(currentPlayer.unoPoints);
                if (currPlayerUnoPoints != 0 && currPlayerUnoPoints != -1) {
                    switch (currentPlayer.team) {
                        case ("green"): teamcolor = ":green_heart:"; break;
                        case ("blue"): teamcolor = ":blue_heart:"; break;
                        case ("red"): teamcolor = ":heart:"; break;
                        case ("yellow"): teamcolor = ":yellow_heart:"; break;
                        default: teamcolor = ":black_heart:"; break;
                    }
                    if (currPlayerUnoPoints == previousPlayerUnoPoints) {
                        result.Add((previousPlayerUnoStanding).ToString() + ". " + teamcolor + " " + currentPlayer.name + "- "
                    + currentPlayer.unoPoints + " points");
                        //not necessary to update previousplayerunostanding
                    } else {
                        result.Add((currentRank).ToString() + ". " + teamcolor + " " + currentPlayer.name + "- "
                            + currentPlayer.unoPoints + " points");
                        previousPlayerUnoStanding = currentRank;
                    }
                    previousPlayerUnoPoints = currentPlayer.unoPointsInt;
                }
                playerno++;
                currentRank++;
            }
            List<User> temp = new List<User>();
            temp.Add(new User("null", blueTeam.ToString(), ":blue_heart: **Blue Team"));
            temp.Add(new User("null", greenTeam.ToString(), ":green_heart: **Green Team"));
            temp.Add(new User("null", yellowTeam.ToString(), ":yellow_heart: **Yellow Team"));
            temp.Add(new User("null", redTeam.ToString(), ":heart: **Red Team"));
            temp.Add(new User("null", adminTeam.ToString(), ":black_heart: **Admins"));
            var teams = temp.OrderByDescending(x => x.unoPointsInt);

            result.Add(".==.==.==.==.==.==.==.==.==.==.");

            foreach (User team in teams) {
                result.Add(team.team + ":** " + team.unoPoints);
            }

            result.Add("Showing " + (startingIndex+1) + "-" + (startingIndex+10) + ". Type `p*uno lb (starting index)` for more");
            return result;
        }

        internal static void SaveLeaderboard() {
            List<string> previous = new List<string>();
            List<User> maxAmountUsers = GetUsersUnoList();
            int maxAmount = maxAmountUsers.Count();
            previous = GetLeaderboard();
            maxAmount -= 10;
            int currIndex = 11;
            while (maxAmount >= 1) {
                previous.Add("");
                previous.AddRange(GetLeaderboard(currIndex));
                maxAmount -= 10;
                currIndex += 10;
            }
            string targetFile = Path.Combine(maindir, "HistoricalUnoScore.txt");
            using (StreamWriter sw = new StreamWriter(targetFile)) {
                sw.WriteLine(DateTime.Now.Date + " " + DateTime.Now.TimeOfDay);
                foreach (string line in previous) {
                    sw.WriteLine(line);
                }
            }
            return;
        }
    }
}

public class User {
    public string name;
    public string unoPoints;
    public int unoPointsInt;
    public string team;
    public User(string name, string unoPoints, string team) {
        this.name = name;
        this.unoPoints = unoPoints;
        this.team = team;
        this.unoPointsInt = Int32.Parse(unoPoints);
    }
}
