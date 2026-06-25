using System.Windows;
using System.Windows.Controls;
using Guardarropa.ViewModels;

namespace Guardarropa.Views;

public partial class AjustesView : UserControl
{
    public AjustesView()
    {
        InitializeComponent();
    }

    private void TxtContrasenaActual_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is AjustesViewModel vm)
            vm.ContrasenaActual = TxtContrasenaActual.Password;
    }

    private void TxtNuevaContrasena_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is AjustesViewModel vm)
            vm.NuevaContrasena = TxtNuevaContrasena.Password;
    }

    private void TxtConfirmarNueva_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is AjustesViewModel vm)
            vm.ConfirmarNuevaContrasena = TxtConfirmarNueva.Password;
    }
}
