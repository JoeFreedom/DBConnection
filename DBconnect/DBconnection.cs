using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DBconnect
{
    public delegate void Message(string message);
    public class DBconnection
    {
        public event Message success;
        public event Message error;
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
        }

        public DBconnection(string host, string database, string port, string username, string pass)
        {
            ConnString = "Server=" + host + ";Database=" + database + ";port=" + port + ";User Id=" + username + ";password=" + pass;
        }

        public void ConnectDB() 
        {
            if (connection != null && connection.Ping())
            {
                error?.Invoke("Подключение уже осуществлено");
                return;
            }
            connection = new MySqlConnection(ConnString);
            connection.Open();
            if (connection.Ping())
            {
                success?.Invoke("Успешное подключение к БД");
            }
            else
            {
                error?.Invoke("Нет подключения к БД");
            }
        }

        public async Task ConnectDBAsync()
        {
            if (connection != null && connection.Ping())
            {
                error?.Invoke("[From async method] Подключение уже осуществлено");
                return;
            }
            connection = new MySqlConnection(ConnString);
            await connection.OpenAsync();
            if (connection.Ping())
            {
                success?.Invoke("[From async method] Успешное подключение к БД");
            }
            else
            {
                error?.Invoke("[From async method] Нет подключения к БД");
            }
        }

        public MySqlDataReader SelectQuery(string sql)
        {
            if (connection.Ping())
            {
                var command = new MySqlCommand { Connection = connection, CommandText = sql };
                var result = command.ExecuteReader();
                if (result != null)
                {
                    success?.Invoke("Запрос выполнен");
                    return result;
                }
                else
                {
                    error?.Invoke("Запрос в БД выполнить не удалось");
                    return result;
                }
            }
            else
            {
                error?.Invoke("Нет подключения к БД");
                return null;
            }
        }

        public async Task<DbDataReader> SelectQueryAsync(string sql)
        {
            if (connection.Ping())
            {
                var command = new MySqlCommand { Connection = connection, CommandText = sql };
                var result = await command.ExecuteReaderAsync();
                if (result != null)
                {
                    success?.Invoke("[From async method] Запрос выполнен");
                    return result;
                }
                else
                {
                    error?.Invoke("[From async method] Запрос в БД выполнить не удалось");
                    return result;
                }
            }
            else
            {
                error?.Invoke("[From async method] Нет подключения к БД");
                return null;
            }
        }

        public int InsertQuery(string sql)
        {
            if (connection.Ping()) 
            {
                var command = new MySqlCommand { Connection = connection, CommandText = sql };
                var result = command.ExecuteNonQuery();
                if(result > 0)
                {
                    success?.Invoke("Запись успешно добавлена в БД");
                }
                else
                {
                    error?.Invoke("Не удалось внести запись в БД");
                }
                return result;
            }
            else
            {
                error?.Invoke("Нет подключения к БД");
                return -1;
            }     
        }

        public async Task<int> InsertQueryAsync(string sql)
        {
            if (connection.Ping())
            {
                var command = new MySqlCommand { Connection = connection, CommandText = sql };
                var result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    success?.Invoke("[From async method] Запись успешно добавлена в БД");
                }
                else
                {
                    error?.Invoke("[From async method] Не удалось внести запись в БД");
                }
                return result;
            }
            else
            {
                error?.Invoke("[From async method] Нет подключения к БД");
                return -1;
            }
        }

        public int UpdateQuery(string sql)
        {
            if (connection.Ping())
            {
                var command = new MySqlCommand { Connection = connection, CommandText = sql };
                var result = command.ExecuteNonQuery();
                if(result > 0)
                {
                    success?.Invoke("Изменения в БД внесены");
                }
                else
                {
                    error?.Invoke("Не удалось внести изменения в БД");
                }
                return result;
            }
            else
            {
                error?.Invoke("Нет подключения к БД");
                return -1;
            }
        }

        public async Task<int> UpdateQueryAsync(string sql)
        {
            if (connection.Ping())
            {
                var command = new MySqlCommand { Connection = connection, CommandText = sql };
                var result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    success?.Invoke("[From async method] Изменения в БД внесены");
                }
                else
                {
                    error?.Invoke("[From async method] Не удалось внести изменения в БД");
                }
                return result;
            }
            else
            {
                error?.Invoke("[From async method] Нет подключения к БД");
                return -1;
            }
        }

        public bool IsConnect()
        {
            return connection.Ping();
        }

        public void Close()
        {
            if (connection.Ping())
            {
                connection.Close();
                success?.Invoke("Соединение закрыто");
            } else
            {
                error?.Invoke("Соединенине с БД либо уже закрыто, либо его не было");
            }
        }
        public async void CloseAsync()
        {
            if (connection.Ping())
            {
                await connection.CloseAsync();
                success?.Invoke("[From async method] Соединение закрыто");
            } else
            {
                error?.Invoke("[From async method] Соединенине с БД либо уже закрыто, либо его не было");
            }
        }
    }
}
