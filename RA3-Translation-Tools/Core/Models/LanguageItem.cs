using RA3_Translation_Tools.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA3_Translation_Tools.Core.Models
{
    public class LanguageItem
    {
        public SupportedLanguage Language { get; set; }
        public string DisplayName { get; set; }

        public LanguageItem(SupportedLanguage language, string displayName)
        {
            Language = language;
            DisplayName = displayName;
        }
    }
}
