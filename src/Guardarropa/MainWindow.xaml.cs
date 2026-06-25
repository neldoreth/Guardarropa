using System.Windows;
using Guardarropa.Models;
using Guardarropa.Services;
using Guardarropa.ViewModels;
using Guardarropa.Views;

namespace Guardarropa;

public partial class MainWindow : Window
{
    private readonly BaseDatosServicio _dbServicio = new();
    private readonly ImpresionServicio _impresionServicio = new();
    private Empresa? _empresaActual;
    private PrincipalViewModel? _principalVm;

    public MainWindow()
    {
        InitializeComponent();
        _dbServicio.InicializarBaseDatos();
        Iniciar();
    }

    private void Iniciar()
    {
        if (_dbServicio.EsPrimerInicio())
            MostrarPrimerInicio();
        else
            MostrarSeleccionEmpresa();
    }

    private void MostrarPrimerInicio()
    {
        var vm = new PrimerInicioViewModel(_dbServicio, () => MostrarSeleccionEmpresa());
        var view = new PrimerInicioView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }

    private void MostrarSeleccionEmpresa()
    {
        var vm = new SeleccionEmpresaViewModel(
            _dbServicio,
            empresa =>
            {
                _empresaActual = empresa;
                MostrarPantallaPrincipal();
            },
            () => PedirContrasenaYAbrir(() => MostrarAjustes())
        );
        var view = new SeleccionEmpresaView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }

    private void MostrarPantallaPrincipal()
    {
        if (_empresaActual == null) return;

        _principalVm = new PrincipalViewModel(
            _dbServicio,
            _impresionServicio,
            _empresaActual,
            () => MostrarSeleccionEmpresa(),
            () => PedirContrasenaYAbrir(() => MostrarAjustes()),
            () => MostrarCierreZ()
        );
        var view = new PrincipalView { DataContext = _principalVm };
        ContenidoPrincipal.Content = view;
    }

    private void MostrarAjustes()
    {
        var vm = new AjustesViewModel(
            _dbServicio,
            () =>
            {
                if (_empresaActual != null)
                    MostrarPantallaPrincipal();
                else
                    MostrarSeleccionEmpresa();
            },
            () => MostrarHistoricoZ()
        );
        var view = new AjustesView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }

    private void MostrarCierreZ()
    {
        if (_empresaActual == null) return;

        var vm = new CierreZViewModel(
            _dbServicio,
            _impresionServicio,
            _empresaActual,
            () => MostrarPantallaPrincipal(),
            () => _principalVm?.Refrescar()
        );
        var view = new CierreZView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }

    private void MostrarHistoricoZ()
    {
        var vm = new HistoricoZViewModel(
            _dbServicio,
            () => MostrarAjustes()
        );
        var view = new HistoricoZView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }

    private void PedirContrasenaYAbrir(Action onAutenticado)
    {
        var dialog = new ContrasenaDialog(_dbServicio) { Owner = this };
        if (dialog.ShowDialog() == true && dialog.Autenticado)
        {
            onAutenticado();
        }
    }
}
