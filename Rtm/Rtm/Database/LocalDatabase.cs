using System;
using System.Collections.Generic;
using System.Text;
using Rtm.DependencyInterfaces;
using Rtm.Models;
using SQLite;
using Xamarin.Forms;

namespace Rtm.Database
{
    public class LocalDatabase : ILocalDatabase
    {
        private readonly SQLiteConnection _connection;
        private readonly ISQLiteDatabase _database = DependencyService.Get<ISQLiteDatabase>();


        public LocalDatabase()
        {
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
