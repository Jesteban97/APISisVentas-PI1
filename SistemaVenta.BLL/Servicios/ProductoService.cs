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

namespace SistemaVenta.BLL.Servicios
{
    public class ProductoService:IProductoService
    {
        private readonly IGenericRepository<Producto> productoRepositorio;
        private readonly IMapper mapper;

        public ProductoService(IGenericRepository<Producto> productoRepositorio, IMapper mapper)
        {
            this.productoRepositorio = productoRepositorio;
            this.mapper = mapper;
        }

        public async Task<List<ProductoDTO>> lista()
        {
            try {
                var queryProducto = await this.productoRepositorio.Consultar();
                var listaProductos = queryProducto.Include(cat=>cat.IdCategoriaNavigation).ToList();

                return this.mapper.Map<List<ProductoDTO>>(listaProductos.ToList());
            } catch
            {
                throw;
            }
        }
        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                var productoCreado = await this.productoRepositorio.Crear(this.mapper.Map<Producto>(modelo));
                if (productoCreado.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el producto");
                }


                return this.mapper.Map<ProductoDTO>(productoCreado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(ProductoDTO modelo)
        {
            try
            {
                var productoModelo = this.mapper.Map<Producto>(modelo);
                var productoEncontrado = await this.productoRepositorio.Obtener(p => p.IdProducto == productoModelo.IdProducto);

                if (productoEncontrado == null)
                {
                    throw new TaskCanceledException("producto no encontrado");
                }

                productoEncontrado.Nombre = productoModelo.Nombre;
                productoEncontrado.Precio = productoModelo.Precio;
                productoEncontrado.IdCategoria = productoModelo.IdCategoria;
                productoEncontrado.Stock = productoModelo.Stock;
                productoEncontrado.EsActivo = productoModelo.EsActivo;

                bool resp = await this.productoRepositorio.Editar(productoEncontrado);

                if (!resp)
                {
                    throw new TaskCanceledException("No se pudo editar");
                }
                return resp;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var productoEncontrado = await this.productoRepositorio.Obtener(u => u.IdProducto == id);

                if (productoEncontrado == null)
                {
                    throw new TaskCanceledException("producto no encontrado");
                }
                bool resp = await this.productoRepositorio.Eliminar(productoEncontrado);

                if (!resp)
                {
                    throw new TaskCanceledException("No se pudo eliminar");
                }
                return resp;
            }
            catch
            {
                throw;
            }
        }

       
    }
}
