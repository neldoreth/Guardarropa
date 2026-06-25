using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guardarropa.Models;
using Guardarropa.Services;

namespace Guardarropa.ViewModels;

public partial class SeleccionEmpresaViewModel : ObservableObject
{
    private readonly BaseDatosServicio _dbServicio;
    private readonly Action<Empresa> _onEmpresaSeleccionada;
    private readonly Action _onAbrirAjustes;

    [ObservableProperty]
    private ObservableCollection<Empresa> _empresas = new();

    [ObservableProperty]
    private Empresa? _empresaSeleccionada;

    [ObservableProperty]
    private string _error = string.Empty;

    public SeleccionEmpresaViewModel(BaseDatosServicio dbServicio, Action<Empresa> onEmpresaSeleccionada, Action onAbrirAjustes)
    {
        _dbServicio = dbServicio;
        _onEmpresaSeleccionada = onEmpresaSeleccionada;
        _onAbrirAjustes = onAbrirAjustes;
        CargarEmpresas();
    }

    public void CargarEmpresas()
    {
        Empresas = new ObservableCollection<Empresa>(_dbServicio.ObtenerEmpresas());
    }

    [RelayCommand]
    private void Continuar()
    {
        if (EmpresaSeleccionada == null)
        {
            Error = "Selecciona una empresa";
            return;
        }
        _onEmpresaSeleccionada(EmpresaSeleccionada);
    }

    [RelayCommand]
    private void AbrirAjustes()
    {
        _onAbrirAjustes();
    }
}
