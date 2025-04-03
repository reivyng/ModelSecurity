using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles del sistema.
    /// Implementa la lógica de negocio para la gestión de roles, incluyendo operaciones CRUD.
    /// </summary>
    public class RolBusiness
    {
        // Dependencias inyectadas
        private readonly RolData _rolData;        // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="rolData">Servicio de acceso a datos para roles</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public RolBusiness(RolData rolData, ILogger logger)
        {
            _rolData = rolData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los roles del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de roles en formato DTO</returns>
        public async Task<IEnumerable<RolDTO>> GetAllRolesAsync()
        {
            try
            {
                // Obtener roles de la capa de datos
                var roles = await _rolData.GetAllAsync();
                var rolesDTO = new List<RolDTO>();

                // Convertir cada rol a DTO
                foreach (var rol in roles)
                {
                    rolesDTO.Add(new RolDTO
                    {
                        Id = rol.Id,
                        TypeRol = rol.Type_Rol,
                        Description = rol.Description,
                        Active = rol.Active
                    });
                }

                return rolesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        /// <summary>
        /// Obtiene un rol específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del rol</param>
        /// <returns>Rol en formato DTO</returns>
        public async Task<RolDTO> GetRolByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                // Buscar el rol en la base de datos
                var rol = await _rolData.GetByidAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                // Convertir el rol a DTO
                return new RolDTO
                {
                    Id = rol.Id,
                    TypeRol = rol.Type_Rol,
                    Description = rol.Description,
                    Active = rol.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo rol en el sistema
        /// </summary>
        /// <param name="rolDto">Datos del rol a crear</param>
        /// <returns>Rol creado en formato DTO</returns>
        public async Task<RolDTO> CreateRolAsync(RolDTO rolDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateRol(rolDto);

                // Crear la entidad Rol desde el DTO
                var rol = new Rol
                {
                    Type_Rol = rolDto.TypeRol,
                    Description = rolDto.Description,
                    Active = rolDto.Active
                };

                // Guardar el rol en la base de datos
                var rolCreado = await _rolData.CreateAsync(rol);

                // Convertir el rol creado a DTO para la respuesta
                return new RolDTO
                {
                    Id = rol.Id,
                    TypeRol = rol.Type_Rol,
                    Description = rol.Description,
                    Active = rol.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", rolDto?.TypeRol ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de rol
        /// </summary>
        /// <param name="rolDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateRol(RolDTO rolDto)
        {
            // Validar que el DTO no sea nulo
            if (rolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            // Validar que el TypeRol no esté vacío
            if (string.IsNullOrWhiteSpace(rolDto.TypeRol))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con TypeRol vacío");
                throw new Utilities.Exceptions.ValidationException("TypeRol", "El TypeRol del rol es obligatorio");
            }
        }
    }
}