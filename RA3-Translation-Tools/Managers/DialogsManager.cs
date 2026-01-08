using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Huskui.Avalonia.Controls;
using RA3_Translation_Tools.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RA3_Translation_Tools.Managers
{
    public static class DialogsManager
    {
        private const string strExtension = "*.str";
        private const string NullTitleMsg = "Title cannot be null or empty";
        private static TopLevel? _topLevel;
        private static AppWindow? _appWindow;

        // Статический метод для инициализации
        public static void Initialize(TopLevel topLevel, AppWindow appWindow)
        {
            _topLevel = topLevel;
            _appWindow = appWindow;
        }

        #region Методы диалога сообщений

        public static async Task<bool?> ShowMsgDialogAsync(
            string message,
            string title,
            bool showButtons = false,
            string primaryButtonText = "",
            string secondaryButtonText = "")
        {
            if (_appWindow == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or empty", nameof(message));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(NullTitleMsg, nameof(title));
            }

            YesNoDialog dialog = new()
            {
                Title = title,
                Content = message,
                PrimaryText = showButtons ? primaryButtonText : "Да",
                SecondaryText = secondaryButtonText ?? "Отмена",
                IsPrimaryButtonVisible = showButtons
            };

            _appWindow.PopDialog(dialog);
            return await dialog.CompletionSource.Task;
        }

        #endregion

        #region Методы диалога сохранения файлов

        public static async Task<IStorageFile?> SaveFileDialogAsync(
            string title = "Сохранить файл",
            string suggestedFileName = "безымянный.txt",
            IEnumerable<string>? allowedExtensions = null)
        {
            if (_topLevel == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(NullTitleMsg, nameof(title));
            }

            FilePickerSaveOptions options = new()
            {
                Title = title,
                SuggestedFileName = suggestedFileName ?? "безымянный.txt",
                ShowOverwritePrompt = true
            };

            if (allowedExtensions?.Any() == true)
            {
                options.FileTypeChoices =
                [
                    new FilePickerFileType("Файлы")
            {
                Patterns = [.. allowedExtensions]
            }
                ];
            }

            return await _topLevel.StorageProvider.SaveFilePickerAsync(options);
        }

        #endregion

        #region Методы диалога файлов

        public static async Task<IStorageFolder?> OpenModStrDirectoryDialogAsync()
        {
            return await OpenDirectoryDialogAsync("Выберите папку с файлами mod.str");
        }

        public static async Task<IStorageFile?> OpenSingleFileDialogAsync(string title, IEnumerable<string>? allowedExtensions = null)
        {
            if (_topLevel == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(NullTitleMsg, nameof(title));
            }

            FilePickerOpenOptions options = CreateFilePickerOptions(title, allowedExtensions, false);
            IReadOnlyList<IStorageFile> files = await _topLevel.StorageProvider.OpenFilePickerAsync(options);

            return files.Count > 0 ? files[0] : null;
        }

        public static async Task<IReadOnlyList<IStorageFile>?> OpenMultipleFilesDialogAsync(string title, IEnumerable<string>? allowedExtensions = null)
        {
            if (_topLevel == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(NullTitleMsg, nameof(title));
            }

            FilePickerOpenOptions options = CreateFilePickerOptions(title, allowedExtensions, true);
            IReadOnlyList<IStorageFile> files = await _topLevel.StorageProvider.OpenFilePickerAsync(options);

            return files.Count > 0 ? files : null;
        }

        #endregion

        #region Удобные методы диалога файлов

        public static async Task<IStorageFolder?> OpenDirectoryDialogAsync(string title = "Выберите папку", bool allowMultiple = false)
        {
            if (_topLevel == null)
            {
                return null;
            }

            IReadOnlyList<IStorageFolder> folder = await _topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = title,
                AllowMultiple = allowMultiple
            });

            return folder?.Count > 0 ? folder[0] : null;
        }

        public static async Task<IStorageFile?> OpenTextFileDialogAsync(string title = "Выберите текстовый файл")
        {
            string[] extensions = ["*.txt", strExtension, "*.csv"];
            return await OpenSingleFileDialogAsync(title, extensions);
        }

        public static async Task<IStorageFile?> OpenModStrFileDialogAsync(string title = "Выберите файл модификации")
        {
            string[] extensions = [strExtension];
            return await OpenSingleFileDialogAsync(title, extensions);
        }

        public static async Task<IReadOnlyCollection<IStorageFile>?> OpenMultipleModStrFilesDialogAsync(string title = "Выберите файл модификации")
        {
            string[] extensions = [strExtension];
            return await OpenMultipleFilesDialogAsync(title, extensions);
        }

        public static async Task<IStorageFile?> OpenAllFilesDialogAsync(string title = "Выберите файл")
        {
            return await OpenSingleFileDialogAsync(title);
        }

        public static async Task<IReadOnlyList<IStorageFile>?> OpenMultipleTextFilesDialogAsync(string title = "Выберите текстовые файлы")
        {
            string[] extensions = ["*.txt", strExtension, "*.csv"];
            return await OpenMultipleFilesDialogAsync(title, extensions);
        }

        #endregion

        #region Приватные помощники

        private static FilePickerOpenOptions CreateFilePickerOptions(
            string title,
            IEnumerable<string>? allowedExtensions,
            bool allowMultiple)
        {
            FilePickerOpenOptions options = new()
            {
                Title = title,
                AllowMultiple = allowMultiple
            };

            if (allowedExtensions?.Any() == true)
            {
                options.FileTypeFilter =
                [
                    new FilePickerFileType(title)
                    {
                        Patterns = [.. allowedExtensions]
                    }
                ];
            }

            return options;
        }

        #endregion
    }
}