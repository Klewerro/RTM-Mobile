using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rztm.Database
{
    public interface ILocalDatabase
    {
        SQLiteAsyncConnection GetConnection();
        Task PrepareDatabaseTablesAsync();
    }
}
