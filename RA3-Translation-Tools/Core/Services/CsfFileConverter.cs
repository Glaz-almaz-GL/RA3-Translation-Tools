using Core.Helpers;
using RA3_Translation_Tools.Core.Helpers;
using RA3_Translation_Tools.Core.Interfaces;
using RA3_Translation_Tools.Core.Models.Enums;
using SadPencil.Ra2CsfFile;

namespace RA3_Translation_Tools.Core.Services;

public class CsfFileConverter : ICsfFileConverter
{
    public CsfFile? ConvertStrToCsf(string inputStrText, SupportedLanguage lang)
    {
        CsfFile? csfFile = CsfFileHelper.ConvertStrToCsf(inputStrText);
        csfFile?.Language = CsfFileHelper.ConvertToCsfLang(lang);
        return csfFile;
    }

    public string? ConvertCsfToStr(CsfFile csf)
    {
        return CsfFileHelper.ConvertCsfToStr(csf);
    }

    public CsfFile? ConvertJsonToCsf(string[] inputJsonLines, SupportedLanguage lang)
    {
        CsfFile? csfFile = CsfFileHelper.ConvertJsonToCsf(inputJsonLines);
        csfFile?.Language = CsfFileHelper.ConvertToCsfLang(lang);
        return csfFile;
    }

    public string? ConvertCsfToJson(CsfFile csf)
    {
        return CsfFileHelper.ConvertCsfToJson(csf);
    }
}