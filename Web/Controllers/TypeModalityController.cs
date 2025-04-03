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
    /// Controlador para la gestión de modalidades en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TypeModalityController : ControllerBase
    {
        private readonly TypeModalityBusiness _typeModalityBusiness;
        private readonly ILogger<TypeModalityController> _logger;

        /// <summary>
        /// Constructor del controlador de modalidades
        /// </summary>
        /// <param name="typeModalityBusiness">Capa de negocio de modalidades</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public TypeModalityController(TypeModalityBusiness typeModalityBusiness, ILogger<TypeModalityController> logger)
        {
            _typeModalityBusiness = typeModalityBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las modalidades del sistema
        /// </summary>
        /// <returns>Lista de modalidades</returns>
        /// <response code="200">Retorna la lista de modalidades</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TypeModalityDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllTypeModalities()
        {
            try
            {
                var typeModalities = await _typeModalityBusiness.GetAllTypeModalitiesAsync();
                return Ok(typeModalities);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener modalidades");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una modalidad específica por su ID
        /// </summary>
        /// <param name="id">ID de la modalidad</param>
        /// <returns>Modalidad solicitada</returns>
        /// <response code="200">Retorna la modalidad solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Modalidad no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TypeModalityDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTypeModalityById(int id)
        {
            try
            {
                var typeModality = await _typeModalityBusiness.GetTypeModalityByIdAsync(id);
                return Ok(typeModality);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la modalidad con ID: {TypeModalityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Modalidad no encontrada con ID: {TypeModalityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener modalidad con ID: {TypeModalityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva modalidad en el sistema
        /// </summary>
        /// <param name="typeModalityDto">Datos de la modalidad a crear</param>
        /// <returns>Modalidad creada</returns>
        /// <response code="201">Retorna la modalidad creada</response>
        /// <response code="400">Datos de la modalidad no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(TypeModalityDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateTypeModality([FromBody] TypeModalityDTO typeModalityDto)
        {
            try
            {
                var createdTypeModality = await _typeModalityBusiness.CreateTypeModalityAsync(typeModalityDto);
                return CreatedAtAction(nameof(GetTypeModalityById), new { id = createdTypeModality.Id }, createdTypeModality);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear modalidad");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear modalidad");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}