using System;
using System.Collections.Generic;

namespace Obracore.Client.Services
{
    public class ToastService
    {
        public event Action<ToastMessage>? OnShow;
        public event Action? OnClear;

        public void ShowSuccess(string message, string? heading = null) => Notify(message, "success", heading);
        public void ShowError(string message, string? heading = null) => Notify(message, "danger", heading);
        public void ShowInfo(string message, string? heading = null) => Notify(message, "info", heading);

        private void Notify(string message, string level, string? heading)
        {
            OnShow?.Invoke(new ToastMessage { Message = message, Level = level, Heading = heading });
        }

        public void Clear() => OnClear?.Invoke();
    }

    public class ToastMessage
    {
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = "info"; // bootstrap level: success, danger, info, warning
        public string? Heading { get; set; }
    }
}
