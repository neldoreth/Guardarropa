using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Guardarropa.ViewModels;

namespace Guardarropa.Views;

public partial class PrincipalView : UserControl
{
    public PrincipalView()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        TxtNumeroPercha.Focus();
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        TxtNumeroPercha.Focus();
    }

    private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is PrincipalViewModel vm)
        {
            vm.ImprimirTicketCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void BtnMenos_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PrincipalViewModel vm && vm.NumeroPrendas > 1)
            vm.NumeroPrendas--;
    }

    private void BtnMas_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PrincipalViewModel vm)
            vm.NumeroPrendas++;
    }
}
