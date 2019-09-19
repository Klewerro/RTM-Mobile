using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using Rztm.DependencyInterfaces;
using Rztm.iOS.DependencyImplementations;
using SQLite;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDatabase_iOS))]
namespace Rztm.iOS.DependencyImplementations
{
    public class SQLiteDatabase_iOS : ISQLiteDatabase
    {
        private string _path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "localdb.db3");

        public void CreateDatabaseIfNotExist()
        {
            var exists = Directory.Exists(_path);
            if (!exists)
                Directory.CreateDirectory(_path);
        }

        public string GetDatabaseConnectionString() => _path.Remove(_path.Length - 1);

        public SQLiteConnection GetConnection() => new SQLiteConnection(_path.Remove(_path.Length - 1));
    }
}