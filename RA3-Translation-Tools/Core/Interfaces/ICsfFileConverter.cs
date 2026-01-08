using RA3_Translation_Tools.Core.Models.Enums;
using SadPencil.Ra2CsfFile;

namespace RA3_Translation_Tools.Core.Interfaces
{
    public interface ICsfFileConverter
    {
        CsfFile? ConvertStrToCsf(string inputStrText, SupportedLanguage lang);
        string? ConvertCsfToStr(CsfFile csf);
        CsfFile? ConvertJsonToCsf(string[] inputJsonLines, SupportedLanguage lang);
        string? ConvertCsfToJson(CsfFile csf);
    }
}
