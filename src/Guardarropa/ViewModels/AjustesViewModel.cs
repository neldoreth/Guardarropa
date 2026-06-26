using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guardarropa.Models;
using Guardarropa.Services;

namespace Guardarropa.ViewModels;

public partial class AjustesViewModel : ObservableObject
{
    private readonly BaseDatosServicio _dbServicio;
    private readonly Action _onCerrar;
    private readonly Action _onAbrirHistorico;

    // --- Tema ---
    [ObservableProperty] private bool _temaOscuro;

    public bool TemaClaro
    {
        get => !TemaOscuro;
        set
        {
            if (value != !TemaOscuro)
            {
                TemaOscuro = !value;
                OnPropertyChanged(nameof(TemaClaro));
            }
        }
    }

    partial void OnTemaOscuroChanged(bool value)
    {
        OnPropertyChanged(nameof(TemaClaro));
        App.CambiarTema(value);
    }

    // --- Configuracion general ---
    [ObservableProperty] private decimal _precioPorPrenda;
    [ObservableProperty] private string _nombreImpresora = string.Empty;
    [ObservableProperty] private int _anchoTicketMm;
    [ObservableProperty] private int _altoTicketMm;
    [ObservableProperty] private int _margenIzquierdoMm;
    [ObservableProperty] private int _margenDerechoMm;
    [ObservableProperty] private int _margenSuperiorMm;
    [ObservableProperty] private int _margenInferiorMm;
    [ObservableProperty] private int _tamanoTextoNormal;
    [ObservableProperty] private int _tamanoTextoGrande;
    [ObservableProperty] private int _tamanoTextoTitulo;

    // --- Impresoras disponibles ---
    [ObservableProperty] private ObservableCollection<string> _impresorasDisponibles = new();

    // --- Empresas ---
    [ObservableProperty] private ObservableCollection<Empresa> _empresas = new();
    [ObservableProperty] private Empresa? _empresaSeleccionada;
    [ObservableProperty] private string _empresaNombre = string.Empty;
    [ObservableProperty] private string _empresaCalle = string.Empty;
    [ObservableProperty] private string _empresaNumero = string.Empty;
    [ObservableProperty] private string _empresaCiudad = string.Empty;
    [ObservableProperty] private string _empresaProvincia = string.Empty;

    partial void OnEmpresaSeleccionadaChanged(Empresa? value)
    {
        if (value != null)
        {
            EmpresaNombre = value.Nombre;
            EmpresaCalle = value.Calle;
            EmpresaNumero = value.Numero;
            EmpresaCiudad = value.Ciudad;
            EmpresaProvincia = value.Provincia;
        }
    }

    // --- Contador total de tickets ---
    [ObservableProperty] private int _contadorTickets;

    // --- Contrasena ---
    [ObservableProperty] private string _contrasenaActual = string.Empty;
    [ObservableProperty] private string _nuevaContrasena = string.Empty;
    [ObservableProperty] private string _confirmarNuevaContrasena = string.Empty;

    // --- Contrasena Z ---
    [ObservableProperty] private string _contrasenaZ = string.Empty;

    // --- Mensajes ---
    [ObservableProperty] private string _mensaje = string.Empty;
    [ObservableProperty] private bool _hayError;

    public AjustesViewModel(BaseDatosServicio dbServicio, Action onCerrar, Action onAbrirHistorico)
    {
        _dbServicio = dbServicio;
        _onCerrar = onCerrar;
        _onAbrirHistorico = onAbrirHistorico;
        CargarConfiguracion();
        CargarEmpresas();
        CargarImpresoras();
    }

    private void CargarConfiguracion()
    {
        var config = _dbServicio.ObtenerConfiguracion();
        PrecioPorPrenda = config.PrecioPorPrenda;
        NombreImpresora = config.NombreImpresora;
        AnchoTicketMm = config.AnchoTicketMm;
        AltoTicketMm = config.AltoTicketMm;
        MargenIzquierdoMm = config.MargenIzquierdoMm;
        MargenDerechoMm = config.MargenDerechoMm;
        MargenSuperiorMm = config.MargenSuperiorMm;
        MargenInferiorMm = config.MargenInferiorMm;
        TamanoTextoNormal = config.TamanoTextoNormal;
        TamanoTextoGrande = config.TamanoTextoGrande;
        TamanoTextoTitulo = config.TamanoTextoTitulo;
        ContrasenaZ = config.ContrasenaZ;
        TemaOscuro = config.TemaOscuro;
        ContadorTickets = config.ContadorTickets;
    }

    private void CargarEmpresas()
    {
        Empresas = new ObservableCollection<Empresa>(_dbServicio.ObtenerEmpresas());
    }

    private void CargarImpresoras()
    {
        ImpresorasDisponibles = new ObservableCollection<string>(ImpresionServicio.ObtenerImpresoras());
    }

