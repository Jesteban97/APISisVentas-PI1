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
    public class UsuarioService: IUsuarioService
    {
        private readonly IGenericRepository<Usuario> usuarioRepositorio;
        private readonly IMapper mapper;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepositorio, IMapper mapper)
        {
            this.usuarioRepositorio = usuarioRepositorio;
            this.mapper = mapper;
        }
        public async Task<List<UsuarioDTO>> lista()
        {
            try
            {
                var queryUsuario = await this.usuarioRepositorio.Consultar();
                var listasUsuarios = queryUsuario.Include(rol => rol.IdRolNavigation).ToList();

                return this.mapper.Map<List<UsuarioDTO>>(listasUsuarios);
            }
            catch
            {
                throw;
            }
        }

        public async Task<SesionDTO> validarCredenciales(string correo, string clave)
        {
            try
            {
                var queryUsuario = await this.usuarioRepositorio.Consultar(u =>
                u.Correo == correo &&
                u.Clave == clave);

                if(queryUsuario.FirstOrDefault() == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                Usuario devolverUsuario = queryUsuario.Include(rol => rol.IdRolNavigation).First();

                return this.mapper.Map<SesionDTO>(devolverUsuario);
            }
            catch
            {
                throw;
            }
        }
        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                var usuarioCreado = await this.usuarioRepositorio.Crear(this.mapper.Map<Usuario>(modelo));
                if(usuarioCreado.IdUsuario == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el usuario");
                }
                var query = await this.usuarioRepositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);

                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();

                return this.mapper.Map<UsuarioDTO>(usuarioCreado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            try
            {
                var usuarioModelo = this.mapper.Map<Usuario>(modelo);
                var usuarioEncontrado = await this.usuarioRepositorio.Obtener(u=>u.IdUsuario == usuarioModelo.IdUsuario);

                if (usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("usuario no encontrado");
                }

                usuarioEncontrado.NombreCompleto = usuarioModelo.NombreCompleto;
                usuarioEncontrado.Correo = usuarioModelo.Correo;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.Clave = usuarioModelo.Clave;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;

                bool resp = await this.usuarioRepositorio.Editar(usuarioEncontrado);

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
                var usuarioEncontrado = await this.usuarioRepositorio.Obtener(u => u.IdUsuario == id);

                if (usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("usuario no encontrado");
                }
                bool resp = await this.usuarioRepositorio.Eliminar(usuarioEncontrado);

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
