using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rtm.Database
{
    public interface ILocalDatabase
    {
        SQLiteConnection GetConnection();
    }
}
