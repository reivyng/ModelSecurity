using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros de Sofia.
    /// Implementa la lógica de negocio para la gestión de registros de Sofia, incluyendo operaciones CRUD.
    /// </summary>
    public class RegisterySofiaBusiness
    {
        // Dependencias inyectadas
        private readonly RegisterySofiaData _registerySofiaData; // Acceso a la capa de datos
        private readonly ILogger _logger;                       // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="registerySofiaData">Servicio de acceso a datos para registros de Sofia</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public RegisterySofiaBusiness(RegisterySofiaData registerySofiaData, ILogger logger)
        {
            _registerySofiaData = registerySofiaData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de Sofia del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de registros de Sofia en formato DTO</returns>
        public async Task<IEnumerable<RegisterySofiaDTO>> GetAllRegisterySofiasAsync()
        {
            try
            {
                // Obtener registros de Sofia de la capa de datos
                var registerySofias = await _registerySofiaData.GetAllAsync();
                var registerySofiasDTO = new List<RegisterySofiaDTO>();

                // Convertir cada registro de Sofia a DTO
                foreach (var registerySofia in registerySofias)
                {
                    registerySofiasDTO.Add(new RegisterySofiaDTO
                    {
                        Id = registerySofia.Id,
                        Name = registerySofia.Name,
                        Description = registerySofia.Description,
                        Document = registerySofia.Document
                    });
                }

                return registerySofiasDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de Sofia");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros de Sofia", ex);
            }
        }

        /// <summary>
        /// Obtiene un registro de Sofia específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del registro de Sofia</param>
        /// <returns>Registro de Sofia en formato DTO</returns>
        public async Task<RegisterySofiaDTO> GetRegisterySofiaByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro de Sofia con ID inválido: {RegisterySofiaId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro de Sofia debe ser mayor que cero");
            }

            try
            {
                // Buscar el registro de Sofia en la base de datos
                var registerySofia = await _registerySofiaData.GetByIdAsync(id);
                if (registerySofia == null)
                {
                    _logger.LogInformation("No se encontró ningún registro de Sofia con ID: {RegisterySofiaId}", id);
                    throw new EntityNotFoundException("RegisterySofia", id);
                }

                // Convertir el registro de Sofia a DTO
                return new RegisterySofiaDTO
                {
                    Id = registerySofia.Id,
                    Name = registerySofia.Name,
                    Description = registerySofia.Description,
                    Document = registerySofia.Document
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de Sofia con ID: {RegisterySofiaId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro de Sofia con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo registro de Sofia en el sistema
        /// </summary>
        /// <param name="registerySofiaDto">Datos del registro de Sofia a crear</param>
        /// <returns>Registro de Sofia creado en formato DTO</returns>
        public async Task<RegisterySofiaDTO> CreateRegisterySofiaAsync(RegisterySofiaDTO registerySofiaDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateRegisterySofia(registerySofiaDto);

                // Crear la entidad RegisterySofia desde el DTO
                var registerySofia = new RegisterySofia
                {
                    Name = registerySofiaDto.Name,
                    Description = registerySofiaDto.Description,
                    Document = registerySofiaDto.Document,
                };

                // Guardar el registro de Sofia en la base de datos
                var registerySofiaCreado = await _registerySofiaData.CreateAsync(registerySofia);

                // Convertir el registro de Sofia creado a DTO para la respuesta
                return new RegisterySofiaDTO
                {
                    Id = registerySofia.Id,
                    Name = registerySofia.Name,
                    Description = registerySofia.Description,
                    Document = registerySofia.Document
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro de Sofia: {RegisterySofiaName}", registerySofiaDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro de Sofia", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de registro de Sofia
        /// </summary>
        /// <param name="registerySofiaDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateRegisterySofia(RegisterySofiaDTO registerySofiaDto)
        {
            // Validar que el DTO no sea nulo
            if (registerySofiaDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto registro de Sofia no puede ser nulo");
            }

            // Validar que el Name no esté vacío
            if (string.IsNullOrWhiteSpace(registerySofiaDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro de Sofia con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del registro de Sofia es obligatorio");
            }
        }
    }
}





