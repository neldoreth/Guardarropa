using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guardarropa.Models;
using Guardarropa.Services;

namespace Guardarropa.ViewModels;

public partial class PrincipalViewModel : ObservableObject
{
    private readonly BaseDatosServicio _dbServicio;
    private readonly ImpresionServicio _impresionServicio;
    private readonly Empresa _empresa;
    private readonly Action _onVolverSeleccion;
    private readonly Action _onAbrirAjustes;
    private readonly Action _onAbrirCierreZ;

    [ObservableProperty]
    private string _nombreEmpresa = string.Empty;

    [ObservableProperty]
    private int _numeroPrendas = 1;

    [ObservableProperty]
    private decimal _precioPorPrenda;

    [ObservableProperty]
    private decimal _precioTotal;

    [ObservableProperty]
    private int _siguienteNumeroTicket;

    [ObservableProperty]
    private string _mensaje = string.Empty;

    [ObservableProperty]
    private bool _hayError;

    public PrincipalViewModel(BaseDatosServicio dbServicio, ImpresionServicio impresionServicio,
        Empresa empresa, Action onVolverSeleccion, Action onAbrirAjustes, Action onAbrirCierreZ)
    {
        _dbServicio = dbServicio;
        _impresionServicio = impresionServicio;
        _empresa = empresa;
        _onVolverSeleccion = onVolverSeleccion;
        _onAbrirAjustes = onAbrirAjustes;
        _onAbrirCierreZ = onAbrirCierreZ;

        NombreEmpresa = empresa.Nombre;
        var config = _dbServicio.ObtenerConfiguracion();
        PrecioPorPrenda = config.PrecioPorPrenda;
        SiguienteNumeroTicket = _dbServicio.ObtenerSiguienteNumeroTicket(empresa.Id);
        CalcularPrecio();
    }

    partial void OnNumeroPrendasChanged(int value)
    {
        if (value < 1) NumeroPrendas = 1;
        CalcularPrecio();
    }

    private void CalcularPrecio()
    {
        PrecioTotal = NumeroPrendas * PrecioPorPrenda;
    }

    [RelayCommand]
    private void ImprimirTicket()
    {
        if (NumeroPrendas < 1)
        {
            Mensaje = "El numero de prendas debe ser al menos 1";
            HayError = true;
            return;
        }

        try
        {
            var config = _dbServicio.ObtenerConfiguracion();
            var ticket = _dbServicio.CrearTicket(_empresa.Id, NumeroPrendas, PrecioPorPrenda);
            _impresionServicio.ImprimirTicket(ticket, config);
            SiguienteNumeroTicket = _dbServicio.ObtenerSiguienteNumeroTicket(_empresa.Id);
            NumeroPrendas = 1;
            Mensaje = $"Ticket #{ticket.NumeroTicket} impreso correctamente";
            HayError = false;
        }
        catch (Exception ex)
        {
            Mensaje = $"Error al imprimir: {ex.Message}";
            HayError = true;
        }
    }

    [RelayCommand]
    private void VolverSeleccion()
    {
        _onVolverSeleccion();
    }

    [RelayCommand]
    private void AbrirAjustes()
    {
        _onAbrirAjustes();
    }

    [RelayCommand]
    private void AbrirCierreZ()
    {
        _onAbrirCierreZ();
    }

    public void Refrescar()
    {
        var config = _dbServicio.ObtenerConfiguracion();
        PrecioPorPrenda = config.PrecioPorPrenda;
        SiguienteNumeroTicket = _dbServicio.ObtenerSiguienteNumeroTicket(_empresa.Id);
        CalcularPrecio();
    }
}
