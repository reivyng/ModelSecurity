using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las relaciones entre roles y usuarios.
    /// Implementa la lógica de negocio para la gestión de relaciones entre roles y usuarios, incluyendo operaciones CRUD.
    /// </summary>
    internal class UserRolBusiness
    {
        // Dependencias inyectadas
        private readonly UserRolData _userRolData; // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="userRolData">Servicio de acceso a datos para relaciones entre roles y usuarios</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public UserRolBusiness(UserRolData userRolData, ILogger logger)
        {
            _userRolData = userRolData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las relaciones entre roles y usuarios del sistema y las convierte a DTOs
        /// </summary>
        /// <returns>Lista de relaciones entre roles y usuarios en formato DTO</returns>
        public async Task<IEnumerable<UserRolDTO>> GetAllUserRolsAsync()
        {
            try
            {
                // Obtener relaciones entre roles y usuarios de la capa de datos
                var userRols = await _userRolData.GetAllAsync();
                var userRolsDTO = new List<UserRolDTO>();

                // Convertir cada relación a DTO
                foreach (var userRol in userRols)
                {
                    userRolsDTO.Add(new UserRolDTO
                    {
                        Id = userRol.Id,
                        UserId = userRol.UserId,
                        RolId = userRol.RolId
                    });
                }

                return userRolsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones entre roles y usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones entre roles y usuarios", ex);
            }
        }

        /// <summary>
        /// Obtiene una relación específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la relación</param>
        /// <returns>Relación en formato DTO</returns>
        public async Task<UserRolDTO> GetUserRolByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una relación con ID inválido: {UserRolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación debe ser mayor que cero");
            }

            try
            {
                // Buscar la relación en la base de datos
                var userRol = await _userRolData.GetByIdAsync(id);
                if (userRol == null)
                {
                    _logger.LogInformation("No se encontró ninguna relación con ID: {UserRolId}", id);
                    throw new EntityNotFoundException("UserRol", id);
                }

                // Convertir la relación a DTO
                return new UserRolDTO
                {
                    Id = userRol.Id,
                    UserId = userRol.UserId,
                    RolId = userRol.RolId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación con ID: {UserRolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva relación entre rol y usuario en el sistema
        /// </summary>
        /// <param name="userRolDto">Datos de la relación a crear</param>
        /// <returns>Relación creada en formato DTO</returns>
        public async Task<UserRolDTO> CreateUserRolAsync(UserRolDTO userRolDto)
        {
            
                // Validar los datos del DTO
                ValidateUserRol(userRolDto);

                // Crear la entidad UserRol desde el DTO
                var userRol = new UserRol
                {
                    UserId = userRolDto.UserId,
                    RolId = userRolDto.RolId
                };

                // Guardar la relación en la base de datos
                var userRolCreado = await _userRolData.CreateAsync(userRol);

                // Convertir la relación creada a DTO para la respuesta
                return new UserRolDTO
                {
                    Id = userRolCreado.Id,
                    UserId = userRolCreado.UserId,
                    RolId = userRolCreado.RolId
                };
            
            
        }

        /// <summary>
        /// Valida los datos del DTO de relación
        /// </summary>
        /// <param name="userRolDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateUserRol(UserRolDTO userRolDto)
        {
            // Validar que el DTO no sea nulo
            if (userRolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto relación no puede ser nulo");
            }

            // Validar que el UserId no esté vacío
            if (userRolDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId de la relación es obligatorio");
            }

            // Validar que el RolId no esté vacío
            if (userRolDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación con RolId inválido");
                throw new Utilities.Exceptions.ValidationException("RolId", "El RolId de la relación es obligatorio");
            }
        }
    }
}







