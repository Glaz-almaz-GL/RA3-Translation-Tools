using RA3_Translation_Tools.Core.Helpers;
using RA3_Translation_Tools.Core.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RA3_Translation_Tools.Core.Services
{
    public partial class OriginalTranslationMatcher : IOriginalTranslationMatcher
    {
        private Dictionary<string, string> _translations = [];

        public async Task<Dictionary<string, string>> LoadOriginalTranslationsAsync(string filePath)
        {
            _translations.Clear();

            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"[DEBUG] Файл оригинальных переводов не найден: {filePath}");
                return [];
            }

            string[] lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
            _translations = TranslationHelper.ParseTranslationsFromLines(lines);

            Debug.WriteLine($"[INFO] Загружено {_translations.Count} оригинальных фраз.");

            return _translations;
        }

        public string ApplyOriginalTranslations(string iniText, string? newFactionTags)
        {
            if (_translations.Count == 0)
            {
                return iniText;
            }

            List<string> factions = FactionHelper.BuildFactionList(newFactionTags);
            Regex regex = OriginalTranslationsRegex();
            Regex engTagRegex = EngTagRegex();

            return regex.Replace(iniText, match => TranslationMatcher.ProcessMatch(match, _translations, factions, engTagRegex));
        }

        [GeneratedRegex(@"^(?<key>[A-Z_]+:[^\r\n]*)\s*\r?\n(?<value>.*?)(?=\r?\n\s*End\s*\r?\n|\r?\n\s*End\s*$|End\s*$|\z)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline, "ru-RU")]
        private static partial Regex OriginalTranslationsRegex();

        [GeneratedRegex(@"^\s*""([^""]+)""\s*:\s*""([^""]*)""[,\s]*$")]
        public static partial Regex TranslationGroupRegex();

        [GeneratedRegex(@"\s*End\s*$", RegexOptions.IgnoreCase | RegexOptions.RightToLeft, "ru-RU")]
        private static partial Regex EngTagRegex();
    }
}