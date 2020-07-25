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

        public bool isConnect;

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
            ConnectDB();
        }

        public DBconnection(string host, string database, string port, string username, string pass)
        {
            ConnString = "Server=" + host + ";Database=" + database + ";port=" + port + ";User Id=" + username + ";password=" + pass;
            ConnectDB();
        }

        public void ConnectDB() 
        {
            connection = new MySqlConnection(ConnString);
            connection.Open();
            if(connection.Ping())
            {
                isConnect = true;
            } else
            {
                isConnect = false;
            }
        }

        public MySqlDataReader SelectQuery(string sql)
        {
            var command = new MySqlCommand { Connection = connection, CommandText = sql };
            var result = command.ExecuteReader();
            return result;
        }

        public int InsertQuery(string sql)
        {
            var command = new MySqlCommand { Connection = connection, CommandText = sql };
            var result = command.ExecuteNonQuery();
            return result;
        }

        public int UpdateQuery(string sql)
        {
            var command = new MySqlCommand { Connection = connection, CommandText = sql };
            var result = command.ExecuteNonQuery();
            return result;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
