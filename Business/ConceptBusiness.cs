using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los conceptos en el sistema.
    /// </summary>
    public class ConceptBusiness
    {
        private readonly ConceptData _conceptData;
        private readonly ILogger<ConceptBusiness> _logger;

        public ConceptBusiness(ConceptData conceptData, ILogger<ConceptBusiness> logger)
        {
            _conceptData = conceptData;
            _logger = logger;
        }

        // Método para obtener todos los conceptos como DTOs
        public async Task<IEnumerable<ConceptDto>> GetAllConceptsAsync()
        {
            try
            {
                var concepts = await _conceptData.GetAllAsync();
                return MapToDTOList(concepts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los conceptos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de conceptos", ex);
            }
        }

        // Método para obtener un concepto por ID como DTO
        public async Task<ConceptDto> GetConceptByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un concepto con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del concepto debe ser mayor que cero");
            }

            try
            {
                var concept = await _conceptData.GetByIdAsync(id);
                if (concept == null)
                {
                    _logger.LogInformation("No se encontró ningún concepto con ID: {Id}", id);
                    throw new EntityNotFoundException("concept", id);
                }

                return MapToDTO(concept);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el concepto con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el concepto con ID {id}", ex);
            }
        }

        // Método para crear un concepto desde un DTO
        public async Task<ConceptDto> CreateConceptAsync(ConceptDto conceptDto)
        {
            try
            {
                ValidateConcept(conceptDto);

                var concept = MapToEntity(conceptDto);
                concept.CreateDate = DateTime.Now;
                var conceptCreado = await _conceptData.CreateAsync(concept);

                return MapToDTO(conceptCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo concepto: {Name}", conceptDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el concepto", ex);
            }
        }

        // Método para actualizar un concepto existente
        public async Task<bool> UpdateConceptAsync(ConceptDto conceptDto)
        {
            try
            {
                ValidateConcept(conceptDto);

                var concept = MapToEntity(conceptDto);

                var result = await _conceptData.UpdateAsync(concept);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar el concepto con ID {ConceptId}", conceptDto.Id);
                    throw new EntityNotFoundException("Concept", conceptDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el concepto con ID {ConceptId}", conceptDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el concepto con ID {conceptDto.Id}", ex);
            }
        }

        // Método para actualizar campos específicos de un concepto
        public async Task<bool> UpdatePartialConceptAsync(int id, ConceptDto updatedFields)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un concepto con un ID inválido: {ConceptId}", id);
                throw new ValidationException("id", "El ID del concepto debe ser mayor a 0");
            }

            try
            {
                var existingConcept = await _conceptData.GetByIdAsync(id);
                if (existingConcept == null)
                {
                    _logger.LogInformation("No se encontró el concepto con ID {ConceptId} para actualización parcial", id);
                    throw new EntityNotFoundException("Concept", id);
                }

                if (!string.IsNullOrWhiteSpace(updatedFields.Name))
                {
                    existingConcept.Name = updatedFields.Name;
                }
                if (!string.IsNullOrWhiteSpace(updatedFields.Observation))
                {
                    existingConcept.Observation = updatedFields.Observation;
                }
                if (updatedFields.Active != existingConcept.Active)
                {
                    existingConcept.Active = updatedFields.Active;
                }

                var result = await _conceptData.UpdateAsync(existingConcept);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar parcialmente el concepto con ID {ConceptId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el concepto con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el concepto con ID {ConceptId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el concepto con ID {id}", ex);
            }
        }

        // Método para realizar una eliminación lógica de un concepto
        public async Task<bool> SoftDeleteConceptAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una eliminación lógica con un ID inválido: {ConceptId}", id);
                throw new ValidationException("id", "El ID del concepto debe ser mayor a 0");
            }

            try
            {
                var concept = await _conceptData.GetByIdAsync(id);
                if (concept == null)
                {
                    _logger.LogInformation("No se encontró el concepto con ID {ConceptId} para eliminación lógica", id);
                    throw new EntityNotFoundException("Concept", id);
                }

                concept.Active = false;

                var result = await _conceptData.UpdateAsync(concept);

                if (!result)
                {
                    _logger.LogWarning("No se pudo realizar la eliminación lógica del concepto con ID {ConceptId}", id);
                    throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del concepto con ID {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar la eliminación lógica del concepto con ID {ConceptId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al realizar la eliminación lógica del concepto con ID {id}", ex);
            }
        }

        // Método para eliminar un concepto por su ID
        public async Task<bool> DeleteConceptAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un concepto con un ID inválido: {ConceptId}", id);
                throw new ValidationException("id", "El ID del concepto debe ser mayor a 0");
            }

            try
            {
                var result = await _conceptData.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogInformation("No se encontró el concepto con ID {ConceptId} para eliminar", id);
                    throw new EntityNotFoundException("Concept", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el concepto con ID {ConceptId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el concepto con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateConcept(ConceptDto conceptDto)
        {
            if (conceptDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Concept no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(conceptDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un concepto con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del concepto es obligatorio");
            }
        }

        // Método para mapear de Concept a ConceptDto
        private ConceptDto MapToDTO(Concept concept)
        {
            return new ConceptDto
            {
                Id = concept.Id,
                Name = concept.Name,
                Observation = concept.Observation,
                Active = concept.Active,
            };
        }

        // Método para mapear de ConceptDto a Concept
        private Concept MapToEntity(ConceptDto conceptDto)
        {
            return new Concept
            {
                Id = conceptDto.Id,
                Name = conceptDto.Name,
                Observation = conceptDto.Observation,
                Active = conceptDto.Active,
            };
        }

        // Método para mapear una lista de Concept a una lista de ConceptDto
        private IEnumerable<ConceptDto> MapToDTOList(IEnumerable<Concept> concepts)
        {
            var conceptsDTO = new List<ConceptDto>();
            foreach (var concept in concepts)
            {
                conceptsDTO.Add(MapToDTO(concept));
            }
            return conceptsDTO;
        }
    }
}
