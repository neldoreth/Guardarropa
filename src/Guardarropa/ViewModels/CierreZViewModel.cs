using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guardarropa.Models;
using Guardarropa.Services;

namespace Guardarropa.ViewModels;

public partial class CierreZViewModel : ObservableObject
{
    private readonly BaseDatosServicio _dbServicio;
    private readonly ImpresionServicio _impresionServicio;
    private readonly Empresa _empresa;
    private readonly Action _onCerrar;
    private readonly Action _onCierreRealizado;

    [ObservableProperty] private string _contrasenaZ = string.Empty;
    [ObservableProperty] private string _mensaje = string.Empty;
    [ObservableProperty] private bool _hayError;
    [ObservableProperty] private bool _cierreRealizado;
    [ObservableProperty] private int _totalTickets;
    [ObservableProperty] private int _totalPrendas;
    [ObservableProperty] private decimal _totalRecaudado;

    private CierreZ? _ultimoCierre;

    public CierreZViewModel(BaseDatosServicio dbServicio, ImpresionServicio impresionServicio,
        Empresa empresa, Action onCerrar, Action onCierreRealizado)
    {
        _dbServicio = dbServicio;
        _impresionServicio = impresionServicio;
        _empresa = empresa;
        _onCerrar = onCerrar;
        _onCierreRealizado = onCierreRealizado;
    }

    [RelayCommand]
    private void RealizarCierre()
    {
        if (string.IsNullOrWhiteSpace(ContrasenaZ))
        {
            Mensaje = "Introduce la contrasena Z";
            HayError = true;
            return;
        }

        if (!_dbServicio.ValidarContrasenaZ(ContrasenaZ))
        {
            Mensaje = "Contrasena Z incorrecta";
            HayError = true;
            return;
        }

        _ultimoCierre = _dbServicio.RealizarCierreZ(_empresa.Id);
        TotalTickets = _ultimoCierre.TotalTickets;
        TotalPrendas = _ultimoCierre.TotalPrendas;
        TotalRecaudado = _ultimoCierre.TotalRecaudado;
        CierreRealizado = true;
        Mensaje = "Cierre Z realizado correctamente";
        HayError = false;
        _onCierreRealizado();
    }

    [RelayCommand]
    private void ImprimirCierre()
    {
        if (_ultimoCierre == null) return;
        try
        {
            var config = _dbServicio.ObtenerConfiguracion();
            _impresionServicio.ImprimirCierreZ(_ultimoCierre, config);
            Mensaje = "Cierre Z impreso correctamente";
            HayError = false;
        }
        catch (Exception ex)
        {
            Mensaje = $"Error al imprimir: {ex.Message}";
            HayError = true;
        }
    }

    [RelayCommand]
    private void Cerrar()
    {
        _onCerrar();
    }
}
