using System.Windows;
using System.Windows.Controls;
using Guardarropa.ViewModels;

namespace Guardarropa.Views;

public partial class CierreZView : UserControl
{
    public CierreZView()
    {
        InitializeComponent();
    }

    private void TxtContrasenaZ_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is CierreZViewModel vm)
            vm.ContrasenaZ = TxtContrasenaZ.Password;
    }
}
