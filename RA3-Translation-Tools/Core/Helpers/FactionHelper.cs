using System;
using System.Collections.Generic;
using System.Linq;

namespace RA3_Translation_Tools.Core.Helpers
{
    public static class FactionHelper
    {
        private static readonly string[] _defaultFactions = ["ALLIED", "JAPAN", "SOVIET"];

        public static List<string> BuildFactionList(string? newFactionTags)
        {
            if (!string.IsNullOrWhiteSpace(newFactionTags))
            {
                List<string> additionalFactions = [.. newFactionTags
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToUpper())];

                return [.. _defaultFactions, .. additionalFactions];
            }

            return [.. _defaultFactions];
        }

        public static string NormalizeSuffix(string suffix, List<string> factions)
        {
            string normalized = suffix.ToUpper();
            foreach (string f in factions)
            {
                if (normalized.StartsWith(f))
                {
                    return normalized[f.Length..];
                }
            }
            return normalized;
        }

        public static bool IsSpecialPrefix(string prefix)
        {
            return prefix.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                   prefix.Equals("Type", StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class KeyHelper
    {
        public static (string prefix, string suffix) SplitKey(string fullKey)
        {
            string[] parts = fullKey.Split(':');
            string prefix = parts[0];
            string suffix = string.Concat(parts.Skip(1));
            return (prefix, suffix);
        }
    }
}
