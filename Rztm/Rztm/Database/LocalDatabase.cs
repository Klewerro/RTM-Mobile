using System;
using System.Collections.Generic;
using System.Text;
using Rztm.DependencyInterfaces;
using Rztm.Models;
using SQLite;
using Xamarin.Forms;

namespace Rztm.Database
{
    public class LocalDatabase : ILocalDatabase
    {
        private readonly SQLiteConnection _connection;
        private readonly ISQLiteDatabase _database;


        public LocalDatabase(ISQLiteDatabase database)
        {
            _database = database;

            _database.CreateDatabaseIfNotExist();
            _connection = _database.GetConnection();
            PrepareDatabaseTables();
        }

        public SQLiteConnection GetConnection()
            => _connection;

        
        private void PrepareDatabaseTables()
        {
            _connection.CreateTable<BusStop>();
        }
    }
}
