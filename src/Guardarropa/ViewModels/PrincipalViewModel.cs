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
    private int _numeroPercha;

    [ObservableProperty]
    private string _fechaActual = string.Empty;

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
        NumeroPercha = _dbServicio.ObtenerSiguienteNumeroPercha(empresa.Id);
        FechaActual = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy");
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

        var config = _dbServicio.ObtenerConfiguracion();

        if (string.IsNullOrWhiteSpace(config.NombreImpresora))
        {
            Mensaje = "No hay impresora configurada. Ve a Ajustes > Impresora para seleccionar una.";
            HayError = true;
            return;
        }

        try
        {
            var ticket = _dbServicio.CrearTicket(_empresa.Id, NumeroPrendas, PrecioPorPrenda, NumeroPercha);
            _impresionServicio.ImprimirTicket(ticket, config);
            NumeroPercha = _dbServicio.ObtenerSiguienteNumeroPercha(_empresa.Id);
            NumeroPrendas = 1;
            Mensaje = $"Percha #{ticket.NumeroPercha} impresa correctamente";
            HayError = false;
        }
        catch (Exception ex)
        {
            Mensaje = $"Error al imprimir: {ex.Message}";
            HayError = true;
        }
    }

    [RelayCommand]
    private void VolverSeleccion() => _onVolverSeleccion();

    [RelayCommand]
    private void AbrirAjustes() => _onAbrirAjustes();

    [RelayCommand]
    private void AbrirCierreZ() => _onAbrirCierreZ();

    public void Refrescar()
    {
        var config = _dbServicio.ObtenerConfiguracion();
        PrecioPorPrenda = config.PrecioPorPrenda;
        NumeroPercha = _dbServicio.ObtenerSiguienteNumeroPercha(_empresa.Id);
        CalcularPrecio();
    }
}
