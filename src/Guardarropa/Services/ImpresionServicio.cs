using System.Drawing;
using System.Drawing.Printing;
using Guardarropa.Models;

namespace Guardarropa.Services;

public class ImpresionServicio
{
    public static string[] ObtenerImpresoras()
    {
        var impresoras = new string[PrinterSettings.InstalledPrinters.Count];
        PrinterSettings.InstalledPrinters.CopyTo(impresoras, 0);
        return impresoras;
    }

    public void ImprimirTicket(Ticket ticket, Configuracion config)
    {
        var doc = new PrintDocument();

        if (!string.IsNullOrEmpty(config.NombreImpresora))
            doc.PrinterSettings.PrinterName = config.NombreImpresora;

        doc.DefaultPageSettings.PaperSize = new PaperSize("Ticket",
            MmACentesimasPulgada(config.AnchoTicketMm),
            MmACentesimasPulgada(config.AltoTicketMm));

        doc.DefaultPageSettings.Margins = new Margins(
            MmACentesimasPulgada(config.MargenIzquierdoMm),
            MmACentesimasPulgada(config.MargenDerechoMm),
            MmACentesimasPulgada(config.MargenSuperiorMm),
            MmACentesimasPulgada(config.MargenInferiorMm));

        doc.PrintPage += (sender, e) => DibujarTicket(e, ticket, config);
        doc.Print();
    }

    public void ImprimirCierreZ(CierreZ cierre, Configuracion config)
    {
        var doc = new PrintDocument();

        if (!string.IsNullOrEmpty(config.NombreImpresora))
            doc.PrinterSettings.PrinterName = config.NombreImpresora;

        doc.DefaultPageSettings.PaperSize = new PaperSize("Ticket",
            MmACentesimasPulgada(config.AnchoTicketMm),
            MmACentesimasPulgada(config.AltoTicketMm));

        doc.DefaultPageSettings.Margins = new Margins(
            MmACentesimasPulgada(config.MargenIzquierdoMm),
            MmACentesimasPulgada(config.MargenDerechoMm),
            MmACentesimasPulgada(config.MargenSuperiorMm),
            MmACentesimasPulgada(config.MargenInferiorMm));

        doc.PrintPage += (sender, e) => DibujarCierreZ(e, cierre, config);
        doc.Print();
    }

