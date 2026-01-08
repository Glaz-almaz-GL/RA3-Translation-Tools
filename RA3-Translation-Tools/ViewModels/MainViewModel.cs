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

    // === .big Files ===
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

    // === Conversion ===
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

    // --- New Properties for CSF <-> JSON ---
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
    // --- End of New Properties ---

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

    // --- New Can Properties for CSF <-> JSON ---
    public bool CanConvertCsfToJson =>
        !string.IsNullOrWhiteSpace(InputCsfToJsonFilePath) &&
        !string.IsNullOrWhiteSpace(OutputJsonFilePath) &&
        File.Exists(InputCsfToJsonFilePath);

    public bool CanConvertJsonToCsf =>
        !string.IsNullOrWhiteSpace(InputJsonToCsfFilePath) &&
        !string.IsNullOrWhiteSpace(OutputCsfFromJsonFilePath) &&
        File.Exists(InputJsonToCsfFilePath) &&
        InputJsonToCsfLang != SupportedLanguage.Unknown;
    // --- End of New Properties ---

    // === File/Folder Selection Commands ===
    [RelayCommand]
    private async Task SelectInputBigFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync(
            "Select .big file",
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
            "Save as .big file",
            exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputBigFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectUnpackDirectory()
    {
        IStorageFolder? folder = await DialogsManager.OpenDirectoryDialogAsync("Select folder to unpack .big file");

        if (folder?.Path?.LocalPath is { } path)
        {
            UnpackDirectory = path.TrimEnd(Path.DirectorySeparatorChar);
        }
    }

    [RelayCommand]
    private async Task SelectPackedDirectory()
    {
        IStorageFolder? folder = await DialogsManager.OpenDirectoryDialogAsync("Select folder with files to pack");

        if (folder?.Path?.LocalPath is { } path)
        {
            PackedDirectory = path.TrimEnd(Path.DirectorySeparatorChar);
        }
    }

    [RelayCommand]
    private async Task SelectInputStrFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync("Select .str file", ["*.str"]);

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

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync("Save as .csf file", exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputCsfFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectInputCsfFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync("Select .csf file", ["*.csf"]);

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

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync("Save as .str file", exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputStrFilePath = filePath;
        }
    }

    // --- New File Selection Commands for CSF <-> JSON ---
    [RelayCommand]
    private async Task SelectInputCsfToJsonFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync("Select .csf file", ["*.csf"]);

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

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync("Save as .json file", exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputJsonFilePath = filePath;
        }
    }

    [RelayCommand]
    private async Task SelectInputJsonToCsfFile()
    {
        IStorageFile? path = await DialogsManager.OpenSingleFileDialogAsync("Select .json file", ["*.json"]);

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

        IStorageFile? path = await DialogsManager.SaveFileDialogAsync("Save as .csf file", exampleFileName);

        if (path?.Path?.LocalPath is { } filePath)
        {
            OutputCsfFromJsonFilePath = filePath;
        }
    }
    // --- End of New Commands ---

    // === Main Commands ===
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
            GrowlsManager.ShowSuccesMsg($"File {Path.GetFileName(InputBigFilePath)} successfully unpacked!");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Failed to unpack .big file");
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
            GrowlsManager.ShowSuccesMsg($"Directory {Path.GetDirectoryName(PackedDirectory)} successfully packed into file {Path.GetFileName(OutputBigFilePath)}!");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Failed to pack .big file");
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
                GrowlsManager.ShowErrorMsg("The resulting CSF file is empty.");
                return;
            }

            await _fileService.WriteCsfFileAsync(OutputCsfFilePath, csf);
            GrowlsManager.ShowSuccesMsg(".str → .csf: conversion completed");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Failed to convert .str to .csf");
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
            GrowlsManager.ShowSuccesMsg(".csf → .str: conversion completed");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Failed to convert .csf to .str");
        }
    }

    // --- New CSF <-> JSON Conversion Commands ---
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
            GrowlsManager.ShowSuccesMsg(".csf → .json: conversion completed");
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Failed to convert .csf to .json");
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
            // --- CORRECT: Read file content ---
            string[] jsonContent = await File.ReadAllLinesAsync(InputJsonToCsfFilePath);

            // --- CORRECT: Pass content and language ---
            CsfFile? csf = _csfFileConverter.ConvertJsonToCsf(jsonContent, InputJsonToCsfLang);

            if (csf != null)
            {
                await _fileService.WriteCsfFileAsync(OutputCsfFromJsonFilePath, csf);
                GrowlsManager.ShowSuccesMsg(".json → .csf: conversion completed");
            }
            else
            {
                GrowlsManager.ShowErrorMsg("Failed to create CSF file from JSON.");
            }
        }
        catch (Exception ex)
        {
            GrowlsManager.ShowErrorMsg(ex, "Failed to convert .json to .csf");
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