// Managers/GrowlsManager.cs
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Models;
using System;
using System.Diagnostics;

namespace RA3_Translation_Tools.Managers
{
    public static class GrowlsManager
    {
        private static AppWindow? _appWindow;

        // Статический метод для инициализации
        public static void Initialize(AppWindow appWindow)
        {
            _appWindow = appWindow ?? throw new ArgumentNullException(nameof(appWindow));
        }

        #region Метода показа сообщения
        public static GrowlItem? ShowInfoMsg(string msg, string title = "")
        {
            return ShowGrowlMsg(GrowlLevel.Information, GetDefaultTitle(GrowlLevel.Information, title), msg);
        }

        public static GrowlItem? ShowProgressInfoMsg(string msg, string title = "", double progress = 0)
        {
            return ShowGrowlMsg(GrowlLevel.Information, GetDefaultTitle(GrowlLevel.Information, title), msg, true, progress);
        }

        public static GrowlItem? ShowSuccesMsg(string msg, string title = "")
        {
            return ShowGrowlMsg(GrowlLevel.Success, GetDefaultTitle(GrowlLevel.Success, title), msg);
        }

        public static GrowlItem? ShowWarningMsg(string warnMsg)
        {
            return ShowGrowlMsg(GrowlLevel.Warning, GetDefaultTitle(GrowlLevel.Warning), warnMsg);
        }

        public static GrowlItem? ShowErrorMsg(Exception ex, string title = "")
        {
            string errorMessage = ex.Message + (ex.InnerException?.Message ?? "");
            return ShowGrowlMsg(GrowlLevel.Danger, GetDefaultTitle(GrowlLevel.Danger, title), errorMessage);
        }

        public static GrowlItem? ShowErrorMsg(string errMsg, string title = "")
        {
            return ShowGrowlMsg(GrowlLevel.Danger, GetDefaultTitle(GrowlLevel.Danger, title), errMsg);
        }
        #endregion

        public static GrowlItem? ShowGrowlMsg(
            GrowlLevel growlLevel,
            string title,
            string content,
            bool isProgressVisible = false,
            double progress = 0)
        {
            if (_appWindow == null)
            {
                return null;
            }

            GrowlItem growlItem = new()
            {
                Level = growlLevel,
                Title = GetDefaultTitle(growlLevel, title),
                Content = content,
                Progress = isProgressVisible ? progress : 0,
                IsProgressBarVisible = isProgressVisible
            };

            Debug.WriteLine($"{title}: {content}");

            _appWindow.PopGrowl(growlItem);
            return growlItem;
        }

        #region Приватные помощники
        private static string GetDefaultTitle(GrowlLevel level, string customTitle = "")
        {
            return !string.IsNullOrWhiteSpace(customTitle)
                ? customTitle
                : level switch
                {
                    GrowlLevel.Information => "Информация",
                    GrowlLevel.Success => "Успех",
                    GrowlLevel.Warning => "Предупреждение",
                    GrowlLevel.Danger => "Ошибка",
                    _ => "Сообщение"
                };
        }
        #endregion
    }
}