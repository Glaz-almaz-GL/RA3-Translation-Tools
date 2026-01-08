using Newtonsoft.Json.Linq;
using RA3_Translation_Tools.Core.Constants;
using RA3_Translation_Tools.Core.Helpers;
using RA3_Translation_Tools.Core.Models.Enums;
using RA3_Translation_Tools.Managers;
using SadPencil.Ra2CsfFile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Core.Helpers;

public static class CsfFileHelper
{
    private sealed class ParseState(CsfFile output)
    {
        public string? CurrentKey { get; set; }
        public StringBuilder? ValueBuilder { get; set; }
        public CsfFile Output { get; } = output;

        public void AddCurrentValue()
        {
            if (CurrentKey != null && ValueBuilder?.Length > 0)
            {
                string value = ValueBuilder.ToString();
                if (!Output.Labels.ContainsKey(CurrentKey))
                {
                    Output.AddLabel(CurrentKey.ToUpper(), value);
                }
            }
            CurrentKey = null;
            ValueBuilder = null;
        }

        public void StartNewBlock(string key)
        {
            AddCurrentValue();
            CurrentKey = key.Trim();
            ValueBuilder = new StringBuilder();
        }

        public void AppendValueLine(string? line)
        {
            if (string.IsNullOrWhiteSpace(line) || ValueBuilder == null)
            {
                return;
            }

            if (ValueBuilder.Length > 0)
            {
                ValueBuilder.Append(' ');
            }

            ValueBuilder.Append(line.Replace("\\r\\n", "\\n").Replace("\\n", "\r\n"));
        }
    }

