using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Primaski_Bot.Modules {
    public static class UtilSaveFile {
        static string userSavesPath = "..\\..\\save_files\\";
        static string ballog = "..\\..\\ballog.txt";
        static string buglog = "..\\..\\buglog.txt";
        static string maindir = "..\\..\\";
        static int baseIncome = 100;


        static string[] defaultProfileCreds =
            {"Balance:100","Points:0","Daily:0","Unopoints:-1","Unoteam:-1","Unowins:-1","Unosilvers:-1","Unobronzes:-1","Unowhiteflags:-1"};
        static string[] defaultProfileCredsUno =
            {"Balance:100","Points:0","Daily:0","Unopoints:0","Unowins:0","Unosilvers:0","Unobronzes:0","Unowhiteflags:0"};
        static string[] scoreChangeProcedures =
            {"ADVENTURES Daily", "ADVENTURES Adventure", "ADVENTURES Administrator Set Score", "ADVENTURES Administrator Add Score",
            "UNO Standard Game Log", "UNO Administrator Set Score", "UNO Administrator Add Score",
             };


        static string[] illegalChars = {"<", ">", ":", "\"", "/", "\\", "?", "!", "*", "."};


        /* PUBLIC METHODS */

        /// <summary>
        /// Creates a save file for the user specified. If the Uno field is initialized, then the account will have Uno compatibility, and it should be filled with the team color.
        /// </summary>
        internal static bool CreateAccount(string id, string username, string uno = "") {
            username = MakeUsernameCompatible(username);
            string expectedPath = userSavesPath + username + "_" + id + ".txt";
            if (File.Exists(expectedPath)) {
                Console.WriteLine(username + " already has a file. New account not created.");
                return false;
            }

            var fs = File.Create(expectedPath);
            fs.Close(); //closes StreamWriter that File.Create made by default
            StreamWriter sw = new StreamWriter(expectedPath);
            sw.WriteLine("Username:" + username);
            sw.WriteLine("UserID:" + id);

            if (uno == "") {
                foreach (string x in defaultProfileCreds) { //global var stores save file content
                    sw.WriteLine(x);
                }
            } else {
                foreach (string x in defaultProfileCredsUno) { //global var stores save file content
                    sw.WriteLine(x);
                }
                uno = uno.ToLower();
                sw.WriteLine("Unoteam:" + uno.Replace("uno ",""));
            }
            sw.Close();
            Console.WriteLine(username + " made an account");
            return true;
        }

        /// <summary>
        /// Pass in the user ID, along with kind of balance found in save file (example: "Unopoints"). Returns balance string. -1 if error, -2 if user not found.
        /// </summary>
        public static string GetBalanceById(string userID, string balanceType) {
            //GET LIST OF FILES WITH ID IN DIRECTORY
            DirectoryInfo direct = new DirectoryInfo(userSavesPath);
            FileInfo[] filesInDir = direct.GetFiles("*" + userID + "*.*");
            if(filesInDir.Length == 0) {
                Console.WriteLine("Attempted to get balance by ID, user not found.");
                return "-2";
            }
            foreach (FileInfo foundFile in filesInDir) {
                string fullName = foundFile.FullName;
            }
            if(filesInDir.Length > 1) { //for usernames, this is fine. for ID's, this suggests a program mishap
                ReportBug(userID, "Critical Error: Two user save files found with the same ID."); return "-1";
            }

            //FIND THE INFORMATION INSIDE THE FILE
            string targetFile = userSavesPath + filesInDir[0].ToString();
            string balanceLine = "";
            string temp;
            using (StreamReader str = new StreamReader(targetFile)) {
                while (!str.EndOfStream) {
                    temp = str.ReadLine();
                    if (temp.Contains(balanceType + ":")) {
                        balanceLine = temp;
                    }
                }
            }
            if(balanceLine == "") {
                ReportBug(userID, "Critical Error: User does not have a " + balanceType + " line in their save file."); return "-1";
            }

            //EXTRACT AND RETURN INFORMATION
            balanceLine = balanceLine.Replace(balanceType + ":", "");
            if(!Int32.TryParse(balanceLine,out int ignore)){
                ReportBug(userID, "Critical Error: Unable to parse balance."); return "-1";
            }
            return balanceLine;
        }


        /// <summary>
        /// Pass in the username, along with kind of balance found in save file (example: "Unopoints"). Returns balance string. -1 if error, -2 if user not found.
        /// </summary>
        public static string GetBalanceByUsername(string username, string balanceType) {
            //GET LIST OF FILES WITH ID IN DIRECTORY
            username = MakeUsernameCompatible(username);
            DirectoryInfo direct = new DirectoryInfo(userSavesPath);
            FileInfo[] filesInDir = direct.GetFiles(username + "*");
            if (filesInDir.Length == 0) {
                Console.WriteLine("Attempted to get balance by username, not found.");
                return "-2";
            }
            foreach (FileInfo foundFile in filesInDir) {
                string fullName = foundFile.FullName;
            }
            if (filesInDir.Length > 1) { //TO-DO
                ReportBug(username, "Error: Implement ability to scroll through identical usernames"); return "-1";
            }

            //FIND THE INFORMATION INSIDE THE FILE
            string targetFile = userSavesPath + filesInDir[0].ToString();
            string balanceLine = "";
            string temp;
            using (StreamReader str = new StreamReader(targetFile)) {
                while (!str.EndOfStream) {
                    temp = str.ReadLine();
                    if (temp.Contains(balanceType + ":")) {
                        balanceLine = temp;
                    }
                }
            }
            if (balanceLine == "") {
                ReportBug(username, "Critical Error: User does not have a " + balanceType +" line in their save file."); return "-1";
            }

            //EXTRACT AND RETURN INFORMATION
            balanceLine = balanceLine.Replace(balanceType + ":", "");
            if (!Int32.TryParse(balanceLine, out int ignore)) {
                ReportBug(username, "Critical Error: Unable to parse balance."); return "-1";
            }
            return balanceLine;
        }

        /// <summary>
        /// Changes user's balance directly to a number. Pass in the username, along with kind of balance found in save file (example: "Unopoints").
        /// Purpose can be found in this files "ScoreChangeProcedures". Returns "-1" if error, "-2" if user not found, else the new balance as a string.
        /// </summary>
        public static string SetBalanceByUsername(string user, string amount, string balanceType, string caller, byte purpose) {
            //GET LIST OF FILES WITH ID IN DIRECTORY
            if (!Int32.TryParse(amount, out int ignore)) {
                ReportBug("all", "Not a valid amount " + amount);
                return "-1";
            } else if (Int32.Parse(amount) < 0) {
                ReportBug("all", "Not a valid amount " + amount);
                return "-1";
            }
            if (user == "all") {
                bool worked = SetBalanceAll(amount, balanceType, caller, purpose);
                if (!worked) {
                    ReportBug("all users", "Could not reset all user's scores to 0.");
                    return "-1";
                }
                return amount;
            }
            user = MakeUsernameCompatible(user);
            DirectoryInfo direct = new DirectoryInfo(userSavesPath);
            FileInfo[] filesInDir = direct.GetFiles(user + "*");
            if (filesInDir.Length == 0) {
                Console.WriteLine("Couldn't find file for " + user + " in set balance by username");
                return "-1";
            }
            foreach (FileInfo foundFile in filesInDir) {
                string fullName = foundFile.FullName;
            }
            if (filesInDir.Length > 1) { //TO-DO
                ReportBug(user, "Error: Implement ability to scroll through identical usernames"); return "-1";
            }

            //FIND THE INFORMATION INSIDE THE FILE
            string targetFile = userSavesPath + filesInDir[0].ToString();
            string balanceLine = "";
            string temp;
            short lineToEdit = 1;
            using (StreamReader str = new StreamReader(targetFile)) {
                while (!str.EndOfStream) {
                    temp = str.ReadLine();
                    if (temp.Contains(balanceType + ":")) {
                        balanceLine = temp;
                        break;
                    }
                    lineToEdit++;
                }
            }
            if (balanceLine == "") {
                ReportBug(user, "Critical Error: User does not have a " + balanceType + " line in their save file."); return "-1";
            }

            //MODIFY INFORMATION, WRITE NEW FILE OVER OLD FILE

            string balanceAmount = balanceLine.Replace(balanceType + ":", "");
            if (!Int32.TryParse(balanceAmount, out int ignore2)) {
                ReportBug(user, "Not a valid balance: " + balanceAmount + ".");
                return "-1";
            }

            ReportScoreChange(caller, user, balanceAmount, amount, purpose);

            String replacedLine = balanceType + ":" + amount;
            string[] lines = File.ReadAllLines(targetFile);
            using (StreamWriter writer = new StreamWriter(targetFile)) {
                for (int curr = 1; curr <= lines.Length; curr++) {
                    if (curr == lineToEdit) {
                        writer.WriteLine(replacedLine);
                    } else {
                        writer.WriteLine(lines[curr - 1]);
                    }
                }
            }

            return amount;
        }

        /// <summary>
        /// Takes specified user's balance, and adds the amount to it. Pass in the username, along with kind of balance found in save file (example: "Unopoints"). 
        /// Purpose can be found in this files "ScoreChangeProcedures". Returns "-1" if error, "-2" if user not found, else the new balance as a string.
        /// </summary>
        internal static string AddBalanceByUsername(string user, string amount, string balanceType, string caller, byte purpose) {
            //GET LIST OF FILES WITH ID IN DIRECTORY
            if (!Int32.TryParse(amount, out int ignore2)) {
                ReportBug(user, "Not a valid amount: " + amount);
                return "-1";
            }
            int amountInt = Int32.Parse(amount);
            if (user == "all") {
                //not yet implemented
                return "-1";
            }
            user = MakeUsernameCompatible(user);
            DirectoryInfo direct = new DirectoryInfo(userSavesPath);
            FileInfo[] filesInDir = direct.GetFiles(user + "*");
            if (filesInDir.Length == 0) {
                Console.WriteLine("Attempted to add balance by username. Files not found.");
                return "-1";
            }
            foreach (FileInfo foundFile in filesInDir) {
                string fullName = foundFile.FullName;
            }
            if (filesInDir.Length > 1) { //TO-DO
                ReportBug(user, "Error: Implement ability to scroll through identical usernames"); return "-1";
            }
            //FIND THE INFORMATION INSIDE THE FILE
            string targetFile = userSavesPath + filesInDir[0].ToString();
            string balanceLine = "";
            string temp;
            short lineToEdit = 1;
            using (StreamReader str = new StreamReader(targetFile)) {
                while (!str.EndOfStream) {
                    temp = str.ReadLine();
                    if (temp.Contains(balanceType + ":")) {
                        balanceLine = temp;
                        break;
                    }
                    lineToEdit++;
                }
            }
            if (balanceLine == "") {
                ReportBug(user, "Critical Error: User does not have a " + balanceType + " line in their save file."); return "-1";
            }
            //MODIFY INFORMATION, WRITE NEW FILE OVER OLD FILE

            string balanceAmount = balanceLine.Replace(balanceType + ":", "");
            if (!Int32.TryParse(balanceAmount, out int ignore)) {
                ReportBug(user, "Not a valid balance: " + balanceAmount + ".");
                return "-1";
            }
            int balance = Int32.Parse(balanceAmount);
            string originalVal = balance.ToString(); //original value is stored for reporting
            if (balance + amountInt < 0) {
                ReportBug(user, "Not a valid amount: " + (balance + amountInt));
                return "-1";
            }
            balance += amountInt;
            if (balance < 0 || balance > Int32.MaxValue) {
                ReportBug(user, "Not a valid amount: " + balance);
                return "-1";
            }
            balanceAmount = balance.ToString();

            ReportScoreChange(caller, user, originalVal, balanceAmount, purpose);

            String replacedLine = balanceType + ":" + balanceAmount;
            string[] lines = File.ReadAllLines(targetFile);
            using (StreamWriter writer = new StreamWriter(targetFile)) {
                for (int curr = 1; curr <= lines.Length; curr++) {
                    if (curr == lineToEdit) {
                        writer.WriteLine(replacedLine);
                    } else {
                        writer.WriteLine(lines[curr - 1]);
                    }
                }
            }
            return balanceAmount;
        }

        internal static void ReportBug(string userID, string message) {
            StreamWriter swc = File.AppendText(buglog);
            swc.WriteLine(DateTime.Now.Date + "\t" + DateTime.Now.TimeOfDay +
                userID + "\t" + message);
            swc.WriteLine();
            swc.Close();
            return;
        }

        internal static void ReportScoreChange(string pointManagerUsername, string userModified, string amountFrom,
            string amountTo, byte procedureID) {
            string procedure = scoreChangeProcedures[procedureID];
            StreamWriter swc = File.AppendText(ballog);
            swc.WriteLine(DateTime.Now.Date + DateTime.Now.TimeOfDay + " - " + procedure + ":");
            swc.WriteLine(pointManagerUsername + " has changed " + userModified + "'s score from "
                + amountFrom + " to " + amountTo);
            swc.WriteLine();
            swc.Close();
            return;
        }

        /// <summary>
        /// Appends a single string line to the end of a save file or set of save file. Please specify the ENTIRE query for the line.
        /// </summary>
        internal static bool AddLineToSaveFile(string line, string user, bool onlyUnoUsers = false) {
            //RETRIEVE FILES
            if(user != "all") {
                throw new NotImplementedException();
            }
            DirectoryInfo direct = new DirectoryInfo(userSavesPath);
            FileInfo[] filesInDir = direct.GetFiles();
            if (filesInDir.Length == 0) {
                Console.WriteLine("Attempted to set balance for all, files not found.");
                return false;
            }
            List<string> fileLinksInDir = new List<string>();

            foreach (FileInfo file in filesInDir) {
                fileLinksInDir.Add(userSavesPath + file.ToString());
            }

            List<string> tempo = new List<string>();
            bool uno = false;
            //REMOVE UNNECESSARY FILES IF IT'S AN UNOPOINT COMMAND
            if (onlyUnoUsers) {
                uno = true;
                foreach (string file in fileLinksInDir) {
                    string score = File.ReadAllLines(file).FirstOrDefault(l => l.Contains("Unopoints:"));
                    //-1 suggests that a player is not in the Uno server, and should not be counted
                    if (!score.Contains("Unopoints:-1")) {
                        tempo.Add(file);
                    }
                }
            }
            if (uno) {
                fileLinksInDir = tempo;
            }

            for (int i = 0; i < fileLinksInDir.Count(); i++) {
                string targetFile;
                targetFile = Path.Combine(userSavesPath, fileLinksInDir[i]);

                using (StreamWriter writer = new StreamWriter(targetFile)) {
                    writer.WriteLine(line);
                }
            }
            return true;
        }


        /// <summary>
        /// Turns Username into plain ASCII text, with spaces as underscores, and edges trimmed off. Necessary to be called for Save File retrieval or creation.
        /// </summary>
        public static string MakeUsernameCompatible(string username) {
            //please put a fucking ASCII character in your name
            if (username.Contains("รђ"))
            {
                return "Shadow Stalker";
            }
            username = Regex.Replace(username, @"[^\u0000-\u007F]+", "");
            string temp;
            temp = username.Trim();
            username = temp;
            if (username.Contains(" ")) {
                username = Regex.Replace(username, " ", "_");
            }
            username = Regex.Replace(username, "[^a-zA-Z0-9_]+", "", RegexOptions.Compiled);
            if (username.Contains("|")) {
                string tamp;
                tamp = username.Replace("|", "");
                username = tamp;
            }
            bool isAllUnderscores = username.All(c => c == '_'); //if all underscores, this line is necessary or else index OOR
            if (isAllUnderscores) {
                return "InvalidUsername";
            }
            bool underscore = false;
            if (username[username.Length - 1] == '_') {
                underscore = true;
            }
            while (underscore) {
                string tempo = username.Substring(0, username.Length - 1);
                username = tempo;
                if (username[username.Length - 1] == '_') {
                    continue;
                } else {
                    underscore = false;
                }
            }
            if (username[0] == '_') {
                underscore = true;
            }
            while (underscore) {
                string tempo = username.Substring(1, username.Length - 1);
                username = tempo;
                if (username[0] == '_') {
                    continue;
                } else {
                    underscore = false;
                }
            }
            return username;
        }


        /* PRIVATE METHODS */

        private static bool SetBalanceAll(string amount, string balanceType, string caller, byte purpose) {
            //RETRIEVE FILES
            if(!Int32.TryParse(amount,out int ignore)) {
                ReportBug("all", "Not a valid amount " + amount);
                return false;
            } else if (Int32.Parse(amount) < 0) {
                ReportBug("all", "Not a valid amount " + amount);
                return false;
            }
            DirectoryInfo direct = new DirectoryInfo(userSavesPath);
            FileInfo[] filesInDir = direct.GetFiles();
            if (filesInDir.Length == 0) {
                Console.WriteLine("Attempted to set balance for all, files not found.");
                return false;
            }
            List<string> fileLinksInDir = new List<string>();

            foreach (FileInfo file in filesInDir) {
                fileLinksInDir.Add(userSavesPath + file.ToString());
            }

            List<string> tempo = new List<string>();
            bool uno = false;
            //REMOVE UNNECESSARY FILES IF IT'S AN UNOPOINT COMMAND
            if (balanceType == "Unopoints") {
                uno = true;
                foreach (string file in fileLinksInDir) {
                    string score = File.ReadAllLines(file).FirstOrDefault(l => l.Contains("Unopoints:"));
                    //-1 suggests that a player is not in the Uno server, and should not be counted
                    if (!score.Contains("Unopoints:-1")) {
                        tempo.Add(file);
                    }
                }
            }
            if (uno) {
                fileLinksInDir = tempo;
                UtilUno.SaveLeaderboard(); //before replacing let's save all residual information in a save file
            }
            String replacedLine = balanceType + ":" + amount;

            for (int i = 0; i < fileLinksInDir.Count(); i++) {
                string balanceLine = "";
                string temp;
                string targetFile;
                short lineToEdit = 1;
                targetFile = Path.Combine(userSavesPath, fileLinksInDir[i]);
                using (StreamReader str = new StreamReader(targetFile)) {
                    while (!str.EndOfStream) {
                        temp = str.ReadLine();
                        if (temp.Contains(balanceType)) {
                            balanceLine = temp;
                            break;
                        }
                        lineToEdit++;
                    }
                }
                if (balanceLine == "") {
                    ReportBug(fileLinksInDir[i], "Critical Error: User does not have a " + balanceType + " line in their save file."); return false;
                }
                string balanceAmount = balanceLine.Replace(balanceType + ":", "");
                if (!Int32.TryParse(balanceAmount, out int ignore2)) {
                    ReportBug(fileLinksInDir[i], "Not a valid balance: " + balanceAmount + ".");
                    continue;
                }
                string[] lines = File.ReadAllLines(targetFile);

                using (StreamWriter writer = new StreamWriter(targetFile)) {
                    for (int curr = 1; curr <= lines.Length; curr++) {
                        if (curr == lineToEdit) {
                            writer.WriteLine(replacedLine);
                        } else {
                            writer.WriteLine(lines[curr - 1]);
                        }
                    }
                }
                ReportScoreChange(caller, "everyone", balanceAmount, amount, purpose);
            }
            return true;
        }

            private static bool EligibleToReceiveDaily() {
            throw new NotImplementedException();
        }
    }
}