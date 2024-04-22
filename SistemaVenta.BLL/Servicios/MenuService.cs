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
    public class MenuService:IMenuService
    {
        private readonly IGenericRepository<Usuario> usuarioRepositorio;
        private readonly IGenericRepository<MenuRol> menuRolRepositorio;
        private readonly IGenericRepository<Menu> menuRepositorio;
        private readonly IMapper mapper;

        public MenuService(IGenericRepository<Usuario> usuarioRepositorio, IGenericRepository<MenuRol> menuRolRepositorio, IGenericRepository<Menu> menuRepositorio, IMapper mapper)
        {
            this.usuarioRepositorio = usuarioRepositorio;
            this.menuRolRepositorio = menuRolRepositorio;
            this.menuRepositorio = menuRepositorio;
            this.mapper = mapper;
        }

        public async Task<List<MenuDTO>> Lista(int idUsuario)
        {
            IQueryable<Usuario> tbUsuario = await this.usuarioRepositorio.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<MenuRol> tbMenuRol = await this.menuRolRepositorio.Consultar();
            IQueryable<Menu> tbMenu = await this.menuRepositorio.Consultar();
            try
            {
                IQueryable<Menu>tbResult = (from u in tbUsuario
                                            join mr in tbMenuRol on u.IdRol equals mr.IdRol
                                            join m in tbMenu on mr.IdMenu equals m.IdMenu
                                            select m).AsQueryable();
                var listaMenus = tbResult.ToList();
                return this.mapper.Map<List<MenuDTO>>(listaMenus);
            }
            catch
            {
                throw;
            }
        }
    }
}
