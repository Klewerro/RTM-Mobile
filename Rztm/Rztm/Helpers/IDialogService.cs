using Prism.Services;
using System;

namespace Rztm.Helpers
{
    public interface IDialogService : IPageDialogService
    {
        void DisplayToast(string message, ToastTime toastTime);
        IDisposable DisplayToast(string message, ToastTime toastTime, string actionText, Action action);
    }
}
