using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using Xamarin.Forms;

namespace mobilka.Models
{
    public class DatabaseService
    {
        SQLiteConnection database;

        public DatabaseService()
        {
            
            string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "magazine.db");

            
            database = new SQLiteConnection(databasePath);

           
            database.CreateTable<Users>();
            database.CreateTable<Tovar>();
           
            database.CreateTable<Orders>();
            
        }

        
        public SQLiteConnection GetConnection()
        {
            return database;
        }
    }
}
