namespace Guardarropa;

public record VersionInfo(string Version, string Fecha, string[] Cambios);

public static class AppInfo
{
    public const string NombrePrograma = "Guardarropa";
    public const string Desarrollador = "ClickEZ Solutions";
    public const string Version = "0.1.1";

    public static readonly IReadOnlyList<VersionInfo> Historial = new List<VersionInfo>
    {
        new("0.1.1", "21/09/2026", new[]
        {
            "Nueva ventana \"Acerca de\" con el numero de version y el historial de cambios",
            "Diario de tickets: registro automatico en un archivo de texto diario por empresa",
            "Restablecimiento de la contrasena maestra mediante archivo reset.txt",
            "Icono propio para la aplicacion",
        }),
        new("0.1.0", "26/06/2026", new[]
        {
            "Direcciones completas en empresas (calle, numero, ciudad, provincia)",
            "Contador total de tickets, con reinicio irreversible desde Ajustes",
            "Ajustes reorganizado en 4 pestanas: General, Contrasena, Cierre Z e Impresora",
            "La tecla Intro imprime la percha directamente",
        }),
        new("0.0.2", "25/06/2026", new[]
        {
            "Mejoras generales de interfaz y experiencia de usuario",
        }),
        new("0.0.1", "25/06/2026", new[]
        {
            "Implementacion inicial completa de la aplicacion Guardarropa",
        }),
    };
}
