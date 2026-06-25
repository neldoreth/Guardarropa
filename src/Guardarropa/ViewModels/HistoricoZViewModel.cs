using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guardarropa.Models;
using Guardarropa.Services;

namespace Guardarropa.ViewModels;

public partial class HistoricoZViewModel : ObservableObject
{
    private readonly BaseDatosServicio _dbServicio;
    private readonly Action _onCerrar;

    [ObservableProperty]
    private ObservableCollection<CierreZ> _cierres = new();

    [ObservableProperty]
    private decimal _totalGeneral;

    [ObservableProperty]
    private int _totalTicketsGeneral;

    [ObservableProperty]
    private int _totalPrendasGeneral;

    public HistoricoZViewModel(BaseDatosServicio dbServicio, Action onCerrar)
    {
        _dbServicio = dbServicio;
        _onCerrar = onCerrar;
        CargarHistorico();
    }

    private void CargarHistorico()
    {
        var lista = _dbServicio.ObtenerHistoricoCierresZ();
        Cierres = new ObservableCollection<CierreZ>(lista);
        TotalGeneral = lista.Sum(c => c.TotalRecaudado);
        TotalTicketsGeneral = lista.Sum(c => c.TotalTickets);
        TotalPrendasGeneral = lista.Sum(c => c.TotalPrendas);
    }

    [RelayCommand]
    private void Cerrar()
    {
        _onCerrar();
    }
}
