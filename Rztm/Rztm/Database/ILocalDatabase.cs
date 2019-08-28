using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.Database
{
    public interface ILocalDatabase
    {
        SQLiteConnection GetConnection();
    }
}
