using System.Windows.Controls;
using Guardarropa.ViewModels;

namespace Guardarropa.Views;

public partial class PrimerInicioView : UserControl
{
    public PrimerInicioView()
    {
        InitializeComponent();
    }

    private void TxtContrasena_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is PrimerInicioViewModel vm)
            vm.Contrasena = TxtContrasena.Password;
    }

    private void TxtConfirmar_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is PrimerInicioViewModel vm)
            vm.ConfirmarContrasena = TxtConfirmar.Password;
    }
}
