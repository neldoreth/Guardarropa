namespace Guardarropa.Models;

public class CierreZ
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int TotalTickets { get; set; }
    public int TotalPrendas { get; set; }
    public decimal TotalRecaudado { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public List<Ticket> Tickets { get; set; } = new();
}
