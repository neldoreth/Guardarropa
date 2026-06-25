using System.Windows;
using Guardarropa.Services;

namespace Guardarropa.Views;

public partial class ContrasenaDialog : Window
{
    private readonly BaseDatosServicio _dbServicio;

    public bool Autenticado { get; private set; }

    public ContrasenaDialog(BaseDatosServicio dbServicio)
    {
        InitializeComponent();
        _dbServicio = dbServicio;
    }

    private void Aceptar_Click(object sender, RoutedEventArgs e)
    {
        if (_dbServicio.ValidarContrasenaMaestra(TxtContrasena.Password))
        {
            Autenticado = true;
            DialogResult = true;
            Close();
        }
        else
        {
            TxtError.Text = "Contrasena incorrecta";
        }
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
