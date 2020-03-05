using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.DependencyInterfaces
{
    public interface IUpdateSupport
    {
        bool CheckIfApkIsDownloaded();
        void ApkInstall();
    }
}
