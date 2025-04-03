using Business;
using Entity.DTOautogestion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de conceptos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ConceptController : ControllerBase
    {
        private readonly ConceptBusiness _conceptBusiness;
        private readonly ILogger<ConceptController> _logger;

        /// <summary>
        /// Constructor del controlador de conceptos
        /// </summary>
        /// <param name="conceptBusiness">Capa de negocio de conceptos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ConceptController(ConceptBusiness conceptBusiness, ILogger<ConceptController> logger)
        {
            _conceptBusiness = conceptBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los conceptos del sistema
        /// </summary>
        /// <returns>Lista de conceptos</returns>
        /// <response code="200">Retorna la lista de conceptos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConceptDTO>), 200)]
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
        /// <param name="id">ID del concepto</param>
        /// <returns>Concepto solicitado</returns>
        /// <response code="200">Retorna el concepto solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Concepto no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConceptDTO), 200)]
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
        /// <param name="conceptDto">Datos del concepto a crear</param>
        /// <returns>Concepto creado</returns>
        /// <response code="201">Retorna el concepto creado</response>
        /// <response code="400">Datos del concepto no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ConceptDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateConcept([FromBody] ConceptDTO conceptDto)
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
    }
}

