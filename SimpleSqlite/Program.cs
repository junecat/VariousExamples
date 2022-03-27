using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace SimpleSql
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> taleList = new List<string>();
            using (var connection = new SqliteConnection(@"Data Source=hello.sqlite")) {
                SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
                connection.Open();
                const string sql_query = "SELECT name FROM sqlite_schema WHERE type ='table' AND name NOT LIKE 'sqlite_%';";
                var command = new SqliteCommand(sql_query, connection);

                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var name = reader.GetString(0);
                        taleList.Add(name);
                        Console.WriteLine($"{name}");
                    }
                }
            }
        }
    }
}
