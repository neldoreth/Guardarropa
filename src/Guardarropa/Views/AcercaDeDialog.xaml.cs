using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Guardarropa.Views;

public partial class AcercaDeDialog : Window
{
    public AcercaDeDialog()
    {
        InitializeComponent();
        TxtNombre.Text = AppInfo.NombrePrograma;
        TxtVersion.Text = $"Version {AppInfo.Version}";
        TxtDesarrollador.Text = $"Desarrollado por {AppInfo.Desarrollador}";
        CargarHistorial();
    }

    private void CargarHistorial()
    {
        for (var i = 0; i < AppInfo.Historial.Count; i++)
        {
            var entrada = AppInfo.Historial[i];

            var titulo = new TextBlock
            {
                Text = $"Version {entrada.Version} ({entrada.Fecha})",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("ColorAccento"),
                Margin = new Thickness(0, i == 0 ? 0 : 16, 0, 8),
            };
            PanelHistorial.Children.Add(titulo);

            foreach (var cambio in entrada.Cambios)
            {
                var linea = new TextBlock
                {
                    Text = $"• {cambio}",
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = (Brush)FindResource("TextoTerciario"),
                    Margin = new Thickness(0, 0, 0, 4),
                };
                PanelHistorial.Children.Add(linea);
            }
        }
    }

    private void Cerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
