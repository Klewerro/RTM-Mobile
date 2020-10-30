using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rztm.DependencyInterfaces;
using Rztm.Models;
using SQLite;
using Xamarin.Forms;

namespace Rztm.Database
{
    public class LocalDatabase : ILocalDatabase
    {
        private readonly SQLiteAsyncConnection _connection;
        private readonly ISQLiteDatabase _database;


        public LocalDatabase(ISQLiteDatabase database)
        {
            _database = database;

            _database.CreateDatabaseIfNotExist();
            _connection = _database.GetConnection();
        }

        public SQLiteAsyncConnection GetConnection()
            => _connection;

        
        public async Task PrepareDatabaseTablesAsync()
        {
            await _connection.CreateTableAsync<BusStop>();
        }
    }
}
