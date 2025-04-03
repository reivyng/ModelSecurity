using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las modalidades.
    /// Implementa la lógica de negocio para la gestión de modalidades, incluyendo operaciones CRUD.
    /// </summary>
    internal class TypeModalityBusiness
    {
        // Dependencias inyectadas
        private readonly TypeModalityData _typeModalityData; // Acceso a la capa de datos
        private readonly ILogger _logger;                   // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="typeModalityData">Servicio de acceso a datos para modalidades</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public TypeModalityBusiness(TypeModalityData typeModalityData, ILogger logger)
        {
            _typeModalityData = typeModalityData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las modalidades del sistema y las convierte a DTOs
        /// </summary>
        /// <returns>Lista de modalidades en formato DTO</returns>
        public async Task<IEnumerable<TypeModalityDTO>> GetAllTypeModalitiesAsync()
        {
            try
            {
                // Obtener modalidades de la capa de datos
                var typeModalities = await _typeModalityData.GetAllAsync();
                var typeModalitiesDTO = new List<TypeModalityDTO>();

                // Convertir cada modalidad a DTO
                foreach (var typeModality in typeModalities)
                {
                    typeModalitiesDTO.Add(new TypeModalityDTO
                    {
                        Id = typeModality.Id,
                        Name = typeModality.Name,
                        Description = typeModality.Description
                    });
                }

                return typeModalitiesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las modalidades");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de modalidades", ex);
            }
        }

        /// <summary>
        /// Obtiene una modalidad específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la modalidad</param>
        /// <returns>Modalidad en formato DTO</returns>
        public async Task<TypeModalityDTO> GetTypeModalityByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una modalidad con ID inválido: {TypeModalityId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la modalidad debe ser mayor que cero");
            }

            try
            {
                // Buscar la modalidad en la base de datos
                var typeModality = await _typeModalityData.GetByIdAsync(id);
                if (typeModality == null)
                {
                    _logger.LogInformation("No se encontró ninguna modalidad con ID: {TypeModalityId}", id);
                    throw new EntityNotFoundException("TypeModality", id);
                }

                // Convertir la modalidad a DTO
                return new TypeModalityDTO
                {
                    Id = typeModality.Id,
                    Name = typeModality.Name,
                    Description = typeModality.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la modalidad con ID: {TypeModalityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la modalidad con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva modalidad en el sistema
        /// </summary>
        /// <param name="typeModalityDto">Datos de la modalidad a crear</param>
        /// <returns>Modalidad creada en formato DTO</returns>
        public async Task<TypeModalityDTO> CreateTypeModalityAsync(TypeModalityDTO typeModalityDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateTypeModality(typeModalityDto);

                // Crear la entidad TypeModality desde el DTO
                var typeModality = new TypeModality
                {
                    Name = typeModalityDto.Name,
                    Description = typeModalityDto.Description
                };

                // Guardar la modalidad en la base de datos
                var typeModalityCreada = await _typeModalityData.CreateAsync(typeModality);

                // Convertir la modalidad creada a DTO para la respuesta
                return new TypeModalityDTO
                {
                    Id = typeModalityCreada.Id,
                    Name = typeModalityCreada.Name,
                    Description = typeModalityCreada.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva modalidad: {TypeModalityName}", typeModalityDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la modalidad", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de modalidad
        /// </summary>
        /// <param name="typeModalityDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateTypeModality(TypeModalityDTO typeModalityDto)
        {
            // Validar que el DTO no sea nulo
            if (typeModalityDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto modalidad no puede ser nulo");
            }

            // Validar que el Name no esté vacío
            if (string.IsNullOrWhiteSpace(typeModalityDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una modalidad con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la modalidad es obligatorio");
            }
        }
    }
}






