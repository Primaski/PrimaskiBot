//SERVES AS UTILBALANCE BACKUP FOR NOW
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Primaski_Bot.Modules {
    public static class UtilBalance {
        static string filepath = "../../balance.txt";
        static string ballog = "../../ballog.txt";
        static string buglog = "../../buglog.txt";
        static int baseIncome = 100;

        internal static bool EligibleToReceiveDailyById(string userID = null) {
            if(userID == null) {
                return false;
            }

            return false;
        }

        //returns balance, -1 if error, -2 if user not found
        public static string GetBalanceById(string userID) {
            foreach (string line in File.ReadLines(filepath)) {
                if (line.Contains(userID)) {
                    int temp = line.IndexOf("bal:");
                    if (temp == -1) {
                        StreamWriter swc = File.AppendText(buglog);
                        swc.WriteLine(DateTime.Now.Day + "\t" + userID + "\t" + "Critical Error in finding balance");
                        swc.Close();
                        return "-1"; //error
                    }
                    string linew = line.Substring(temp); //cuts line to the point where bal starts
                    temp = linew.IndexOf('\t');
                    if (temp == -1) {
                        StreamWriter swc = File.AppendText(buglog);
                        swc.WriteLine(DateTime.Now.Day + "\t" + userID + "\t" + "Critical Error in finding balance, no tab followed");
                        swc.Close();
                        return "-1"; //error
                    }
                    string linex = linew.Substring(4, temp - 4); //cuts off everything after the specified balance and before the bal int
                    return linex;
                }
            }
            return "-2"; //create new user
        }

        internal static void CreateAccount(ulong id, string username) {
            StreamWriter sw = File.AppendText(filepath);
            sw.WriteLine();
            sw.WriteLine(id + "\tuser:" + username + "\tbal:" + baseIncome + "\tdaily:" + DateTime.Now.Day);
            sw.Close();
            StreamWriter sx = File.AppendText(ballog);
            sx.WriteLine();
            sx.WriteLine(DateTime.Now.Date + DateTime.Now.TimeOfDay + ":" + "Account created. Username: " + username + " ID: " + id);
            sx.Close();
            return;
        }

        public static string GetBalanceByUsername(string user) {
            string linex = "";
            foreach (string line in File.ReadLines(filepath)) {
                if (line.Contains(user)) {
                    int temp = line.IndexOf("bal:");
                    if (temp == -1) {
                        StreamWriter swc = File.AppendText(buglog);
                        swc.WriteLine(DateTime.Now.Day + "\t" + user + "\t" + "Critical Error in finding balance");
                        swc.Close();
                        return "-1"; //error
                    }
                    string linew = line.Substring(temp);
                    temp = linew.IndexOf('\t');
                    if (temp == -1) {
                        StreamWriter swc = File.AppendText(buglog);
                        swc.WriteLine(DateTime.Now.Day + "\t" + user + "\t" + "Critical Error in finding balance, no tab followed");
                        swc.Close();
                        return "-1"; //error
                    }
                    linex = linew.Substring(4, temp - 4); //cuts off everything after the specified balance and before the bal int
                    return linex;
                }
            }
            return "-2";
        }

        //ASSUMES ALL USERNAMES ARE DISTINCT, CONFLICT ARISES IF TWO USERS HAVE THE SAME NAME. NOT A CONCERN AS OF NOW THOUGH, AS GAME IS SMALL SCALE
        public static int SetBalanceByUsername(string user, int amount, string purpose = null) {
            if(purpose == null) {
                return -1;
            }
            string linex = "";
            string linenew = "";
            int lineLocation = 0;
            if(amount < 0) {
                return -1;
            }
            foreach (string line in File.ReadLines(filepath)) {
                if (line.Contains(user)) {
                    int temp = line.IndexOf("bal:");
                    if (temp == -1) {
                        StreamWriter swc = File.AppendText(buglog);
                        swc.WriteLine(DateTime.Now.Day + "\t" + user + "\t" + "Critical Error in finding balance");
                        swc.Close();
                        return -1; //error
                    }
                    string linew = line.Substring(temp);
                    temp = linew.IndexOf('\t');
                    if (temp == -1) {
                        StreamWriter swc = File.AppendText(buglog);
                        swc.WriteLine(DateTime.Now.Day + "\t" + user + "\t" + "Critical Error in finding balance, no tab followed");
                        swc.Close();
                        return -1; //error
                    }
                    linex = linew.Substring(4, temp - 4); //cuts off everything after the specified balance and before the bal int
                    linenew = line.Replace(linex, amount.ToString());
                    break;
                }
                lineLocation++;
            }
            if(linex == "") { //not found
                return -1;
            }
            string[] lines = File.ReadAllLines(filepath);
            using (StreamWriter writer = new StreamWriter(filepath)) {
                for(int currLine = 0; currLine < lines.Length; currLine++) {
                    if(currLine == lineLocation) { //line to replace
                        writer.WriteLine(linenew);
                        Console.WriteLine(linenew);

                    } else {
                        writer.WriteLine(lines[currLine]);
                    }
                }
                writer.Close();
            }
            StreamWriter ballogwriter = File.AppendText(ballog);
            ballogwriter.WriteLine();
            ballogwriter.WriteLine(DateTime.Now.Date + DateTime.Now.TimeOfDay + ":" + "SETBAL " + user + " "
                + amount + " Purpose: " + purpose);
            ballogwriter.Close();

                return amount;
        }

        internal static int AddBalanceByUsername(string user, int amount, string purpose = null) {

            string linex = "";
            string linenew = "";
            int lineLocation = 0;
            int oldVal, newVal;
            oldVal = newVal = -1;
            foreach (string line in File.ReadLines(filepath)) {
                if (line.Contains(user)) {
                    int temp = line.IndexOf("bal:");
                    if (temp == -1) {
                        StreamWriter swc = File.AppendText(buglog);
                        swc.WriteLine(DateTime.Now.Day + "\t" + user + "\t" + "Critical Error in finding balance");
                        swc.Close();
                        return -1; //error
                    }
                    string linew = line.Substring(temp);
                    temp = linew.IndexOf('\t');
                    if (temp == -1) {
                        StreamWriter swc = File.AppendText(buglog);
                        swc.WriteLine(DateTime.Now.Day + "\t" + user + "\t" + "Critical Error in finding balance, no tab followed");
                        swc.Close();
                        return -1; //error
                    }
                    linex = linew.Substring(4, temp - 4); //cuts off everything after the specified balance and before the bal int
                    if(!Int32.TryParse(linex,out int test)){
                        return -1;
                    }
                    oldVal = Int32.Parse(linex);
                    newVal = oldVal + amount;
                    if(newVal < 0 || newVal > int.MaxValue) {
                        return -1;
                    }
                    linenew = line.Replace(linex, newVal.ToString());
                    break;
                }
                lineLocation++;
            }
            if (linex == "") { //not found
                return -1;
            }
            string[] lines = File.ReadAllLines(filepath);
            using (StreamWriter writer = new StreamWriter(filepath)) {
                for (int currLine = 0; currLine < lines.Length; currLine++) {
                    if (currLine == lineLocation) { //line to replace
                        writer.WriteLine(linenew);
                        Console.WriteLine(linenew);

                    } else {
                        writer.WriteLine(lines[currLine]);
                    }
                }
                writer.Close();
            }
            StreamWriter ballogwriter = File.AppendText(ballog);
            ballogwriter.WriteLine();
            ballogwriter.WriteLine(DateTime.Now.Date + DateTime.Now.TimeOfDay + ":" + "ADDBAL " + user + " "
                + amount + " Purpose: " + purpose);
            ballogwriter.Close();
            return newVal;
        }
    }
}
*/