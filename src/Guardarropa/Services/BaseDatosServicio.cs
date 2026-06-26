using Microsoft.EntityFrameworkCore;
using Guardarropa.Data;
using Guardarropa.Models;
using System.Security.Cryptography;
using System.Text;

namespace Guardarropa.Services;

public class BaseDatosServicio
{
    public void InicializarBaseDatos()
    {
        using var db = new GuardarropaDbContext();
        db.Database.EnsureCreated();
    }

    public bool EsPrimerInicio()
    {
        using var db = new GuardarropaDbContext();
        return !db.Configuraciones.Any();
    }

    public void CrearConfiguracionInicial(string contrasenaMaestra)
    {
        using var db = new GuardarropaDbContext();
        var config = new Configuracion
        {
            ContrasenaMaestra = HashContrasena(contrasenaMaestra),
            PrecioPorPrenda = 1.00m
        };
        db.Configuraciones.Add(config);
        db.SaveChanges();
    }

    public bool ValidarContrasenaMaestra(string contrasena)
    {
        using var db = new GuardarropaDbContext();
        var config = db.Configuraciones.FirstOrDefault();
        if (config == null) return false;
        return config.ContrasenaMaestra == HashContrasena(contrasena);
    }

    public bool ValidarContrasenaZ(string contrasena)
    {
        using var db = new GuardarropaDbContext();
        var config = db.Configuraciones.FirstOrDefault();
        if (config == null) return false;
        return config.ContrasenaZ == contrasena;
    }

    public Configuracion ObtenerConfiguracion()
    {
        using var db = new GuardarropaDbContext();
        return db.Configuraciones.FirstOrDefault() ?? new Configuracion();
    }

    public void GuardarConfiguracion(Configuracion config)
    {
        using var db = new GuardarropaDbContext();
        var existente = db.Configuraciones.FirstOrDefault();
        if (existente != null)
        {
            existente.PrecioPorPrenda = config.PrecioPorPrenda;
            existente.NombreImpresora = config.NombreImpresora;
            existente.AnchoTicketMm = config.AnchoTicketMm;
            existente.AltoTicketMm = config.AltoTicketMm;
            existente.MargenIzquierdoMm = config.MargenIzquierdoMm;
            existente.MargenDerechoMm = config.MargenDerechoMm;
            existente.MargenSuperiorMm = config.MargenSuperiorMm;
            existente.MargenInferiorMm = config.MargenInferiorMm;
            existente.TamanoTextoNormal = config.TamanoTextoNormal;
            existente.TamanoTextoGrande = config.TamanoTextoGrande;
            existente.TamanoTextoTitulo = config.TamanoTextoTitulo;
            existente.TemaOscuro = config.TemaOscuro;
            existente.ContrasenaZ = config.ContrasenaZ;
        }
        db.SaveChanges();
    }

    public void CambiarContrasenaMaestra(string nuevaContrasena)
    {
        using var db = new GuardarropaDbContext();
        var config = db.Configuraciones.FirstOrDefault();
        if (config != null)
        {
            config.ContrasenaMaestra = HashContrasena(nuevaContrasena);
            db.SaveChanges();
        }
    }

    public string GenerarContrasenaZ()
    {
        using var db = new GuardarropaDbContext();
        var config = db.Configuraciones.FirstOrDefault();
        if (config == null) return string.Empty;

        var contrasena = GenerarCodigoAleatorio(6);
        config.ContrasenaZ = contrasena;
        db.SaveChanges();
        return contrasena;
    }

    // --- Empresas ---

    public List<Empresa> ObtenerEmpresas()
    {
        using var db = new GuardarropaDbContext();
        return db.Empresas.Where(e => e.Activa).OrderBy(e => e.Nombre).ToList();
    }

    public void AgregarEmpresa(string nombre, string calle, string numero, string ciudad, string provincia)
    {
        using var db = new GuardarropaDbContext();
        db.Empresas.Add(new Empresa
        {
            Nombre = nombre,
            Calle = calle,
            Numero = numero,
            Ciudad = ciudad,
            Provincia = provincia
        });
        db.SaveChanges();
    }

