namespace Guardarropa.Models;

public class Configuracion
{
    public int Id { get; set; }
    public string ContrasenaMaestra { get; set; } = string.Empty;
    public decimal PrecioPorPrenda { get; set; } = 1.00m;
    public string NombreImpresora { get; set; } = string.Empty;
    public int AnchoTicketMm { get; set; } = 80;
    public int AltoTicketMm { get; set; } = 200;
    public int MargenIzquierdoMm { get; set; } = 5;
    public int MargenDerechoMm { get; set; } = 5;
    public int MargenSuperiorMm { get; set; } = 5;
    public int MargenInferiorMm { get; set; } = 5;
    public int TamanoTextoNormal { get; set; } = 10;
    public int TamanoTextoGrande { get; set; } = 24;
    public int TamanoTextoTitulo { get; set; } = 14;
    public string ContrasenaZ { get; set; } = string.Empty;
    public bool TemaOscuro { get; set; } = true;
    public int ContadorTickets { get; set; } = 0;
}
