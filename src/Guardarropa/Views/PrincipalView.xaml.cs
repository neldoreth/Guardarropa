using System.Windows;
using System.Windows.Controls;
using Guardarropa.ViewModels;

namespace Guardarropa.Views;

public partial class PrincipalView : UserControl
{
    public PrincipalView()
    {
        InitializeComponent();
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
