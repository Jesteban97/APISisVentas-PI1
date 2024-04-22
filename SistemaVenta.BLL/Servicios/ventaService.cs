using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model.Models;
using SistemaVenta.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SistemaVenta.BLL.Servicios
{
    public class ventaService:IVentaService
    {
        private readonly IVentaRepository ventaRepositorio;
        private readonly IGenericRepository<DetalleVenta> detalleVentaRepositorio;
        private readonly IMapper mapper;

        public ventaService(IVentaRepository ventaRepositorio, IGenericRepository<DetalleVenta> detalleVentaRepositorio, IMapper mapper)
        {
            this.ventaRepositorio = ventaRepositorio;
            this.detalleVentaRepositorio = detalleVentaRepositorio;
            this.mapper = mapper;
        }

        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await this.ventaRepositorio.Registrar(this.mapper.Map<Venta>(modelo));

                if(ventaGenerada.IdVenta == 0)
                {
                    throw new TaskCanceledException("No se pudo Crear");
                }
                 return this.mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInici, string fechaFin)
        {
            IQueryable<Venta> query = await this.ventaRepositorio.Consultar();
            var ListaResultado = new List<Venta>();
            try
            {
                if (buscarPor == "fecha")
                {
                    DateTime fechaInicio = DateTime.ParseExact(fechaInici, "dd/MM/yyyy", new CultureInfo("es-CO"));
                    DateTime fechaFinal = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                    ListaResultado = await query.Where(v =>
                    v.FechaRegistro.Value.Date >= fechaInicio.Date &&
                    v.FechaRegistro.Value.Date <= fechaFinal.Date)
                        .Include(dv => dv.DetalleVenta)
                        .ThenInclude(p => p.IdProductoNavigation).ToListAsync();
                }
                else
                {

                    ListaResultado = await query.Where(v => v.NumeroDocumento == numeroVenta)
                        .Include(dv => dv.DetalleVenta)
                        .ThenInclude(p => p.IdProductoNavigation).ToListAsync();
                }
            }
            catch
            {
                throw;
            }

            return this.mapper.Map<List<VentaDTO>>(ListaResultado);
        }


        public async Task<List<ReporteDTO>> reporte(string fechaInici, string fechaFin)
        {
            IQueryable<DetalleVenta> query = await this.detalleVentaRepositorio.Consultar();
            var ListaResultado = new List<DetalleVenta>();
            try
            {
                DateTime fechaInicio = DateTime.ParseExact(fechaInici, "dd/MM/yyyy", new CultureInfo("es-CO"));
                DateTime fechaFinal = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                ListaResultado = await query
                    .Include(p=>p.IdProductoNavigation)
                    .Include(v=>v.IdVentaNavigation)
                    .Where(dv =>
                    dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date && 
                    dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaFinal.Date
                    ).ToListAsync();
            }
            catch
            {
                throw;
            }
            return this.mapper.Map<List<ReporteDTO>>(ListaResultado);
        }
    }
}
