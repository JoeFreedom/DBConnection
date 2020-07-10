using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace DBconnect
{
    public class DBconnection
    {
        private MySqlConnection connection;

        private readonly string host;
        private readonly string database;
        private readonly string port;
        private readonly string username;
        private readonly string pass;
        private readonly string ConnString;

        public DBconnection()
        {
            using (var file = new StreamReader("dbconnect.cfg"))
            {
                string tempLine;
                while ((tempLine = file.ReadLine()) != null)
                {
                    tempLine = tempLine.Trim();
                    var index = tempLine.IndexOf('=');
                    if (index < 0)
                        continue;
                    var tempSymbols = tempLine.Substring(index + 1);
                    var tempVar = tempLine.Substring(0, index);
                    tempSymbols = tempSymbols.Trim();
                    tempVar = tempVar.Trim();

                    switch (tempVar)
                    {
                        case "host":
                            host = tempSymbols;
                            break;
                        case "database":
                            database = tempSymbols;
                            break;
                        case "port":
                            port = tempSymbols;
                            break;
                        case "username":
                            username = tempSymbols;
                            break;
                        case "pass":
                            pass = tempSymbols;
                            break;
                    }
                }
            }
            ConnString = "Server=" + host + ";Database=" + database + ";port=" + port + ";User Id=" + username + ";password=" + pass;

            connection = new MySqlConnection(ConnString);
            connection.Open();
        }
    }
}
