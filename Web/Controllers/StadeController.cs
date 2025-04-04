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
    /// Controlador para la gestión de registros de Stade en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StadeController : ControllerBase
    {
        private readonly StadeBusiness _stadeBusiness;
        private readonly ILogger<StadeController> _logger;

        /// <summary>
        /// Constructor del controlador de registros de Stade
        /// </summary>
        /// <param name="stadeBusiness">Capa de negocio de registros de Stade</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public StadeController(StadeBusiness stadeBusiness, ILogger<StadeController> logger)
        {
            _stadeBusiness = stadeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de Stade del sistema
        /// </summary>
        /// <returns>Lista de registros de Stade</returns>
        /// <response code="200">Retorna la lista de registros de Stade</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StateDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllStades()
        {
            try
            {
                var stades = await _stadeBusiness.GetAllStadesAsync();
                return Ok(stades);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registros de Stade");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un registro de Stade específico por su ID
        /// </summary>
        /// <param name="id">ID del registro de Stade</param>
        /// <returns>Registro de Stade solicitado</returns>
        /// <response code="200">Retorna el registro de Stade solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Registro de Stade no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StateDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStadeById(int id)
        {
            try
            {
                var stade = await _stadeBusiness.GetStadeByIdAsync(id);
                return Ok(stade);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el registro de Stade con ID: {StadeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Registro de Stade no encontrado con ID: {StadeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener registro de Stade con ID: {StadeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro de Stade en el sistema
        /// </summary>
        /// <param name="stadeDto">Datos del registro de Stade a crear</param>
        /// <returns>Registro de Stade creado</returns>
        /// <response code="201">Retorna el registro de Stade creado</response>
        /// <response code="400">Datos del registro de Stade no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(StateDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateStade([FromBody] StateDTO stadeDto)
        {
            try
            {
                var createdStade = await _stadeBusiness.CreateStadeAsync(stadeDto);
                return CreatedAtAction(nameof(GetStadeById), new { id = createdStade.Id }, createdStade);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear registro de Stade");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear registro de Stade");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}