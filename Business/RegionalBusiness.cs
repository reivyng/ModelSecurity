using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los registros regionales.
    /// Implementa la lógica de negocio para la gestión de registros regionales, incluyendo operaciones CRUD.
    /// </summary>
    public class RegionalBusiness
    {
        // Dependencias inyectadas
        private readonly RegionalData _regionalData; // Acceso a la capa de datos
        private readonly ILogger _logger;           // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="regionalData">Servicio de acceso a datos para registros regionales</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public RegionalBusiness(RegionalData regionalData, ILogger logger)
        {
            _regionalData = regionalData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros regionales del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de registros regionales en formato DTO</returns>
        public async Task<IEnumerable<RegionalDTO>> GetAllRegionalsAsync()
        {
            try
            {
                // Obtener registros regionales de la capa de datos
                var regionals = await _regionalData.GetAllAsync();
                var regionalsDTO = new List<RegionalDTO>();

                // Convertir cada registro regional a DTO
                foreach (var regional in regionals)
                {
                    regionalsDTO.Add(new RegionalDTO
                    {
                        Id = regional.Id,
                        Name = regional.Name,
                        CodeRegional = regional.Code_Regional
                    });
                }

                return regionalsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros regionales");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de registros regionales", ex);
            }
        }

        /// <summary>
        /// Obtiene un registro regional específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del registro regional</param>
        /// <returns>Registro regional en formato DTO</returns>
        public async Task<RegionalDTO> GetRegionalByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un registro regional con ID inválido: {RegionalId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del registro regional debe ser mayor que cero");
            }

            try
            {
                // Buscar el registro regional en la base de datos
                var regional = await _regionalData.GetByIdAsync(id);
                if (regional == null)
                {
                    _logger.LogInformation("No se encontró ningún registro regional con ID: {RegionalId}", id);
                    throw new EntityNotFoundException("Regional", id);
                }

                // Convertir el registro regional a DTO
                return new RegionalDTO
                {
                    Id = regional.Id,
                    Name = regional.Name,
                    CodeRegional = regional.Code_Regional
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro regional con ID: {RegionalId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el registro regional con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo registro regional en el sistema
        /// </summary>
        /// <param name="regionalDto">Datos del registro regional a crear</param>
        /// <returns>Registro regional creado en formato DTO</returns>
        public async Task<RegionalDTO> CreateRegionalAsync(RegionalDTO regionalDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateRegional(regionalDto);

                // Crear la entidad Regional desde el DTO
                var regional = new Regional
                {
                    Name = regionalDto.Name,
                    Code_Regional = regionalDto.CodeRegional
                };

                // Guardar el registro regional en la base de datos
                var regionalCreado = await _regionalData.CreateAsync(regional);

                // Convertir el registro regional creado a DTO para la respuesta
                return new RegionalDTO
                {
                    Id = regionalCreado.Id,
                    Name = regionalCreado.Name,
                    CodeRegional = regionalCreado.Code_Regional
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo registro regional: {RegionalName}", regionalDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el registro regional", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de registro regional
        /// </summary>
        /// <param name="regionalDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateRegional(RegionalDTO regionalDto)
        {
            // Validar que el DTO no sea nulo
            if (regionalDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto registro regional no puede ser nulo");
            }

            // Validar que el Name no esté vacío
            if (string.IsNullOrWhiteSpace(regionalDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un registro regional con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del registro regional es obligatorio");
            }
        }
    }
}




