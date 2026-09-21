using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Guardarropa.Models;

namespace Guardarropa.Services;

public class DiarioServicio
{
    private const string PrefijoTotal = "TOTAL";

    private static readonly Regex RegexLineaTicket = new(
        @"^(?<hora>\d{2}:\d{2}:\d{2})\s+Percha nº (?<percha>\d+)\s+Prendas: (?<prendas>\d+)\s+Total: (?<total>[\d,]+) €$",
        RegexOptions.Compiled);

    public void RegistrarTicket(Ticket ticket)
    {
        var carpeta = ObtenerCarpetaDiario();
        var ruta = Path.Combine(carpeta, ObtenerNombreArchivo(ticket));

        var lineas = File.Exists(ruta)
            ? File.ReadAllLines(ruta, Encoding.UTF8).Where(l => !l.StartsWith(PrefijoTotal)).ToList()
            : new List<string>();

        lineas.Add(FormatearLineaTicket(ticket));

        var totalPerchas = 0;
        var totalPrendas = 0;
        var totalEuros = 0m;
        foreach (var linea in lineas)
        {
            var match = RegexLineaTicket.Match(linea);
            if (!match.Success) continue;

            totalPerchas++;
            totalPrendas += int.Parse(match.Groups["prendas"].Value, CultureInfo.InvariantCulture);
            totalEuros += decimal.Parse(
                match.Groups["total"].Value.Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture);
        }

        lineas.Add(FormatearLineaTotal(totalPerchas, totalPrendas, totalEuros));

        File.WriteAllLines(ruta, lineas, new UTF8Encoding(true));
    }

    public static string ObtenerCarpetaDiario()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var carpeta = Path.Combine(appData, "Guardarropa", "diario");
        Directory.CreateDirectory(carpeta);
        return carpeta;
    }

    private static string ObtenerNombreArchivo(Ticket ticket)
    {
        var nombreEmpresa = ticket.Empresa?.Nombre ?? string.Empty;
        var prefijo = SanearNombreArchivo(nombreEmpresa[..Math.Min(5, nombreEmpresa.Length)]);
        var fecha = ticket.FechaHora.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return $"{prefijo}{fecha}.txt";
    }

    private static string SanearNombreArchivo(string nombre)
    {
        var caracteresInvalidos = Path.GetInvalidFileNameChars();
        var limpio = new string(nombre.Where(c => !caracteresInvalidos.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(limpio) ? "EMPR" : limpio;
    }

    private static string FormatearLineaTicket(Ticket ticket) =>
        $"{ticket.FechaHora:HH:mm:ss}   Percha nº {ticket.NumeroPercha}   Prendas: {ticket.NumeroPrendas}   Total: {FormatearEuros(ticket.PrecioTotal)} €";

    private static string FormatearLineaTotal(int perchas, int prendas, decimal euros) =>
        $"{PrefijoTotal}   Perchas: {perchas}   Prendas: {prendas}   Total: {FormatearEuros(euros)} €";

    private static string FormatearEuros(decimal valor) =>
        valor.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',');
}
