using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los conceptos.
    /// Implementa la lógica de negocio para la gestión de conceptos, incluyendo operaciones CRUD.
    /// </summary>
    public class ConceptBusiness
    {
        // Dependencias inyectadas
        private readonly ConceptData _conceptData; // Acceso a la capa de datos
        private readonly ILogger _logger;         // Servicio de logging

        /// <summary>
        /// Constructor que recibe las dependencias necesarias
        /// </summary>
        /// <param name="conceptData">Servicio de acceso a datos para conceptos</param>
        /// <param name="logger">Servicio de logging para registro de eventos</param>
        public ConceptBusiness(ConceptData conceptData, ILogger logger)
        {
            _conceptData = conceptData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los conceptos del sistema y los convierte a DTOs
        /// </summary>
        /// <returns>Lista de conceptos en formato DTO</returns>
        public async Task<IEnumerable<ConceptDTO>> GetAllConceptsAsync()
        {
            try
            {
                // Obtener conceptos de la capa de datos
                var concepts = await _conceptData.GetAllAsync();
                var conceptsDTO = new List<ConceptDTO>();

                // Convertir cada concepto a DTO
                foreach (var concept in concepts)
                {
                    conceptsDTO.Add(new ConceptDTO
                    {
                        Id = concept.Id,
                        Observation = concept.Observation
                    });
                }

                return conceptsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los conceptos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de conceptos", ex);
            }
        }

        /// <summary>
        /// Obtiene un concepto específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del concepto</param>
        /// <returns>Concepto en formato DTO</returns>
        public async Task<ConceptDTO> GetConceptByIdAsync(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un concepto con ID inválido: {ConceptId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del concepto debe ser mayor que cero");
            }

            try
            {
                // Buscar el concepto en la base de datos
                var concept = await _conceptData.GetByIdAsync(id);
                if (concept == null)
                {
                    _logger.LogInformation("No se encontró ningún concepto con ID: {ConceptId}", id);
                    throw new EntityNotFoundException("Concept", id);
                }

                // Convertir el concepto a DTO
                return new ConceptDTO
                {
                    Id = concept.Id,
                    Observation = concept.Observation
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el concepto con ID: {ConceptId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el concepto con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo concepto en el sistema
        /// </summary>
        /// <param name="conceptDto">Datos del concepto a crear</param>
        /// <returns>Concepto creado en formato DTO</returns>
        public async Task<ConceptDTO> CreateConceptAsync(ConceptDTO conceptDto)
        {
            try
            {
                // Validar los datos del DTO
                ValidateConcept(conceptDto);

                // Crear la entidad Concept desde el DTO
                var concept = new Concept
                {
                    Observation = conceptDto.Observation
                };

                // Guardar el concepto en la base de datos
                var conceptCreado = await _conceptData.CreateAsync(concept);

                // Convertir el concepto creado a DTO para la respuesta
                return new ConceptDTO
                {
                    Id = concept.Id,
                    Observation = concept.Observation
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo concepto: {ConceptName}", conceptDto?.Observation ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el concepto", ex);
            }
        }

        /// <summary>
        /// Valida los datos del DTO de concepto
        /// </summary>
        /// <param name="conceptDto">DTO a validar</param>
        /// <exception cref="ValidationException">Se lanza cuando los datos no son válidos</exception>
        private void ValidateConcept(ConceptDTO conceptDto)
        {
            // Validar que el DTO no sea nulo
            if (conceptDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto concepto no puede ser nulo");
            }

            // Validar que el Name no esté vacío
            if (string.IsNullOrWhiteSpace(conceptDto.Observation))
            {
                _logger.LogWarning("Se intentó crear/actualizar un concepto con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del concepto es obligatorio");
            }
        }
    }
}

