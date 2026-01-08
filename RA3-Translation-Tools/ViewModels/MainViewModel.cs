using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RA3_Translation_Tools.Core.Helpers;
using RA3_Translation_Tools.Core.Interfaces;
using RA3_Translation_Tools.Core.Models.Enums;
using RA3_Translation_Tools.Managers;
using SadPencil.Ra2CsfFile;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RA3_Translation_Tools.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly IFileService _fileService;
    private readonly ICsfFileConverter _csfFileConverter;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _disposed = false;

    public MainViewModel(
        IFileService fileService,
        ICsfFileConverter csfFileConverter)
    {
        _fileService = fileService;
        _csfFileConverter = csfFileConverter;
    }

    // === .big файлы ===
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanUnpackBig))]
    private string _inputBigFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanPackBig))]
    private string _outputBigFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanPackBig))]
    private string _packedDirectory = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanUnpackBig))]
    private string _unpackDirectory = string.Empty;

    // === Конвертация ===
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertStrToCsf))]
    private string _inputStrFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertStrToCsf))]
    private string _outputCsfFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertCsfToStr))]
    private string _inputCsfFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertCsfToStr))]
    private string _outputStrFilePath = string.Empty;

    // --- Новые свойства для CSF <-> JSON ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertCsfToJson))]
    private string _inputCsfToJsonFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertCsfToJson))]
    private string _outputJsonFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertJsonToCsf))]
    private string _inputJsonToCsfFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertJsonToCsf))]
    private string _outputCsfFromJsonFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConvertJsonToCsf))]
    private SupportedLanguage _inputJsonToCsfLang = SupportedLanguage.Unknown;
    // --- Конец новых свойств ---
    public bool CanUnpackBig =>
        !string.IsNullOrWhiteSpace(InputBigFilePath) &&
        !string.IsNullOrWhiteSpace(UnpackDirectory) &&
        File.Exists(InputBigFilePath) &&
        Directory.Exists(UnpackDirectory);

    public bool CanPackBig =>
        !string.IsNullOrWhiteSpace(PackedDirectory) &&
        !string.IsNullOrWhiteSpace(OutputBigFilePath) &&
        Directory.Exists(PackedDirectory);

    public bool CanConvertStrToCsf =>
        !string.IsNullOrWhiteSpace(InputStrFilePath) &&
        !string.IsNullOrWhiteSpace(OutputCsfFilePath) &&
        File.Exists(InputStrFilePath);

    public bool CanConvertCsfToStr =>
        !string.IsNullOrWhiteSpace(InputCsfFilePath) &&
        !string.IsNullOrWhiteSpace(OutputStrFilePath) &&
        File.Exists(InputCsfFilePath);

    // --- Новые свойства Can для CSF <-> JSON ---
    public bool CanConvertCsfToJson =>
        !string.IsNullOrWhiteSpace(InputCsfToJsonFilePath) &&
        !string.IsNullOrWhiteSpace(OutputJsonFilePath) &&
        File.Exists(InputCsfToJsonFilePath);

    public bool CanConvertJsonToCsf =>
        !string.IsNullOrWhiteSpace(InputJsonToCsfFilePath) &&
        !string.IsNullOrWhiteSpace(OutputCsfFromJsonFilePath) &&
        File.Exists(InputJsonToCsfFilePath) &&
        InputJsonToCsfLang != SupportedLanguage.Unknown;
    // --- Конец новых свойств ---

    // === Команды выбора файлов/папок ===
    [RelayCommand]
    private async Task SelectInputBigFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync(
            "Выберите .big файл",
            ["*.big"]);

        if (path?.Path?.LocalPath is { } filePath)
        {
            InputBigFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectOutputBigFile()
    {
        string exampleFileName = string.IsNullOrWhiteSpace(PackedDirectory)
            ? "packed.big"
            : $"{Path.GetFileNameWithoutExtension(PackedDirectory)}.big";

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync(
            "Сохранить как .big файл",
            exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputBigFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectUnpackDirectory()
    {
        IStorageFolder? folder = await DialogsManager.OpenDirectoryDialogAsync("Выберите папку для распаковки .big файла");

        if (folder?.Path?.LocalPath is { } path)
        {
            UnpackDirectory = path.TrimEnd(Path.DirectorySeparatorChar);
        }
    }

    [RelayCommand]
    private async Task SelectPackedDirectory()
    {
        IStorageFolder? folder = await DialogsManager.OpenDirectoryDialogAsync("Выберите папку с файлами для упаковки");

        if (folder?.Path?.LocalPath is { } path)
        {
            PackedDirectory = path.TrimEnd(Path.DirectorySeparatorChar);
        }
    }

    [RelayCommand]
    private async Task SelectInputStrFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync("Выберите .str файл", ["*.str"]);

        if (path?.Path?.LocalPath is { } filePath)
        {
            InputStrFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectOutputCsfFile()
    {
        string exampleFileName = string.IsNullOrWhiteSpace(InputStrFilePath)
            ? "converted.csf"
            : $"{Path.GetFileNameWithoutExtension(InputStrFilePath)}.csf";

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync("Сохранить как .csf файл", exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputCsfFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectInputCsfFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync("Выберите .csf файл", ["*.csf"]);

        if (path?.Path?.LocalPath is { } filePath)
        {
            InputCsfFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectOutputStrFile()
    {
        string exampleFileName = string.IsNullOrWhiteSpace(InputCsfFilePath)
            ? "converted.str"
            : $"{Path.GetFileNameWithoutExtension(InputCsfFilePath)}.str";

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync("Сохранить как .str файл", exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputStrFilePath = filePath;
        }
    }

    // --- Новые команды выбора файлов для CSF <-> JSON ---
    [RelayCommand]
    private async Task SelectInputCsfToJsonFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync("Выберите .csf файл", ["*.csf"]);

        if (path?.Path?.LocalPath is { } filePath)
        {
            InputCsfToJsonFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectOutputJsonFile()
    {
        string exampleFileName = string.IsNullOrWhiteSpace(InputCsfToJsonFilePath)
            ? "converted.json"
            : $"{Path.GetFileNameWithoutExtension(InputCsfToJsonFilePath)}.json";

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync("Сохранить как .json файл", exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputJsonFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectInputJsonToCsfFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync("Выберите .json файл", ["*.json"]);

        if (path?.Path?.LocalPath is { } filePath)
        {
            InputJsonToCsfFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectOutputCsfFromJsonFile()
    {
        string exampleFileName = string.IsNullOrWhiteSpace(InputJsonToCsfFilePath)
            ? "converted.csf"
            : $"{Path.GetFileNameWithoutExtension(InputJsonToCsfFilePath)}.csf";

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync("Сохранить как .csf файл", exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputCsfFromJsonFilePath = filePath;
        }
    }
    // --- Конец новых команд ---

    // === Основные команды ===
    [RelayCommand]
    private void UnpackBigFile()
    {
        if (!CanUnpackBig)
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(UnpackDirectory);
            BigFileHelper.UnpackBig(InputBigFilePath, UnpackDirectory);
            GrowlsManager.ShowSuccesMsg($"Файл {Path.GetFileName(InputBigFilePath)} успешно распакован!");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Не удалось распаковать .big файл");
        }
    }

    [RelayCommand]
    private void PackBigFile()
    {
        if (!CanPackBig)
        {
            return;
        }

        try
        {
            string outputDir = Path.GetDirectoryName(OutputBigFilePath)!;
            Directory.CreateDirectory(outputDir);

            BigFileHelper.PackBig(PackedDirectory, OutputBigFilePath);
            GrowlsManager.ShowSuccesMsg($"Директория {Path.GetDirectoryName(PackedDirectory)} успешно запакована в файл {Path.GetFileName(OutputBigFilePath)}!");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Не удалось запаковать .big файл");
        }
    }

    [RelayCommand]
    private async Task ConvertStrToCsf()
    {
        if (!CanConvertStrToCsf)
        {
            return;
        }

        try
        {
            string inputText = await File.ReadAllTextAsync(InputStrFilePath);
            CsfFile? csf = _csfFileConverter.ConvertStrToCsf(inputText, SupportedLanguage.Unknown);

            if (csf == null)
            {
                GrowlsManager.ShowErrorMsg("Итоговый CSF файл пуст.");
                return;
            }

            await _fileService.WriteCsfFileAsync(OutputCsfFilePath, csf);
            GrowlsManager.ShowSuccesMsg(".str → .csf: конвертация завершена");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Не удалось конвертировать .str в .csf");
        }
    }

    [RelayCommand]
    private async Task ConvertCsfToStr()
    {
        if (!CanConvertCsfToStr)
        {
            return;
        }

        try
        {
            CsfFile csf = await _fileService.ReadCsfFileAsync(InputCsfFilePath);
            string? strText = _csfFileConverter.ConvertCsfToStr(csf);
            await File.WriteAllTextAsync(OutputStrFilePath, strText);
            GrowlsManager.ShowSuccesMsg(".csf → .str: конвертация завершена");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Не удалось конвертировать .csf в .str");
        }
    }

    // --- Новые команды конвертации CSF <-> JSON ---
    [RelayCommand]
    private async Task ConvertCsfToJson()
    {
        if (!CanConvertCsfToJson)
        {
            return;
        }

        try
        {
            CsfFile? csf = await _fileService.ReadCsfFileAsync(InputCsfToJsonFilePath);
            string? json = _csfFileConverter.ConvertCsfToJson(csf);

            if (json != null)
            {
                await _fileService.WriteFileAsync(OutputJsonFilePath, json);
            }
            GrowlsManager.ShowSuccesMsg(".csf → .json: конвертация завершена");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Не удалось конвертировать .csf в .json");
        }
    }

    [RelayCommand]
    private async Task ConvertJsonToCsf()
    {
        if (!CanConvertJsonToCsf)
        {
            return;
        }

        try
        {
            // --- ПРАВИЛЬНО: Читаем содержимое файла ---
            string[] jsonContent = await File.ReadAllLinesAsync(InputJsonToCsfFilePath);

            // --- ПРАВИЛЬНО: Передаем содержимое и язык ---
            CsfFile? csf = _csfFileConverter.ConvertJsonToCsf(jsonContent, InputJsonToCsfLang);

            if (csf != null)
            {
                await _fileService.WriteCsfFileAsync(OutputCsfFromJsonFilePath, csf);
                GrowlsManager.ShowSuccesMsg(".json → .csf: конвертация завершена");
            }
            else
            {
                GrowlsManager.ShowErrorMsg("Не удалось создать CSF-файл из JSON.");
            }
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Не удалось конвертировать .json в .csf");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _cancellationTokenSource?.Dispose();
            _disposed = true;
        }
    }
}