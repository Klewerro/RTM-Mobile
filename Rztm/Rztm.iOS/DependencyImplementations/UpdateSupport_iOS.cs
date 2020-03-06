using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Rztm.DependencyInterfaces;
using Rztm.iOS.DependencyImplementations;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(UpdateSupport_iOS))]
namespace Rztm.iOS.DependencyImplementations
{
    public class UpdateSupport_iOS : IUpdateSupport
    {
        public void ApkInstall()
        {
            throw new NotImplementedException();
        }

        public bool CheckIfApkIsDownloaded()
        {
            throw new NotImplementedException();
        }

        public void RemoveApkFile()
        {
            throw new NotImplementedException();
        }
    }
}