    private void DibujarTicket(PrintPageEventArgs e, Ticket ticket, Configuracion config)
    {
        if (e.Graphics == null) return;

        var g = e.Graphics;
        var area = e.MarginBounds;
        var fuenteNormal = new Font("Arial", config.TamanoTextoNormal);
        var fuenteTitulo = new Font("Arial", config.TamanoTextoTitulo, FontStyle.Bold);
        var fuenteGrande = new Font("Arial", config.TamanoTextoGrande, FontStyle.Bold);
        var formatoCentrado = new StringFormat { Alignment = StringAlignment.Center };
        float y = area.Top;

        void DibujarCentrado(string texto, Font fuente)
        {
            g.DrawString(texto, fuente, Brushes.Black,
                new RectangleF(area.Left, y, area.Width, fuente.GetHeight(g)), formatoCentrado);
            y += fuente.GetHeight(g);
        }

        var empresa = ticket.Empresa;

        // Cabecera: nombre de la empresa y direccion
        DibujarCentrado(empresa?.Nombre ?? "Empresa", fuenteTitulo);
        y += 4;

        var lineaCalle = string.Join(" ", new[] { empresa?.Calle, empresa?.Numero }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
        if (!string.IsNullOrWhiteSpace(lineaCalle))
        {
            DibujarCentrado(lineaCalle, fuenteNormal);
            y += 2;
        }

        var lineaCiudad = string.Join(" ", new[] { empresa?.Ciudad, empresa?.Provincia }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
        if (!string.IsNullOrWhiteSpace(lineaCiudad))
        {
            DibujarCentrado(lineaCiudad, fuenteNormal);
            y += 2;
        }

        y += 4;
        DibujarCentrado(ticket.FechaHora.ToString("dd/MM/yyyy HH:mm"), fuenteNormal);
        y += 6;

        g.DrawLine(Pens.Black, area.Left, y, area.Right, y);
        y += 8;

        // Numero de percha (grande)
        DibujarCentrado("Nº. Percha", fuenteNormal);
        y += 2;
        DibujarCentrado(ticket.NumeroPercha.ToString(), fuenteGrande);
        y += 8;

        g.DrawLine(Pens.Black, area.Left, y, area.Right, y);
        y += 8;

        // Detalle
        g.DrawString($"Nº de prendas: {ticket.NumeroPrendas}", fuenteNormal, Brushes.Black, area.Left, y);
        y += fuenteNormal.GetHeight(g) + 4;

        g.DrawString($"Precio/prenda: {ticket.PrecioPorPrenda:C2}", fuenteNormal, Brushes.Black, area.Left, y);
        y += fuenteNormal.GetHeight(g) + 8;

        g.DrawLine(Pens.Black, area.Left, y, area.Right, y);
        y += 8;

        DibujarCentrado($"TOTAL: {ticket.PrecioTotal:C2}", fuenteTitulo);
        y += 12;

        // Numero de ticket (contador total)
        g.DrawString($"Ticket Nº {ticket.NumeroTicket}", fuenteNormal, Brushes.Black, area.Left, y);

        fuenteNormal.Dispose();
        fuenteTitulo.Dispose();
        fuenteGrande.Dispose();

        e.HasMorePages = false;
    }

    private void DibujarCierreZ(PrintPageEventArgs e, CierreZ cierre, Configuracion config)
    {
        if (e.Graphics == null) return;

        var g = e.Graphics;
        var area = e.MarginBounds;
        var fuenteNormal = new Font("Arial", config.TamanoTextoNormal);
        var fuenteTitulo = new Font("Arial", config.TamanoTextoTitulo, FontStyle.Bold);
        var fuenteGrande = new Font("Arial", config.TamanoTextoGrande, FontStyle.Bold);
        var formatoCentrado = new StringFormat { Alignment = StringAlignment.Center };
        float y = area.Top;

        g.DrawString("CIERRE Z", fuenteTitulo, Brushes.Black,
            new RectangleF(area.Left, y, area.Width, fuenteTitulo.GetHeight(g)), formatoCentrado);
        y += fuenteTitulo.GetHeight(g) + 4;

        var nombreEmpresa = cierre.Empresa?.Nombre ?? "Empresa";
        g.DrawString(nombreEmpresa, fuenteNormal, Brushes.Black,
            new RectangleF(area.Left, y, area.Width, fuenteNormal.GetHeight(g)), formatoCentrado);
        y += fuenteNormal.GetHeight(g) + 4;

        g.DrawString(cierre.Fecha.ToString("dd/MM/yyyy HH:mm"), fuenteNormal, Brushes.Black,
            new RectangleF(area.Left, y, area.Width, fuenteNormal.GetHeight(g)), formatoCentrado);
        y += fuenteNormal.GetHeight(g) + 8;

        g.DrawLine(Pens.Black, area.Left, y, area.Right, y);
        y += 8;

        g.DrawString($"Total tickets: {cierre.TotalTickets}", fuenteNormal, Brushes.Black, area.Left, y);
        y += fuenteNormal.GetHeight(g) + 4;

        g.DrawString($"Total prendas: {cierre.TotalPrendas}", fuenteNormal, Brushes.Black, area.Left, y);
        y += fuenteNormal.GetHeight(g) + 8;

        g.DrawLine(Pens.Black, area.Left, y, area.Right, y);
        y += 8;

        var textoTotal = $"TOTAL: {cierre.TotalRecaudado:C2}";
        g.DrawString(textoTotal, fuenteGrande, Brushes.Black,
            new RectangleF(area.Left, y, area.Width, fuenteGrande.GetHeight(g)), formatoCentrado);

        fuenteNormal.Dispose();
        fuenteTitulo.Dispose();
        fuenteGrande.Dispose();

        e.HasMorePages = false;
    }

    private static int MmACentesimasPulgada(int mm) => (int)(mm * 3.937);
}
