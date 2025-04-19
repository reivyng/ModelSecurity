using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Concept en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ConceptController : ControllerBase
    {
        private readonly ConceptBusiness _conceptBusiness;
        private readonly ILogger<ConceptController> _logger;

        /// <summary>
        /// Constructor del controlador de Concept
        /// </summary>
        public ConceptController(ConceptBusiness conceptBusiness, ILogger<ConceptController> logger)
        {
            _conceptBusiness = conceptBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los conceptos del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConceptDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllConcepts()
        {
            try
            {
                var concepts = await _conceptBusiness.GetAllConceptsAsync();
                return Ok(concepts);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener conceptos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un concepto específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConceptDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetConceptById(int id)
        {
            try
            {
                var concept = await _conceptBusiness.GetConceptByIdAsync(id);
                return Ok(concept);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el concepto con ID: {ConceptId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Concepto no encontrado con ID: {ConceptId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener concepto con ID: {ConceptId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo concepto en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ConceptDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateConcept([FromBody] ConceptDto conceptDto)
        {
            try
            {
                var createdConcept = await _conceptBusiness.CreateConceptAsync(conceptDto);
                return CreatedAtAction(nameof(GetConceptById), new { id = createdConcept.Id }, createdConcept);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear concepto");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear concepto");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un concepto existente
        /// </summary>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateConcept([FromBody] ConceptDto conceptDto)
        {
            try
            {
                var result = await _conceptBusiness.UpdateConceptAsync(conceptDto);
                if (result)
                {
                    return Ok(new { message = "El concepto fue actualizado exitosamente." });
                }
                return NotFound(new { message = "El concepto no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar concepto");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar concepto");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un concepto existente
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePartialConcept(int id, [FromBody] ConceptDto updatedFields)
        {
            try
            {
                var result = await _conceptBusiness.UpdatePartialConceptAsync(id, updatedFields);
                if (result)
                {
                    return Ok(new { message = "El concepto fue actualizado parcialmente con éxito." });
                }
                return NotFound(new { message = "El concepto no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar parcialmente concepto con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente concepto con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza una eliminación lógica de un concepto
        /// </summary>
        [HttpDelete("soft/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteConcept(int id)
        {
            try
            {
                var result = await _conceptBusiness.SoftDeleteConceptAsync(id);
                if (result)
                {
                    return Ok(new { message = "El concepto fue eliminado lógicamente con éxito." });
                }
                return NotFound(new { message = "El concepto no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar eliminación lógica del concepto con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar eliminación lógica del concepto con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un concepto por su ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteConcept(int id)
        {
            try
            {
                var result = await _conceptBusiness.DeleteConceptAsync(id);
                if (result)
                {
                    return Ok(new { message = "El concepto fue eliminado exitosamente." });
                }
                return NotFound(new { message = "El concepto no fue encontrado." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar concepto con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar concepto con ID: {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
