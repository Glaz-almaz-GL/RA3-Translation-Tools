using RA3_Translation_Tools.Core.Services;
using RA3_Translation_Tools.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RA3_Translation_Tools.Core.Helpers
{
    public static class TranslationHelper
    {
        public static Dictionary<string, string> ParseTranslationsFromLines(string[] lines)
        {
            Dictionary<string, string> translations = [];

            foreach (string line in lines)
            {
                Match match = OriginalTranslationMatcher.TranslationGroupRegex().Match(line);
                if (match.Success)
                {
                    translations[match.Groups[1].Value] = match.Groups[2].Value;
                }
            }

            return translations;
        }

        public static List<string> GetTextFiles(string folder)
        {
            List<string> files = [.. Directory.GetFiles(folder, "*", SearchOption.AllDirectories).Where(f => Path.GetExtension(f).ToLowerInvariant() is ".str" or ".csf" && !IsFileLocked(f))];
            return files;
        }

        public static bool IsFileLocked(string file)
        {
            try { using FileStream _ = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None); return false; }
            catch (IOException) { return true; }
        }

        public static string GetLanguageCode(SupportedLanguage lang)
        {
            return lang switch
            {
                SupportedLanguage.Auto => "auto",
                SupportedLanguage.Russian => "ru",
                SupportedLanguage.English => "en",
                SupportedLanguage.French => "fr",
                SupportedLanguage.German => "de",
                SupportedLanguage.Spanish => "es",
                SupportedLanguage.Italian => "it",
                SupportedLanguage.Portuguese => "pt",
                SupportedLanguage.Chinese => "zh-cn", // Используем конкретный код для упрощенного китайского
                SupportedLanguage.Japanese => "ja",
                SupportedLanguage.Korean => "ko",
                SupportedLanguage.Arabic => "ar",
                SupportedLanguage.Bengali => "bn",
                SupportedLanguage.Hindi => "hi",
                SupportedLanguage.Tamil => "ta",
                SupportedLanguage.Telugu => "te",
                SupportedLanguage.Malayalam => "ml",
                SupportedLanguage.Kannada => "kn",
                SupportedLanguage.Gujarati => "gu",
                SupportedLanguage.Punjabi => "pa",
                SupportedLanguage.Marathi => "mr",
                SupportedLanguage.Thai => "th",
                SupportedLanguage.Vietnamese => "vi",
                SupportedLanguage.Indonesian => "id",
                SupportedLanguage.Malay => "ms",
                SupportedLanguage.Turkish => "tr",
                SupportedLanguage.Polish => "pl",
                SupportedLanguage.Dutch => "nl",
                SupportedLanguage.Swedish => "sv",
                SupportedLanguage.Finnish => "fi",
                SupportedLanguage.Danish => "da",
                SupportedLanguage.Norwegian => "no",
                SupportedLanguage.Hebrew => "he",
                SupportedLanguage.Persian => "fa",
                SupportedLanguage.Urdu => "ur",
                SupportedLanguage.Croatian => "hr",
                SupportedLanguage.Serbian => "sr", // Используем сербский (кириллица) по умолчанию
                SupportedLanguage.Bulgarian => "bg",
                SupportedLanguage.Romanian => "ro",
                SupportedLanguage.Ukrainian => "uk",
                SupportedLanguage.Czech => "cs",
                SupportedLanguage.Slovak => "sk",
                SupportedLanguage.Slovenian => "sl",
                SupportedLanguage.Hungarian => "hu",
                SupportedLanguage.Estonian => "et",
                SupportedLanguage.Latvian => "lv",
                SupportedLanguage.Lithuanian => "lt",
                SupportedLanguage.Icelandic => "is",
                SupportedLanguage.Greek => "el",
                SupportedLanguage.Tagalog => "tl",
                SupportedLanguage.Swahili => "sw",
                SupportedLanguage.Afrikaans => "af",
                SupportedLanguage.Zulu => "zu",
                SupportedLanguage.Khmer => "km",
                SupportedLanguage.Lao => "lo",
                SupportedLanguage.Myanmar => "my",
                SupportedLanguage.Mongolian => "mn",
                SupportedLanguage.Nepali => "ne",
                SupportedLanguage.Sinhala => "si",
                _ => throw new ArgumentException("Unsupported language")
            };
        }

        public static SupportedLanguage ConvertStringToLanguage(string code)
        {
            return code switch
            {
                "auto" => SupportedLanguage.Auto,
                "ru" => SupportedLanguage.Russian,
                "en" => SupportedLanguage.English,
                "fr" => SupportedLanguage.French,
                "de" => SupportedLanguage.German,
                "es" => SupportedLanguage.Spanish,
                "pt" => SupportedLanguage.Portuguese,
                "zh-CN" => SupportedLanguage.Chinese, // Используем конкретный код из GetLanguageCode
                "ja" => SupportedLanguage.Japanese,
                "ko" => SupportedLanguage.Korean,
                "ar" => SupportedLanguage.Arabic,
                "bn" => SupportedLanguage.Bengali,
                "hi" => SupportedLanguage.Hindi,
                "ta" => SupportedLanguage.Tamil,
                "te" => SupportedLanguage.Telugu,
                "ml" => SupportedLanguage.Malayalam,
                "kn" => SupportedLanguage.Kannada,
                "gu" => SupportedLanguage.Gujarati,
                "pa" => SupportedLanguage.Punjabi,
                "mr" => SupportedLanguage.Marathi,
                "th" => SupportedLanguage.Thai,
                "vi" => SupportedLanguage.Vietnamese,
                "id" => SupportedLanguage.Indonesian,
                "ms" => SupportedLanguage.Malay,
                "tr" => SupportedLanguage.Turkish,
                "pl" => SupportedLanguage.Polish,
                "nl" => SupportedLanguage.Dutch,
                "sv" => SupportedLanguage.Swedish,
                "fi" => SupportedLanguage.Finnish,
                "da" => SupportedLanguage.Danish,
                "no" => SupportedLanguage.Norwegian,
                "he" => SupportedLanguage.Hebrew,
                "fa" => SupportedLanguage.Persian,
                "ur" => SupportedLanguage.Urdu,
                "hr" => SupportedLanguage.Croatian,
                "sr" => SupportedLanguage.Serbian, // Сербский (кириллица)
                "bg" => SupportedLanguage.Bulgarian,
                "ro" => SupportedLanguage.Romanian,
                "uk" => SupportedLanguage.Ukrainian,
                "cs" => SupportedLanguage.Czech, // ISO 639-1 код
                "sk" => SupportedLanguage.Slovak,
                "sl" => SupportedLanguage.Slovenian,
                "hu" => SupportedLanguage.Hungarian,
                "et" => SupportedLanguage.Estonian,
                "lv" => SupportedLanguage.Latvian,
                "lt" => SupportedLanguage.Lithuanian,
                "is" => SupportedLanguage.Icelandic,
                "el" => SupportedLanguage.Greek,
                "tl" => SupportedLanguage.Tagalog,
                "sw" => SupportedLanguage.Swahili,
                "af" => SupportedLanguage.Afrikaans,
                "zu" => SupportedLanguage.Zulu,
                "km" => SupportedLanguage.Khmer,
                "lo" => SupportedLanguage.Lao,
                "my" => SupportedLanguage.Myanmar,
                "mn" => SupportedLanguage.Mongolian,
                "ne" => SupportedLanguage.Nepali,
                "si" => SupportedLanguage.Sinhala,
                _ => SupportedLanguage.Unknown
            };
        }
    }
}
