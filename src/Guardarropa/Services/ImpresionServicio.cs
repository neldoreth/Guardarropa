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

        var nombreEmpresa = ticket.Empresa?.Nombre ?? "Empresa";
        g.DrawString(nombreEmpresa, fuenteTitulo, Brushes.Black,
            new RectangleF(area.Left, y, area.Width, fuenteTitulo.GetHeight(g)), formatoCentrado);
        y += fuenteTitulo.GetHeight(g) + 8;

        g.DrawString(ticket.FechaHora.ToString("dd/MM/yyyy HH:mm"), fuenteNormal, Brushes.Black,
            new RectangleF(area.Left, y, area.Width, fuenteNormal.GetHeight(g)), formatoCentrado);
        y += fuenteNormal.GetHeight(g) + 4;

        g.DrawLine(Pens.Black, area.Left, y, area.Right, y);
        y += 8;

        var textoNumero = $"#{ticket.NumeroTicket}";
        g.DrawString(textoNumero, fuenteGrande, Brushes.Black,
            new RectangleF(area.Left, y, area.Width, fuenteGrande.GetHeight(g)), formatoCentrado);
        y += fuenteGrande.GetHeight(g) + 8;

        g.DrawLine(Pens.Black, area.Left, y, area.Right, y);
        y += 8;

        g.DrawString($"Prendas: {ticket.NumeroPrendas}", fuenteNormal, Brushes.Black, area.Left, y);
        y += fuenteNormal.GetHeight(g) + 4;

        g.DrawString($"Precio/prenda: {ticket.PrecioPorPrenda:C2}", fuenteNormal, Brushes.Black, area.Left, y);
        y += fuenteNormal.GetHeight(g) + 8;

        g.DrawLine(Pens.Black, area.Left, y, area.Right, y);
        y += 8;

        var textoTotal = $"TOTAL: {ticket.PrecioTotal:C2}";
        g.DrawString(textoTotal, fuenteTitulo, Brushes.Black,
            new RectangleF(area.Left, y, area.Width, fuenteTitulo.GetHeight(g)), formatoCentrado);

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
