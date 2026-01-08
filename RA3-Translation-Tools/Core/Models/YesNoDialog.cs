using Huskui.Avalonia.Controls;

namespace RA3_Translation_Tools.Core.Models
{
    public partial class YesNoDialog : Dialog
    {

        protected override bool ValidateResult(object? result)
        {
            return true;
        }
    }
}
