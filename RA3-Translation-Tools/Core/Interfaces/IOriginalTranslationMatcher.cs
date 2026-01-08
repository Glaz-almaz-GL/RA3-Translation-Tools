using System.Collections.Generic;
using System.Threading.Tasks;

namespace RA3_Translation_Tools.Core.Interfaces
{
    public interface IOriginalTranslationMatcher
    {
        string ApplyOriginalTranslations(string iniText, string? newFactionTags);
        Task<Dictionary<string, string>> LoadOriginalTranslationsAsync(string filePath);
    }
}
