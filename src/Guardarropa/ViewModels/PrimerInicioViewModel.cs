using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guardarropa.Services;

namespace Guardarropa.ViewModels;

public partial class PrimerInicioViewModel : ObservableObject
{
    private readonly BaseDatosServicio _dbServicio;
    private readonly Action _onCompletado;
    private readonly bool _esRestablecimiento;

    [ObservableProperty]
    private string _contrasena = string.Empty;

    [ObservableProperty]
    private string _confirmarContrasena = string.Empty;

    [ObservableProperty]
    private string _error = string.Empty;

    public string Titulo => _esRestablecimiento ? "Restablecer contrasena" : "Configuracion inicial";

    public string Subtitulo => _esRestablecimiento
        ? "Se detecto una solicitud de restablecimiento (reset.txt). Establece una nueva contrasena maestra."
        : "Establece tu contrasena maestra";

    public string TextoBoton => _esRestablecimiento ? "GUARDAR NUEVA CONTRASENA" : "CREAR CONTRASENA";

    public PrimerInicioViewModel(BaseDatosServicio dbServicio, Action onCompletado, bool esRestablecimiento = false)
    {
        _dbServicio = dbServicio;
        _onCompletado = onCompletado;
        _esRestablecimiento = esRestablecimiento;
    }

    [RelayCommand]
    private void Guardar()
    {
        if (string.IsNullOrWhiteSpace(Contrasena))
        {
            Error = "La contrasena no puede estar vacia";
            return;
        }
        if (Contrasena.Length < 4)
        {
            Error = "La contrasena debe tener al menos 4 caracteres";
            return;
        }
        if (Contrasena != ConfirmarContrasena)
        {
            Error = "Las contrasenas no coinciden";
            return;
        }

        if (_esRestablecimiento)
        {
            _dbServicio.CambiarContrasenaMaestra(Contrasena);
            _dbServicio.EliminarArchivoReset();
        }
        else
        {
            _dbServicio.CrearConfiguracionInicial(Contrasena);
        }
        _onCompletado();
    }
}