    public void EditarEmpresa(int id, string nombre, string calle, string numero, string ciudad, string provincia)
    {
        using var db = new GuardarropaDbContext();
        var empresa = db.Empresas.Find(id);
        if (empresa != null)
        {
            empresa.Nombre = nombre;
            empresa.Calle = calle;
            empresa.Numero = numero;
            empresa.Ciudad = ciudad;
            empresa.Provincia = provincia;
            db.SaveChanges();
        }
    }

    public void EliminarEmpresa(int id)
    {
        using var db = new GuardarropaDbContext();
        var empresa = db.Empresas.Find(id);
        if (empresa != null)
        {
            empresa.Activa = false;
            db.SaveChanges();
        }
    }

    // --- Tickets ---

    public int ObtenerSiguienteNumeroPercha(int empresaId)
    {
        using var db = new GuardarropaDbContext();
        var ultimoTicketSinCierre = db.Tickets
            .Where(t => t.EmpresaId == empresaId && t.CierreZId == null)
            .OrderByDescending(t => t.NumeroPercha)
            .FirstOrDefault();
        return (ultimoTicketSinCierre?.NumeroPercha ?? 0) + 1;
    }

    public int ObtenerContadorTickets()
    {
        using var db = new GuardarropaDbContext();
        return db.Configuraciones.FirstOrDefault()?.ContadorTickets ?? 0;
    }

    public void ReiniciarContadorTickets()
    {
        using var db = new GuardarropaDbContext();
        var config = db.Configuraciones.FirstOrDefault();
        if (config != null)
        {
            config.ContadorTickets = 0;
            db.SaveChanges();
        }
    }

    public Ticket CrearTicket(int empresaId, int numeroPrendas, decimal precioPorPrenda, int numeroPercha)
    {
        using var db = new GuardarropaDbContext();

        var config = db.Configuraciones.FirstOrDefault();
        var numeroTicketTotal = (config?.ContadorTickets ?? 0) + 1;
        if (config != null)
            config.ContadorTickets = numeroTicketTotal;

        var ticket = new Ticket
        {
            NumeroPercha = numeroPercha,
            NumeroTicket = numeroTicketTotal,
            NumeroPrendas = numeroPrendas,
            PrecioPorPrenda = precioPorPrenda,
            PrecioTotal = numeroPrendas * precioPorPrenda,
            FechaHora = DateTime.Now,
            EmpresaId = empresaId
        };
        db.Tickets.Add(ticket);
        db.SaveChanges();

        ticket.Empresa = db.Empresas.Find(empresaId);
        return ticket;
    }

    // --- Cierre Z ---

    public CierreZ RealizarCierreZ(int empresaId)
    {
        using var db = new GuardarropaDbContext();
        var ticketsSinCierre = db.Tickets
            .Where(t => t.EmpresaId == empresaId && t.CierreZId == null)
            .ToList();

        var cierre = new CierreZ
        {
            Fecha = DateTime.Now,
            TotalTickets = ticketsSinCierre.Count,
            TotalPrendas = ticketsSinCierre.Sum(t => t.NumeroPrendas),
            TotalRecaudado = ticketsSinCierre.Sum(t => t.PrecioTotal),
            EmpresaId = empresaId
        };
        db.CierresZ.Add(cierre);
        db.SaveChanges();

        foreach (var ticket in ticketsSinCierre)
        {
            ticket.CierreZId = cierre.Id;
        }
        db.SaveChanges();

        cierre.Empresa = db.Empresas.Find(empresaId);
        return cierre;
    }

    public List<CierreZ> ObtenerHistoricoCierresZ(int? empresaId = null)
    {
        using var db = new GuardarropaDbContext();
        var query = db.CierresZ.Include(z => z.Empresa).AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(z => z.EmpresaId == empresaId.Value);
        return query.OrderByDescending(z => z.Fecha).ToList();
    }

    // --- Utilidades ---

    private static string HashContrasena(string contrasena)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(contrasena));
        return Convert.ToBase64String(bytes);
    }

    private static string GenerarCodigoAleatorio(int longitud)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = RandomNumberGenerator.Create();
        var buffer = new byte[longitud];
        random.GetBytes(buffer);
        return new string(buffer.Select(b => chars[b % chars.Length]).ToArray());
    }
}
