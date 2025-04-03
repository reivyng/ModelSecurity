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
    /// Controlador para la gestión de sedes en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SedeController : ControllerBase
    {
        private readonly SedeBusiness _sedeBusiness;
        private readonly ILogger<SedeController> _logger;

        /// <summary>
        /// Constructor del controlador de sedes
        /// </summary>
        /// <param name="sedeBusiness">Capa de negocio de sedes</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public SedeController(SedeBusiness sedeBusiness, ILogger<SedeController> logger)
        {
            _sedeBusiness = sedeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las sedes del sistema
        /// </summary>
        /// <returns>Lista de sedes</returns>
        /// <response code="200">Retorna la lista de sedes</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SedeDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllSedes()
        {
            try
            {
                var sedes = await _sedeBusiness.GetAllSedesAsync();
                return Ok(sedes);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener sedes");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una sede específica por su ID
        /// </summary>
        /// <param name="id">ID de la sede</param>
        /// <returns>Sede solicitada</returns>
        /// <response code="200">Retorna la sede solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Sede no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SedeDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSedeById(int id)
        {
            try
            {
                var sede = await _sedeBusiness.GetSedeByIdAsync(id);
                return Ok(sede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la sede con ID: {SedeId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Sede no encontrada con ID: {SedeId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener sede con ID: {SedeId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva sede en el sistema
        /// </summary>
        /// <param name="sedeDto">Datos de la sede a crear</param>
        /// <returns>Sede creada</returns>
        /// <response code="201">Retorna la sede creada</response>
        /// <response code="400">Datos de la sede no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(SedeDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateSede([FromBody] SedeDTO sedeDto)
        {
            try
            {
                var createdSede = await _sedeBusiness.CreateSedeAsync(sedeDto);
                return CreatedAtAction(nameof(GetSedeById), new { id = createdSede.Id }, createdSede);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear sede");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear sede");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}