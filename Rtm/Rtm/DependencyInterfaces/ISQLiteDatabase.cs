using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rtm.DependencyInterfaces
{
    public interface ISQLiteDatabase
    {
        void CreateDatabaseIfNotExist();
        string GetDatabaseConnectionString();
        SQLiteConnection GetConnection();
    }
}
