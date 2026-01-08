using RA3_Translation_Tools.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA3_Translation_Tools.Core.Models
{
    public class LanguageItem(SupportedLanguage language, string displayName)
    {
        public SupportedLanguage Language { get; set; } = language;
        public string DisplayName { get; set; } = displayName;
    }
}
