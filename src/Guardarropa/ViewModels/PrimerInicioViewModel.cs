using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guardarropa.Services;

namespace Guardarropa.ViewModels;

public partial class PrimerInicioViewModel : ObservableObject
{
    private readonly BaseDatosServicio _dbServicio;
    private readonly Action _onCompletado;

    [ObservableProperty]
    private string _contrasena = string.Empty;

    [ObservableProperty]
    private string _confirmarContrasena = string.Empty;

    [ObservableProperty]
    private string _error = string.Empty;

    public PrimerInicioViewModel(BaseDatosServicio dbServicio, Action onCompletado)
    {
        _dbServicio = dbServicio;
        _onCompletado = onCompletado;
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

        _dbServicio.CrearConfiguracionInicial(Contrasena);
        _onCompletado();
    }
}
