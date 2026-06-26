namespace Guardarropa.Models;

public class Ticket
{
    public int Id { get; set; }
    public int NumeroPercha { get; set; }
    public int NumeroTicket { get; set; }
    public int NumeroPrendas { get; set; }
    public decimal PrecioPorPrenda { get; set; }
    public decimal PrecioTotal { get; set; }
    public DateTime FechaHora { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public int? CierreZId { get; set; }
    public CierreZ? CierreZ { get; set; }
}
