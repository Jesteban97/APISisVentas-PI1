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
    public class CategoriaService:ICategoriaService
    {
        private readonly IGenericRepository<Categoria> categoriaRepositorio;
        private readonly IMapper mapper;

        public CategoriaService(IGenericRepository<Categoria> categoriaRepositorio, IMapper mapper)
        {
            this.categoriaRepositorio = categoriaRepositorio;
            this.mapper = mapper;
        }

        public async Task<List<CategoriaDTO>> Lista()
        {
            try
            {
                var listaCategorias = await this.categoriaRepositorio.Consultar();
                return this.mapper.Map<List<CategoriaDTO>>(listaCategorias.ToList());
            }
            catch
            {
                throw;
            }
        }
    }
}
