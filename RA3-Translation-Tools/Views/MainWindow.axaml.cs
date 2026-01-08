using Huskui.Avalonia.Controls;
using RA3_Translation_Tools.Managers;

namespace RA3_Translation_Tools.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();
        GrowlsManager.Initialize(this);
        DialogsManager.Initialize(this, this);
        Height = 700;
        Width = 700;
    }
}
