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

namespace SistemaVenta.BLL.Servicios
{
    public class RolService:IRolService
    {
        private readonly IGenericRepository<Rol> rolRepositorio;
        private readonly IMapper mapper;

        public RolService(IGenericRepository<Rol> rolRepositorio, IMapper mapper)
        {
            this.rolRepositorio = rolRepositorio;
            this.mapper = mapper;
        }

        public async Task<List<RolDTO>> Lista()
        {
            try
            {
                var listaRoles = await this.rolRepositorio.Consultar();
                return this.mapper.Map<List<RolDTO>>(listaRoles.ToList());
            }
            catch
            {
                throw;
            }
        }
    }
}