    public static CsfFile? ConvertStrToCsf(string inputText)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                Debug.WriteLine("[CsfFileHelper] Входной текст пуст или null.");
                return null;
            }

            CsfFile csf = new();
            ConfigureCsf(csf);

            string[] lines = NormalizeLines(inputText);
            ParseState state = new(csf);

            foreach (string line in lines)
            {
                ProcessLine(line, state);
            }

            state.AddCurrentValue();
            Debug.WriteLine($"[CsfFileHelper] Успешно извлечено {csf.Labels.Count} строк.");
            return csf;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка конвертации в Csf файл: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка конвертации в Csf файл");
            return null;
        }
    }

    public static string? ConvertCsfToStr(CsfFile? csf)
    {
        try
        {
            if (csf == null)
            {
                Debug.WriteLine("[CsfFileHelper] Входной CSF объект null.");
                return null;
            }

            if (csf.Labels == null)
            {
                Debug.WriteLine("[CsfFileHelper] Labels в CSF объекте null.");
                return null;
            }

            List<string> lines = [];
            foreach ((string? key, string? value) in csf.Labels)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    Debug.WriteLine("[CsfFileHelper] Пропущена запись с пустым ключом.");
                    continue;
                }

                lines.Add(key);
                if (value?.Contains('\n') == true)
                {
                    // Проверяем, что value не null перед Split
                    string[] parts = value.Split('\n');
                    foreach (string part in parts)
                    {
                        // Экранируем кавычки при необходимости
                        string escapedPart = part.Replace("\"", "\\\"");
                        lines.Add($"\t\"{escapedPart}\"");
                    }
                }
                else
                {
                    // Экранируем кавычки при необходимости
                    string escapedValue = value?.Replace("\"", "\\\"") ?? string.Empty;
                    lines.Add($"\t\"{escapedValue}\"");
                }
                lines.Add("End");
                lines.Add("");
            }
            return string.Join("\r\n", lines);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка конвертации в Str файл: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка конвертации в Str файл");
            return null;
        }
    }

    public static CsfFile? ConvertJsonToCsf(string[]? jsonLines, SupportedLanguage targetLanguage = SupportedLanguage.Unknown)
    {
        try
        {
            if (jsonLines == null)
            {
                Debug.WriteLine("[CsfFileHelper] Входной массив JSON строк null.");
                return null;
            }

            if (jsonLines.Length == 0)
            {
                Debug.WriteLine("[CsfFileHelper] Входной массив JSON строк пуст.");
                return null;
            }

            Debug.WriteLine($"[CsfFileHelper] Начало парсинга JSON. Кол-во строк: {jsonLines.Length}");

            CsfFile csf = new();
            // Если язык указан, настраиваем CSF с ним
            if (targetLanguage != SupportedLanguage.Unknown)
            {
                ConfigureCsf(csf, targetLanguage);
            }
            else
            {
                ConfigureCsf(csf);
            }

            Dictionary<string, string> gameStrings = TranslationHelper.ParseTranslationsFromLines(jsonLines);

            if (gameStrings == null)
            {
                Debug.WriteLine("[CsfFileHelper] ParseTranslationsFromLines вернул null.");
                return null;
            }

            foreach (KeyValuePair<string, string> gameString in gameStrings)
            {
                string key = gameString.Key;
                string? value = gameString.Value;

                if (string.IsNullOrWhiteSpace(key))
                {
                    Debug.WriteLine("[CsfFileHelper] Пропущена запись из JSON с пустым ключом.");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Заменяем все символы новой строки на пробелы
                    value = value.Replace("\n", " ").Replace("\r", " ");

                    if (!csf.Labels.ContainsKey(key))
                    {
                        // Добавляем ключ в верхнем регистре
                        csf.AddLabel(key.ToUpper(), value);
                    }
                }
            }

            Debug.WriteLine($"[CsfFileHelper] Успешно извлечено {csf.Labels.Count} строк из JSON.");
            return csf;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка парсинга JSON: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка конвертации JSON в Csf файл");
            return null;
        }
    }

    public static string? ConvertCsfToJson(CsfFile? csf)
    {
        try
        {
            if (csf == null)
            {
                Debug.WriteLine("[CsfFileHelper] Входной CSF объект null.");
                return null;
            }

            if (csf.Labels == null)
            {
                Debug.WriteLine("[CsfFileHelper] Labels в CSF объекте null.");
                return null;
            }

            JObject jsonObject = [];
            JObject gameStringsObject = []; // Используем конструктор

            foreach (KeyValuePair<string, string> label in csf.Labels)
            {
                if (!string.IsNullOrWhiteSpace(label.Key))
                {
                    gameStringsObject[label.Key] = label.Value ?? string.Empty;
                }
            }

            jsonObject["gamestrings"] = gameStringsObject;
            return jsonObject.ToString();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка конвертации Csf файла в JSON: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка конвертации Csf файла в JSON");
            return null;
        }
    }

    public static string GetJsonLanguageSpecificPath(string? langKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(langKey))
            {
                Debug.WriteLine("[CsfFileHelper] Получен пустой или null ключ языка для JSON.");
                langKey = "en"; // Значение по умолчанию
            }

            string jsonFileName = langKey.ToLowerInvariant() + ".json";

            string fullPath = Path.Combine(PathConstants.CasheFolder, jsonFileName);
            return fullPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка при получении пути к JSON файлу: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка при получении оригинальных фраз");
            return string.Empty;
        }
    }

    public static string GetJsonLanguageSpecificPath(SupportedLanguage language)
    {
        try
        {
            string jsonFileName = TranslationHelper.GetLanguageCode(language).ToLowerInvariant() + ".json";

            string fullPath = Path.Combine(PathConstants.CasheFolder, jsonFileName);
            return fullPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка при получении пути к JSON файлу: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка при получении оригинальных фраз");
            return string.Empty;
        }
    }

    public static string LoadJsonLanguageSpecificData(string? langKey)
    {
        try
        {
            string path = GetJsonLanguageSpecificPath(langKey);
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.WriteLine("[CsfFileHelper] Не удалось получить путь к JSON файлу для чтения.");
                return string.Empty;
            }

            if (!File.Exists(path))
            {
                Debug.WriteLine($"[CsfFileHelper] JSON файл не найден: {path}");
                return string.Empty;
            }

            return File.ReadAllText(path);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка чтения JSON файла: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка при чтении JSON файла");
            return string.Empty;
        }
    }

    public static string[] LoadJsonLanguageSpecificData(SupportedLanguage language)
    {
        try
        {
            string path = GetJsonLanguageSpecificPath(language);
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.WriteLine("[CsfFileHelper] Не удалось получить путь к JSON файлу для чтения строк.");
                return []; // Возвращаем пустой массив
            }

            if (!File.Exists(path))
            {
                Debug.WriteLine($"[CsfFileHelper] JSON файл не найден: {path}");
                return [];
            }

            return File.ReadAllLines(path);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка чтения строк из JSON файла: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка при чтении строк из JSON файла");
            return []; // Возвращаем пустой массив
        }
    }

    public static string GetCsfLanguageSpecificPath(string? langKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(langKey))
            {
                Debug.WriteLine("[CsfFileHelper] Получен пустой или null ключ языка для CSF.");
                langKey = "En"; // Значение по умолчанию
            }

            string csfFileName = langKey.ToLowerInvariant() + ".csf";

            string fullPath = Path.Combine(PathConstants.CasheFolder, csfFileName);
            return fullPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка при получении пути к CSF файлу: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка при получении оригинальных фраз из CSF");
            return string.Empty;
        }
    }

    public static string GetCsfLanguageSpecificPath(SupportedLanguage language)
    {
        try
        {
            string csfFileName = TranslationHelper.GetLanguageCode(language).ToLowerInvariant() + ".csf";

            string fullPath = Path.Combine(PathConstants.CasheFolder, csfFileName);
            return fullPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка при получении пути к CSF файлу: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка при получении оригинальных фраз из CSF");
            return string.Empty;
        }
    }

    public static CsfFile? LoadCsfLanguageSpecificData(string? langKey)
    {
        try
        {
            string csfPath = GetCsfLanguageSpecificPath(langKey);

            if (string.IsNullOrWhiteSpace(csfPath))
            {
                Debug.WriteLine("[CsfFileHelper] Не удалось получить путь к CSF файлу для загрузки.");
                return null;
            }

            if (!File.Exists(csfPath))
            {
                Debug.WriteLine($"[CsfFileHelper] CSF файл не найден: {csfPath}");
                return null;
            }

            using FileStream stream = File.OpenRead(csfPath);
            return CsfFile.LoadFromCsfFile(stream);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка загрузки CSF файла: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка при загрузке CSF файла");
            return null;
        }
    }

    public static CsfFile? LoadCsfLanguageSpecificData(SupportedLanguage language)
    {
        try
        {
            string csfPath = GetCsfLanguageSpecificPath(language);

            if (string.IsNullOrWhiteSpace(csfPath))
            {
                Debug.WriteLine("[CsfFileHelper] Не удалось получить путь к CSF файлу для загрузки по языку.");
                return null;
            }

            if (!File.Exists(csfPath))
            {
                Debug.WriteLine($"[CsfFileHelper] CSF файл не найден: {csfPath}");
                return null;
            }

            using FileStream stream = File.OpenRead(csfPath);
            return CsfFile.LoadFromCsfFile(stream);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CsfFileHelper] Ошибка загрузки CSF файла по языку: {ex}");
            GrowlsManager.ShowErrorMsg(ex, "Ошибка при загрузке CSF файла");
            return null;
        }
    }

    // --- Внутренние методы (остаются private static) ---
    private static string[] NormalizeLines(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? [] : text.Replace("\r\n", "\n").Replace("\n", "\r\n").Split("\r\n");
    }

    private static void ProcessLine(string rawLine, ParseState state)
    {
        if (state == null || string.IsNullOrWhiteSpace(rawLine))
        {
            return;
        }

        string trimmed = rawLine.Trim();
        if (trimmed.StartsWith("//"))
        {
            return;
        }

        if (IsEndMarker(trimmed))
        {
            state.AddCurrentValue();
            return;
        }

        if (TryGetValueFromQuotedLine(trimmed, out string? value))
        {
            // Заменяем все символы новой строки на пробелы
            state.AppendValueLine(value);
            return;
        }

        state.StartNewBlock(trimmed);
    }

    private static bool IsEndMarker(string input)
    {
        return input.Equals("End", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetValueFromQuotedLine(string input, out string? value)
    {
        value = null;
        if (input.Length >= 2 && input[0] == '"' && input[^1] == '"')
        {
            value = input[1..^1];
            return true;
        }
        return false;
    }

    public static CsfLang ConvertToCsfLang(SupportedLanguage lang)
    {
        return lang switch
        {
            SupportedLanguage.English => CsfLang.EnglishUS,
            SupportedLanguage.French => CsfLang.French,
            SupportedLanguage.German => CsfLang.German,
            SupportedLanguage.Spanish => CsfLang.Spanish,
            SupportedLanguage.Italian => CsfLang.Italian,
            SupportedLanguage.Japanese => CsfLang.Japanese,
            SupportedLanguage.Korean => CsfLang.Korean,
            SupportedLanguage.Chinese => CsfLang.Chinese,
            _ => CsfLang.EnglishUS
        };
    }

    public static void ConfigureCsf(CsfFile? csf, SupportedLanguage lang)
    {
        if (csf == null)
        {
            return;
        }

        csf.Version = 3;
        csf.Language = ConvertToCsfLang(lang);
        csf.Options.Encoding1252WriteWorkaround = true;
        csf.Options.Encoding1252ReadWorkaround = true;
    }

    public static void ConfigureCsf(CsfFile? csf)
    {
        if (csf == null)
        {
            return;
        }

        csf.Version = 3;
        csf.Language = 0;
        csf.Options.Encoding1252WriteWorkaround = true;
        csf.Options.Encoding1252ReadWorkaround = true;
    }
}