using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Data;

namespace Android_KingHoo_Scanner_Rebuild
{
   public class Tools_SQLite_Class
    {
        static string m_filePath = ""; 
        public Tools_SQLite_Class()
        {
            var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            m_filePath = Path.Combine(documents, "sqliteSample.db");
            File.Create(m_filePath);
        }
        public void InitializeDatabase()
        {

            //var directories = Directory.EnumerateDirectories("./");
            //foreach (var directory in directories)
            //{
            //    Console.WriteLine(directory);
            //}
            //Environment.DataDirectory

            //File.WriteAllText(filename, "Write this text into a file");
            
            //await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists);
            //string dbpath = "sqliteSample.db";
            using (SqliteConnection db =
               new SqliteConnection($"Filename={m_filePath}"))
            {
                try
                {
                    db.Open();

                    String tableCommand = "CREATE TABLE IF NOT " +
                        "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
                        "Text_Entry NVARCHAR(2048) NULL)";

                    SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                    createTable.ExecuteReader();
                }catch(SqliteException sex)
                {
                    Console.WriteLine(sex.Message);
                }
                
            }
        }
        public void AddData(string inputText)
        {
            //string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={m_filePath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                //insertCommand.

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", inputText);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }

        public List<String> GetData()
        {
            List<String> entries = new List<string>();

            //string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={m_filePath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT Text_Entry from MyTable", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(query);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Console.WriteLine(dt.Rows[i]["Text_Entry"].ToString());
                }
                
                //while (query.Read())
                //{
                //    entries.Add(query.GetString(0));
                //}

                db.Close();
            }

            return entries;
        }

        //
    }
}