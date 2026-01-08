using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RA3_Translation_Tools.Core.Helpers
{
    public static class TranslationMatcher
    {
        public static string? FindFactionMatch(Dictionary<string, string> translations, string prefix, string suffix, List<string> factions)
        {
            string searchPrefix = prefix.ToUpper();
            string cleanSuffix = FactionHelper.NormalizeSuffix(suffix, factions);

            string? matchKey = translations.Keys
                .Where(k => k.StartsWith(searchPrefix + ":", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(k =>
                {
                    string origSuffix = k[(searchPrefix.Length + 1)..];
                    string normalizedOrig = FactionHelper.NormalizeSuffix(origSuffix, factions);
                    return normalizedOrig == cleanSuffix;
                });

            return matchKey != null ? translations[matchKey] : null;
        }

        public static string? TryGetOriginalValue(Dictionary<string, string> translations, string prefix, string suffix, string fullKey, List<string> factions)
        {
            // Fast path: exact match
            if (translations.TryGetValue(fullKey, out string? exact))
            {
                return exact;
            }

            // Special handling for Name:/Type:
            return FactionHelper.IsSpecialPrefix(prefix) ? FindFactionMatch(translations, prefix, suffix, factions) : null;
        }

        public static string ProcessMatch(Match match, Dictionary<string, string> translations, List<string> factions, Regex engTagRegex)
        {
            string fullKey = match.Groups["key"].Value.Trim();
            (string prefix, string suffix) = KeyHelper.SplitKey(fullKey);

            string? originalValue = TryGetOriginalValue(translations, prefix, suffix, fullKey, factions);

            if (originalValue == null)
            {
                return match.Value;
            }

            bool endsWithEnd = engTagRegex.IsMatch(match.Value);
            string replacement = $"{fullKey}\n\t\"{originalValue}\"" + (endsWithEnd ? "\nEnd" : "");
            return replacement;
        }
    }
}
