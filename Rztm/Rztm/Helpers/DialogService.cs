using Acr.UserDialogs;
using Prism.Common;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rztm.Helpers
{
    public class DialogService : PageDialogService, IDialogService
    {
        public DialogService(IApplicationProvider applicationProvider) : base(applicationProvider)
        {
        }

        public void DisplayToast(string message, ToastTime toastTime)
        {
            var config = new ToastConfig(message)
            {
                Duration = TimeSpan.FromSeconds((double)toastTime)
            };

            UserDialogs.Instance.Toast(config);

        }

        public IDisposable DisplayToast(string message, ToastTime toastTime, string actionText, Action action)
        {
            var config = new ToastConfig(message)
            {
                Duration = TimeSpan.FromSeconds((double)toastTime),
                Action = new ToastAction
                {
                    Text = actionText,
                    TextColor = System.Drawing.Color.DodgerBlue,
                    Action = action
                }
            };

            return UserDialogs.Instance.Toast(config);
        }
    }


    public enum ToastTime : int
    {
        Short = 3,
        Long = 8,
        VeryLong = 15,
        OneHour = 3600
    }
}
