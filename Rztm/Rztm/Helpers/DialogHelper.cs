using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.Helpers
{
    public static class DialogHelper
    {
        public static void DisplayToast(string message, ToastTime toastTime)
        {
            var config = new ToastConfig(message)
            {
                Duration = TimeSpan.FromSeconds((double)toastTime)
            };

            UserDialogs.Instance.Toast(config);
            
        }

        public static IDisposable DisplayToast(string message, ToastTime toastTime, string actionText, Action action)
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
