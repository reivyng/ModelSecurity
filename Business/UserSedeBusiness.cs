using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros de UserSede.
    /// Implementa la lógica de negocio para la gestión de registros de UserSede, incluyendo operaciones CRUD.
    /// </summary>
    internal class UserSedeBusiness
    {
        // Dependencias inyectadas
        private readonly UserSedeData _userSedeData; // Acceso a la capa de datos
        private readonly ILogger _logger;           // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="userSedeData">Servicio de acceso a datos para registros de UserSede</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public UserSedeBusiness(UserSedeData userSedeData, ILogger logger)
        {
            _userSedeData = userSedeData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de UserSede del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de registros de UserSede en formato DTO</returns>
        public async Task<IEnumerable<UserSedeDTO>> GetAllUserSedesAsync()
        {
            try
            {
                // Obtener registros de UserSede de la capa de datos
                var userSedes = await _userSedeData.GetAllAsync();
                var userSedesDTO = new List<UserSedeDTO>();

                // Convertir cada registro de UserSede a DTO
                foreach (var userSede in userSedes)
                {
                    userSedesDTO.Add(new UserSedeDTO
                    {
                        Id = userSede.Id,
                        status_procedure = userSede.status_procedure,
                        UserId = userSede.UserId,
                        SedeId = userSede.SedeId
                    });
                }

                return userSedesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de UserSede");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros de UserSede", ex);
            }
        }

        /// <summary>
        /// Obtiene un registro de UserSede específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del registro de UserSede</param>
        /// <returns>Registro de UserSede en formato DTO</returns>
        public async Task<UserSedeDTO> GetUserSedeByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de UserSede con ID inválido: {UserSedeId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de UserSede debe ser mayor que cero");
            }

            try
            {
                // Buscar el registro de UserSede en la base de datos
                var userSede = await _userSedeData.GetByIdAsync(id);
                if (userSede == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de UserSede con ID: {UserSedeId}", id);
                    throw new EntityNotFoundException("UserSede", id);
                }

                // Convertir el registro de UserSede a DTO
                return new UserSedeDTO
                {
                    Id = userSede.Id,
                    status_procedure = userSede.status_procedure,
                    UserId = userSede.UserId,
                    SedeId = userSede.SedeId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de UserSede con ID: {UserSedeId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro de UserSede con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo registro de UserSede en el sistema
        /// </summary>
        /// <param name="userSedeDto">Datos del registro de UserSede a crear</param>
        /// <returns>Registro de UserSede creado en formato DTO</returns>
        public async Task<UserSedeDTO> CreateUserSedeAsync(UserSedeDTO userSedeDto)
        {
            
                // Validar los datos del DTO
                ValidateUserSede(userSedeDto);

                // Crear la entidad UserSede desde el DTO
                var userSede = new UserSede
                {
                    status_procedure = userSedeDto.status_procedure,
                    UserId = userSedeDto.UserId,
                    SedeId = userSedeDto.SedeId
                };

                // Guardar el registro de UserSede en la base de datos
                var userSedeCreado = await _userSedeData.CreateAsync(userSede);

                // Convertir el registro de UserSede creado a DTO para la respuesta
                return new UserSedeDTO
                {
                    Id = userSedeCreado.Id,
                    status_procedure = userSedeCreado.status_procedure,
                    UserId = userSedeCreado.UserId,
                    SedeId = userSedeCreado.SedeId
                };
            
        }

        /// <summary>
        /// Valida los datos del DTO de registro de UserSede
        /// </summary>
        /// <param name="userSedeDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateUserSede(UserSedeDTO userSedeDto)
        {
            // Validar que el DTO no sea nulo
            if (userSedeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto registro de UserSede no puede ser nulo");
            }

            // Validar que el UserId no esté vacío
            if (userSedeDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro de UserSede con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId del registro de UserSede es obligatorio");
            }

            // Validar que el SedeId no esté vacío
            if (userSedeDto.SedeId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro de UserSede con SedeId inválido");
                throw new Utilities.Exceptions.ValidationException("SedeId", "El SedeId del registro de UserSede es obligatorio");
            }
        }
    }
}







