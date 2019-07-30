using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Rtm.DependencyInterfaces;
using Rtm.Droid.DependencyImplementations;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDatabase_Droid))]
namespace Rtm.Droid.DependencyImplementations
{
    public class SQLiteDatabase_Droid : ISQLiteDatabase
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