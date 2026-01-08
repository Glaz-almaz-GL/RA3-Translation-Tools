using RA3_Translation_Tools.Core.Models.Enums;
using System.Text;

namespace RA3_Translation_Tools.Core.Interfaces
{
    public interface IEncodingProvider
    {
        Encoding GetEncoding(SupportedLanguage language);
    }
}