    [RelayCommand]
    private void GuardarConfiguracion()
    {
        var config = _dbServicio.ObtenerConfiguracion();
        config.PrecioPorPrenda = PrecioPorPrenda;
        config.NombreImpresora = NombreImpresora;
        config.AnchoTicketMm = AnchoTicketMm;
        config.AltoTicketMm = AltoTicketMm;
        config.MargenIzquierdoMm = MargenIzquierdoMm;
        config.MargenDerechoMm = MargenDerechoMm;
        config.MargenSuperiorMm = MargenSuperiorMm;
        config.MargenInferiorMm = MargenInferiorMm;
        config.TamanoTextoNormal = TamanoTextoNormal;
        config.TamanoTextoGrande = TamanoTextoGrande;
        config.TamanoTextoTitulo = TamanoTextoTitulo;
        config.TemaOscuro = TemaOscuro;
        config.ContrasenaZ = ContrasenaZ;
        _dbServicio.GuardarConfiguracion(config);
        _onCerrar();
    }

    private void LimpiarFormularioEmpresa()
    {
        EmpresaSeleccionada = null;
        EmpresaNombre = string.Empty;
        EmpresaCalle = string.Empty;
        EmpresaNumero = string.Empty;
        EmpresaCiudad = string.Empty;
        EmpresaProvincia = string.Empty;
    }

    [RelayCommand]
    private void NuevaEmpresa()
    {
        LimpiarFormularioEmpresa();
    }

    [RelayCommand]
    private void GuardarEmpresa()
    {
        if (string.IsNullOrWhiteSpace(EmpresaNombre))
        {
            Mensaje = "Introduce un nombre de empresa";
            HayError = true;
            return;
        }

        if (EmpresaSeleccionada != null)
        {
            _dbServicio.EditarEmpresa(EmpresaSeleccionada.Id, EmpresaNombre.Trim(),
                EmpresaCalle.Trim(), EmpresaNumero.Trim(), EmpresaCiudad.Trim(), EmpresaProvincia.Trim());
            Mensaje = "Empresa actualizada";
        }
        else
        {
            _dbServicio.AgregarEmpresa(EmpresaNombre.Trim(),
                EmpresaCalle.Trim(), EmpresaNumero.Trim(), EmpresaCiudad.Trim(), EmpresaProvincia.Trim());
            Mensaje = "Empresa agregada";
        }

        LimpiarFormularioEmpresa();
        CargarEmpresas();
        HayError = false;
    }

    [RelayCommand]
    private void EliminarEmpresa()
    {
        if (EmpresaSeleccionada == null)
        {
            Mensaje = "Selecciona una empresa para eliminar";
            HayError = true;
            return;
        }
        _dbServicio.EliminarEmpresa(EmpresaSeleccionada.Id);
        LimpiarFormularioEmpresa();
        CargarEmpresas();
        Mensaje = "Empresa eliminada";
        HayError = false;
    }

    [RelayCommand]
    private void ReiniciarContadorTickets()
    {
        var respuesta = MessageBox.Show(
            "Vas a reiniciar el contador total de tickets a cero.\n\n" +
            "Esta accion es IRREVERSIBLE: el numero total de tickets emitidos volvera a empezar desde 1 " +
            "y no se podra recuperar el valor actual.\n\n" +
            "Ten en cuenta que esto NO afecta al numero de percha (ese se reinicia con el cierre Z).\n\n" +
            "Estas seguro de que quieres continuar?",
            "Reiniciar contador de tickets",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (respuesta != MessageBoxResult.Yes)
            return;

        _dbServicio.ReiniciarContadorTickets();
        ContadorTickets = 0;
        Mensaje = "Contador total de tickets reiniciado";
        HayError = false;
    }

    [RelayCommand]
    private void CambiarContrasena()
    {
        if (!_dbServicio.ValidarContrasenaMaestra(ContrasenaActual))
        {
            Mensaje = "La contrasena actual no es correcta";
            HayError = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(NuevaContrasena) || NuevaContrasena.Length < 4)
        {
            Mensaje = "La nueva contrasena debe tener al menos 4 caracteres";
            HayError = true;
            return;
        }
        if (NuevaContrasena != ConfirmarNuevaContrasena)
        {
            Mensaje = "Las contrasenas no coinciden";
            HayError = true;
            return;
        }
        _dbServicio.CambiarContrasenaMaestra(NuevaContrasena);
        ContrasenaActual = string.Empty;
        NuevaContrasena = string.Empty;
        ConfirmarNuevaContrasena = string.Empty;
        Mensaje = "Contrasena cambiada correctamente";
        HayError = false;
    }

    [RelayCommand]
    private void AbrirHistorico()
    {
        _onAbrirHistorico();
    }

    [RelayCommand]
    private void Cerrar()
    {
        _onCerrar();
    }
}
