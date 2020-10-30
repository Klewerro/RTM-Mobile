using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.DependencyInterfaces
{
    public interface ISQLiteDatabase
    {
        void CreateDatabaseIfNotExist();
        string GetDatabaseConnectionString();
        SQLiteAsyncConnection GetConnection();
    }
}
